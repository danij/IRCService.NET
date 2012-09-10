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
using IRCServiceNET.Helpers;
using IRCServiceNET.Entities;

namespace IRCServiceNET.Protocols.P10.Parsers
{
    /// <summary>
    /// Parses a user quit
    /// </summary>
    public class Q_Parser : CommandParser
    {
        public override string CommandHeader
        {
            get { return "Q"; }
        }

        public override void Parse(string[] spaceSplit, string[] colonSplit, 
            string fullRow)
        {
            if (spaceSplit.Count() < 2)
            {
                return;
            }

            string reason = "";
            if (colonSplit.Count() > 1)
            {
                reason = StringHelper.JoinArray(colonSplit, ":", 1);
            }

            if (spaceSplit[0].Length != 5)
            {
                Server serverToRemove = null;
                if (spaceSplit[0].Length == 2)
                {
                    serverToRemove = Service.GetServer(spaceSplit[0]);
                }
                else
                {
                    serverToRemove = Service.GetServerByName(spaceSplit[0]);
                }
                if (serverToRemove == null)
                {
                    Service.AddLog("Unknown server " + spaceSplit[0] + " quits");
                    return;
                }
                Service.RemoveServer(serverToRemove, reason);
                return;
            }

            var toRemove = Service.GetUser(spaceSplit[0]);
            if (toRemove == null)
            {
                Service.AddLog("Unable to find " + spaceSplit[0]);
                return;
            }
            (toRemove.Server as Server).RemoveUser(toRemove);

            Service.SendActionToPlugins(p => p.OnUserQuit(toRemove, reason));
        }
    }
}
