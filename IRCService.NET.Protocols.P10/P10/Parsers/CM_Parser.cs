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

            Channel channel = null;

            foreach (var item in Service.Servers)
            {
                channel = item.GetChannel(spaceSplit[2]) as Channel;
                if (channel != null)
                {
                    break;
                }
            }
            if (channel == null)
            {
                Service.AddLog("Channel " + spaceSplit[2] + " was not found");
                return;
            }
            for (int i = 0; i < spaceSplit[3].Length; i++)
            {
                switch (spaceSplit[3][i])
                {
                    case 'n':
                        channel.SetMode(ChannelModes.n, false);
                        break;
                    case 't':
                        channel.SetMode(ChannelModes.t, false);
                        break;
                    case 'i':
                        channel.SetMode(ChannelModes.i, false);
                        break;
                    case 'r':
                        channel.SetMode(ChannelModes.r, false);
                        break;
                    case 'p':
                        channel.SetMode(ChannelModes.p, false);
                        break;
                    case 's':
                        channel.SetMode(ChannelModes.s, false);
                        break;
                    case 'm':
                        channel.SetMode(ChannelModes.m, false);
                        break;
                    case 'O':
                        channel.SetMode(ChannelModes.O, false);
                        break;
                    case 'c':
                        channel.SetMode(ChannelModes.c, false);
                        break;
                    case 'C':
                        channel.SetMode(ChannelModes.C, false);
                        break;
                    case 'l':
                        channel.SetMode(ChannelModes.l, 0);
                        break;
                    case 'k':
                        channel.SetMode(ChannelModes.k, "");
                        break;
                    case 'b':
                        channel.ClearBans();
                        break;
                    case 'o':
                        channel.ClearOp();
                        break;
                    case 'v':
                        channel.ClearVoice();
                        break;
                    case 'h':
                        channel.ClearHalfOp();
                        break;
                }
            }

            if (from != null)
            {
                Service.SendActionToPlugins(
                    p => p.OnChannelClearModes(channel, spaceSplit[3], 
                        from),
                    from.Plugin
                );
            }
            else
            {
                Service.SendActionToPlugins(
                    p => p.OnChannelClearModes(channel, spaceSplit[3], 
                        serverFrom),
                    serverFrom.Plugin
                );
            }
        }
    }
}
