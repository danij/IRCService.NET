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
    /// Parses a notice
    /// </summary>
    public class O_Parser : CommandParser
    {
        public override string CommandHeader
        {
            get { return "O"; }
        }

        public override void Parse(string[] spaceSplit, string[] colonSplit,
            string fullRow)
        {
            if (spaceSplit.Count() < 4)
            {
                return;
            }
            if (colonSplit.Count() < 2)
            {
                return;
            }

            User from = null;
            Server serverFrom = null;

            if (spaceSplit[0].Length == 5)
            {
                from = Service.GetUser(spaceSplit[0]);
                if (from == null)
                {
                    Service.AddLog("Private Notice from unknown user " +
                        spaceSplit[0]);
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
                    Service.AddLog("Private Notice from unknown server " +
                        spaceSplit[0]);
                }
            }

            string message = StringHelper.JoinArray(colonSplit, ":", 1);
            if (spaceSplit[2][0] == '$')
            {
                if (from != null)
                {
                    Service.SendActionToPlugins(
                        p => p.OnGlobalNotice(from, spaceSplit[2], message),
                        from.Plugin
                    );
                }
                else
                {
                    Service.SendActionToPlugins(
                        p => p.OnGlobalNotice(serverFrom, spaceSplit[2],
                            message),
                        serverFrom.Plugin
                    );
                }
                return;
            }
            if (spaceSplit[2][0] != '#')
            {
                User to = Service.GetUser(spaceSplit[2]);
                if (to == null)
                {
                    Service.AddLog("Private Notice for unknown user " +
                        spaceSplit[2]);
                    return;
                }
                if (to.Plugin != null)
                {
                    if (message.Length > 2 && message[0] == IRCConstants.CTCP &&
                            message[message.Length - 1] == IRCConstants.CTCP)
                    {
                        message = message.Substring(1, message.Length - 2);
                        string[] ctcpMessage = message.Split(' ');
                        string parameter = "";
                        if (ctcpMessage.Length > 1)
                        {
                            parameter = ctcpMessage[1];
                        }
                        if (from != null)
                        {
                            to.Plugin.OnCTCPReply(
                                from,
                                to,
                                ctcpMessage[0],
                                parameter
                            );
                        }
                        else
                        {
                            to.Plugin.OnCTCPReply(
                                serverFrom,
                                to,
                                ctcpMessage[0],
                                parameter
                            );
                        }
                        return;
                    }
                    else
                    {
                        if (from != null)
                        {
                            to.Plugin.OnNotice(from, to, message);
                        }
                        else
                        {
                            to.Plugin.OnNotice(serverFrom, to, message);
                        }
                    }
                }
            }
            else
            {
                if (message.Length > 2 && message[0] == IRCConstants.CTCP
                    && message[message.Length - 1] == IRCConstants.CTCP)
                {
                    message = message.Substring(1, message.Length - 2);
                    string[] ctcpMessage = message.Split(' ');
                    string parameter = "";
                    if (ctcpMessage.Length > 1)
                    {
                        parameter = ctcpMessage[1];
                    }
                    if (from != null)
                    {
                        Service.SendActionToPlugins(
                            p => p.OnChannelCTCP(from, spaceSplit[2],
                                ctcpMessage[0], parameter),
                            from.Plugin
                        );
                    }
                    else
                    {
                        Service.SendActionToPlugins(
                            p => p.OnChannelCTCP(serverFrom, spaceSplit[2],
                                ctcpMessage[0], parameter),
                            serverFrom.Plugin
                        );
                    }
                    return;
                }
                else
                {
                    if (from != null)
                    {
                        Service.SendActionToPlugins(
                            p => p.OnChannelNotice(from, spaceSplit[2],
                                message),
                            from.Plugin
                        );
                    }
                    else
                    {
                        Service.SendActionToPlugins(
                            p => p.OnChannelNotice(serverFrom, spaceSplit[2],
                                message),
                            serverFrom.Plugin
                        );
                    }
                }
            }
        }           
    }
}
