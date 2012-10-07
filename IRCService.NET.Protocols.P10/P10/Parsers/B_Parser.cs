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
    /// Parses a channel burst
    /// </summary>
    public class B_Parser : CommandParser
    {
        public override string CommandHeader
        {
            get { return "B"; }
        }

        public override void Parse(string[] spaceSplit, string[] colonSplit, 
            string fullRow)
        {
            if (spaceSplit.Count() < 5)
            {
                return;
            }

            string channelName = spaceSplit[2];
            Channel channel = null;
            UnixTimestamp creationTimestamp = 
                new UnixTimestamp(Convert.ToInt32(spaceSplit[3]));
            int userIndex = 4;
            int channelModes = 0;
            string key = "";
            int limit = 0;

            if (spaceSplit[4][0] == '+')
            {
                userIndex += 1;
                for (int i = 0; i < spaceSplit[4].Length; i++)
                {
                    switch (spaceSplit[4][i])
                    {
                        case 'n': channelModes |= (int)ChannelModes.n; break;
                        case 't': channelModes |= (int)ChannelModes.t; break;
                        case 'm': channelModes |= (int)ChannelModes.m; break;
                        case 'i': channelModes |= (int)ChannelModes.i; break;
                        case 's': channelModes |= (int)ChannelModes.s; break;
                        case 'p': channelModes |= (int)ChannelModes.p; break;
                        case 'r': channelModes |= (int)ChannelModes.r; break;
                        case 'O': channelModes |= (int)ChannelModes.O; break;
                        case 'c': channelModes |= (int)ChannelModes.c; break;
                        case 'C': channelModes |= (int)ChannelModes.C; break;
                        case 'k': channelModes |= (int)ChannelModes.k;
                            if (spaceSplit.Count() > (userIndex + 1))
                            {
                                key = spaceSplit[userIndex];
                                userIndex += 1;
                            }
                            break;
                        case 'l': channelModes |= (int)ChannelModes.l;
                            if (spaceSplit.Count() > (userIndex + 1))
                            {
                                limit = Convert.ToInt32(spaceSplit[userIndex]);
                                if (limit < 0)
                                {
                                    limit = 0;

                                }
                                userIndex += 1;
                            }
                            break;
                    }
                }
            }
            if (spaceSplit.Count() > userIndex)
            {
                bool Op = false;
                bool HalfOp = false;
                bool Voice = false;
                string channelUserNumeric;
                IUser channelUser;
                string[] ChannelUsers = spaceSplit[userIndex].Split(',');
                if (spaceSplit[userIndex][0] != ':')
                {
                    for (int i = 0; i < ChannelUsers.Length; i++)
                    {
                        if (ChannelUsers[i].Length == 5)
                        {
                            channelUserNumeric = ChannelUsers[i];
                        }
                        else
                        {
                            string[] modeSplit = ChannelUsers[i].Split(':');
                            if (modeSplit.Length > 1)
                            {
                                Op = false;
                                HalfOp = false;
                                Voice = false;
                                for (int ii = 0; ii < modeSplit[1].Length; ii++)
                                {
                                    switch (modeSplit[1][ii])
                                    {
                                        case 'o': Op = true; break;
                                        case 'h': HalfOp = true; break;
                                        case 'v': Voice = true; break;
                                    }
                                }
                            }
                            else
                            {
                                continue;
                            }
                            channelUserNumeric = modeSplit[0];
                        }
                        channelUser = Service.GetUser(channelUserNumeric);
                        if (channelUser == null)
                        {
                            Service.AddLog("Burst for unknown user " + 
                                channelUserNumeric + " on channel " + channelName);
                            return;
                        }

                        channel = Service.GetChannel(channelName) as Channel;
                        if (channel == null)
                        {
                            channel = Service.CreateChannel(channelName, 
                                creationTimestamp) as Channel;
                        }
                        else
                        {
                            channel.CreationTimeStamp = creationTimestamp;
                        }

                        if (channelUser.Server.GetChannel(channelName) == null)
                        {
                            (channelUser.Server as Server).AddChannel(channel);
                        }

                        channel.AddUser(channelUser, Op, Voice, HalfOp);
                    }
                }
            }

            List<Ban> bans = new List<Ban>();
            string[] banList = fullRow.Split(
                new string[] { ":%" }, 
                StringSplitOptions.None
            );
            if (banList.Length > 1)
            {
                string[] banStrings = banList[1].Split(' ');
                foreach (string currentBan in banStrings)
                {
                    bans.Add(new Ban(currentBan));
                }
            }

            if (channel == null)
            {
                channel = Service.GetChannel(channelName) as Channel;
                if (channel == null)
                {
                    return;
                }
            }

            if (channel.CreationTimeStamp.Timestamp > 
                creationTimestamp.Timestamp)
            {
                channel.CreationTimeStamp = creationTimestamp;
            }
            if (channelModes > 0)
            {
                channel.SetMode(channelModes);
                if (limit > 0)
                {
                    channel.SetMode(ChannelModes.l, limit);
                }
                if (key.Length > 0)
                {
                    channel.SetMode(ChannelModes.k, key);
                }
            }
            if (bans.Count > 0)
            {
                channel.AddBan(bans);
            }
        }
    }
}
