//IRCService.NET. Generic IRC service library for .NET
//Copyright (C) 2010-2012 Dani J

//This program is free software: you can redistribute it and/or modify
//it under the terms of the GNU General Public License as published by
//the Free Software Foundation, either version 3 of the License, or
//(at your option) any later version.

//This program is distributed in the hope that it will be useful,
//but WITHOUT ANY WARRANTY; without even the implied warranty of
//MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//GNU General Public License for more details.

//You should have received a copy of the GNU General Public License
//along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IRCServiceNET.Entities;
using IRCServiceNET.Helpers;

namespace IRCServiceNET.Protocols.P10.Parsers
{
    /// <summary>
    /// Parses a new user introduction
    /// </summary>
    public class N_Parser : CommandParser
    {
        public override string CommandHeader
        {
            get { return "N"; }
        }

        public override void Parse(string[] spaceSplit, string[] colonSplit, 
            string fullRow)
        {
            if (spaceSplit.Count() == 4)
            {
                //Nick Change
                var nickChangeUser = Service.GetUser(spaceSplit[0]) as User;
                if (nickChangeUser == null)
                {
                    Service.AddLog("Nick change from unknown user " + spaceSplit[0]);
                    return;
                }
                string newnick = spaceSplit[2];
                if (Service.NickExists(newnick))
                {
                    Service.AddLog("Nick " + newnick + " already exists");
                    return;
                }
                nickChangeUser.Nick = newnick;

                Service.SendActionToPlugins(
                    p => p.OnUserChangeNick(nickChangeUser), 
                    nickChangeUser.Plugin
                );
            }
            spaceSplit = colonSplit[0].Split(' ');
            if (spaceSplit.Count() < 8)
            {
                return;
            }
            Server server = Service.GetServer(spaceSplit[0]);
            if (server == null)
            {
                Service.AddLog("N from unknown server " + spaceSplit[0]);
                return;
            }
            string userNumeric = spaceSplit[spaceSplit.Count() - 2];
            if (server.ContainsNumeric(userNumeric))
            {
                Service.AddLog("Numeric collision: client " + userNumeric + 
                    " already exists on " + server.Name);
                return;
            }
            if (Service.NickExists(spaceSplit[2]))
            {
                Service.AddLog("Nick collision: " + spaceSplit[2]);
                return;
            }
            string userName = StringHelper.JoinArray(colonSplit, ":", 1);

            User user = new User(server, userNumeric, spaceSplit[2], 
                spaceSplit[5], spaceSplit[6], userName, 
                new UnixTimestamp(Convert.ToInt32(spaceSplit[4])), 
                spaceSplit[spaceSplit.Count() - 3]);

            if (spaceSplit[7][0] == '+')
            {
                if (spaceSplit[7].Contains('o'))
                {
                    user.IsOper = true;
                }
                if (spaceSplit[7].Contains('k'))
                {
                    user.IsService = true;
                }
                int nextdata = 8;
                if (spaceSplit[7].Contains('r'))
                {
                    user.Login = spaceSplit[nextdata];
                    nextdata += 1;
                }
                if (spaceSplit[7].Contains('h'))
                {
                    string[] fakeIH = spaceSplit[nextdata].Split('@');
                    if (fakeIH.Count() > 1)
                    {
                        user.FakeIdent = fakeIH[0];
                        user.FakeHost = fakeIH[1];
                    }
                }
            }
            server.AddUser(user);
            if (Service.Status == ServiceStatus.BurstCompleted)
            {
                Service.SendActionToPlugins(p => p.OnNewUser(user));
            }
        }
    }
}
