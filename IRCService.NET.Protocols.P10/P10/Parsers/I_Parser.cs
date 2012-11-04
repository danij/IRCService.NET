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
    /// Parses a channel invite
    /// </summary>
    public class I_Parser : CommandParser
    {
        public override string CommandHeader
        {
            get { return "I"; }
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
                    Service.AddLog("Unknown user " + spaceSplit[0] + " uses invite");
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
                    Service.AddLog("Unknown server " + spaceSplit[0] + " uses invite");
                    return;
                }
            }

            var channelName = spaceSplit[3];
            if (channelName[0] == ':')
            {
                channelName = channelName.Substring(1);
            }

            var invitedUser = Service.GetUserByNick(spaceSplit[2]);
            if (invitedUser == null)
            {
                Service.AddLog("Unknown user " + spaceSplit[2] +
                    " was invited to " + channelName);
                return;
            }

            var channel = Service.GetChannel(channelName) as Channel;
            if (channel == null)
            {
                Service.AddLog("Unknown channel " + channelName);
                return;
            }

            if ( ! channel.AddInvitiation(invitedUser))
            {
                Service.AddLog(invitedUser.Nick + " was not on " + channelName);
                return;
            }

            if (invitedUser.Plugin != null)
            {
                if (from != null)
                {
                    invitedUser.Plugin.OnUserInvitedToChannel(invitedUser, channel, from);
                }
                else
                {
                    invitedUser.Plugin.OnUserInvitedToChannel(invitedUser, channel, serverFrom);
                }
            }                    
        }
    }
}
