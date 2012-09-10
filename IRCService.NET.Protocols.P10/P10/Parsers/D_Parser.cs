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
    /// Parses a user disconnect
    /// </summary>
    public class D_Parser : CommandParser
    {
        public override string CommandHeader
        {
            get { return "D"; }
        }

        public override void Parse(string[] spaceSplit, string[] colonSplit, 
            string fullRow)
        {
            if (spaceSplit.Count() < 2)
            {
                return;
            }

            var toRemove = Service.GetUser(spaceSplit[2]);
            if (toRemove == null)
            {
                Service.AddLog("Unable to find " + spaceSplit[2]);
                return;
            }

            IUser from = null;
            IServer serverFrom = null;
            if (spaceSplit[0].Length == 5)
            {
                from = Service.GetUser(spaceSplit[0]);
                if (from == null)
                {
                    Service.AddLog("Kill from unknown user " + spaceSplit[0]);
                    return;
                }
            }
            else
            {
                if (spaceSplit[0].Length == 2)
                {
                    serverFrom = Service.GetServer(spaceSplit[0]);
                }
                else
                {
                    serverFrom = Service.GetServerByName(spaceSplit[0]);
                }
                if (serverFrom == null)
                {
                    Service.AddLog("Kill from unknown server " + spaceSplit[0]);
                    return;
                }
            }

            (toRemove.Server as Server).RemoveUser(toRemove);
            string reason = "";

            if (colonSplit.Count() > 1)
            {
                reason = StringHelper.JoinArray(colonSplit, ":", 1);
            }

            if (from != null)
            {
                Service.SendActionToPlugins(
                    p => p.OnUserKilled(toRemove, from, reason),
                    from.Plugin
                );
            }
            else
            {
                Service.SendActionToPlugins(
                    p => p.OnUserKilled(toRemove, serverFrom, reason),
                    serverFrom.Plugin
                );
            }
        }
    }
}
