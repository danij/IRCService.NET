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
    /// Parses a channel kick
    /// </summary>
    public class K_Parser : CommandParser
    {
        public override string CommandHeader
        {
            get { return "K"; }
        }

        public override void Parse(string[] spaceSplit, string[] colonSplit, 
            string fullRow)
        {
            if (spaceSplit.Count() < 4)
            {
                return;
            }

            IUser from = null;
            IServer serverFrom = null;
            if (spaceSplit[0].Length == 5)
            {
                from = Service.GetUser(spaceSplit[0]);
                if (from == null)
                {
                    Service.AddLog("Unknown user " + spaceSplit[0] + " uses kick");
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
                    Service.AddLog("Unknown server " + spaceSplit[0] + " uses kick");
                    return;
                }
            }
            var kickedUser = Service.GetUser(spaceSplit[3]);
            if (kickedUser == null)
            {
                Service.AddLog("Unknown user " + spaceSplit[3] + 
                    " was kicked from " + spaceSplit[2]);
                return;
            }
            var channel = kickedUser.Server.GetChannel(spaceSplit[2]);
            if (channel == null)
            {
                Service.AddLog("Unknown channel " + spaceSplit[2] + " on server " +
                    kickedUser.Server.Name);
                return;
            }
            if ( ! (channel as Channel).RemoveUser(kickedUser))
            {
                Service.AddLog(kickedUser.Nick + " was not on " + spaceSplit[2]);
                return;
            }

            string reason = "";
            if (colonSplit.Count() > 1)
            {
                reason = StringHelper.JoinArray(colonSplit, ":", 1);
            }

            if (from != null)
            {
                Service.SendActionToPlugins(
                    p => p.OnChannelKick(spaceSplit[2], kickedUser, from, reason),
                    from.Plugin
                );
            }
            else
            {
                Service.SendActionToPlugins(
                    p => p.OnChannelKick(spaceSplit[2], kickedUser, serverFrom, 
                        reason),
                    serverFrom.Plugin
                );
            }            
        }
    }
}
