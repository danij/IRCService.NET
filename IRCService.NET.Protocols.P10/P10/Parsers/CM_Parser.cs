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

namespace IRCServiceNET.Protocols.P10.Parsers
{
    /// <summary>
    /// Parses a clearmode
    /// </summary>
    public class CM_Parser : CommandParser
    {
        public override string CommandHeader
        {
            get { return "CM"; }
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
                    Service.AddLog("Mode change from unknown user " + spaceSplit[0]);
                    return;
                }
            }
            if (spaceSplit[0].Length == 2)
            {
                serverFrom = Service.GetServer(spaceSplit[0]);
                if (serverFrom == null)
                {
                    Service.AddLog("Mode change from unknown server " + spaceSplit[0]);
                    return;
                }
            }

            var channelList = new List<IChannel>();
            IChannel channelToAdd;
            foreach (var item in Service.Servers)
            {
                channelToAdd = item.GetChannel(spaceSplit[2]);
                if (channelToAdd != null)
                {
                    channelList.Add(channelToAdd);
                }
            }
            if (channelList.Count < 1)
            {
                Service.AddLog("Channel " + spaceSplit[2] + " was not found");
                return;
            }
            for (int i = 0; i < spaceSplit[3].Length; i++)
            {
                switch (spaceSplit[3][i])
                {
                    case 'n':
                        foreach (Channel item in channelList)
                        {
                            item.SetMode(ChannelModes.n, false);
                        }
                        break;
                    case 't':
                        foreach (Channel item in channelList)
                        {
                            item.SetMode(ChannelModes.t, false);
                        }
                        break;
                    case 'i':
                        foreach (Channel item in channelList)
                        {
                            item.SetMode(ChannelModes.i, false);
                        }
                        break;
                    case 'r':
                        foreach (Channel item in channelList)
                        {
                            item.SetMode(ChannelModes.r, false);
                        }
                        break;
                    case 'p':
                        foreach (Channel item in channelList)
                        {
                            item.SetMode(ChannelModes.p, false);
                        }
                        break;
                    case 's':
                        foreach (Channel item in channelList)
                        {
                            item.SetMode(ChannelModes.s, false);
                        }
                        break;
                    case 'm':
                        foreach (Channel item in channelList)
                        {
                            item.SetMode(ChannelModes.m, false);
                        }
                        break;
                    case 'l':
                        foreach (Channel item in channelList)
                        {
                            item.SetMode(ChannelModes.l, 0);
                        }
                        break;
                    case 'k':
                        foreach (Channel item in channelList)
                        {
                            item.SetMode(ChannelModes.k, "");
                        }
                        break;
                    case 'b':
                        foreach (Channel item in channelList)
                        {
                            item.ClearBans();
                        }
                        break;
                    case 'o':
                        foreach (Channel item in channelList)
                        {
                            item.ClearOp();
                        }
                        break;
                    case 'v':
                        foreach (Channel item in channelList)
                        {
                            item.ClearVoice();
                        }
                        break;
                    case 'h':
                        foreach (Channel item in channelList)
                        {
                            item.ClearHalfOp();
                        }
                        break;
                }
            }

            if (from != null)
            {
                Service.SendActionToPlugins(
                    p => p.OnChannelClearModes(spaceSplit[2], spaceSplit[3], 
                        from),
                    from.Plugin
                );
            }
            else
            {
                Service.SendActionToPlugins(
                    p => p.OnChannelClearModes(spaceSplit[2], spaceSplit[3], 
                        serverFrom),
                    serverFrom.Plugin
                );
            }
        }
    }
}
