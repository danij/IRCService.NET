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
    /// Parses a mode change 
    /// </summary>
    public class M_Parser : CommandParser
    {
        public override string CommandHeader
        {
            get { return "M"; }
        }

        public bool OpMode { get; set; }

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

            bool change = false;

            if (spaceSplit[2][0] != '#')
            {
                User userAffected = Service.GetUserByNick(spaceSplit[2]) as User;
                if (userAffected == null)
                {
                    Service.AddLog("Mode change for unknown user " + spaceSplit[0]);
                    return;
                }
                bool[] changes = new bool[8];
                for (int i = 0; i < spaceSplit[3].Length; i++)
                {
                    switch (spaceSplit[3][i])
                    {
                        case '+': change = true; break;
                        case '-': change = false; break;
                        case 'o': userAffected.IsOper = change; changes[0] = true; break;
                        case 'k': userAffected.IsService = change; changes[1] = true; break;
                        case 'd': userAffected.IsDeaf = change; changes[2] = true; break;
                        case 'i': userAffected.IsInvisible = change; changes[3] = true; break;
                        case 's': userAffected.IsServerNotice = change; changes[4] = true; break;
                        case 'g': userAffected.IsGlobalNotice = change; changes[5] = true; break;
                        case 'w': userAffected.IsWallOps = change; changes[6] = true; break;
                        case 'h':
                            changes[7] = true;
                            if (change)
                            {
                                if (spaceSplit.Count() > 4)
                                {
                                    string[] fake = spaceSplit[4].Split('@');
                                    if (fake.Length > 1)
                                    {
                                        userAffected.FakeIdent = fake[0];
                                        userAffected.FakeHost = fake[1];
                                    }
                                    else
                                    {
                                        userAffected.FakeHost = spaceSplit[4];
                                    }
                                }
                            }
                            else
                            {
                                userAffected.FakeHost = "";
                                userAffected.FakeIdent = "";
                            }
                            break;
                    }
                }

                foreach (var item in Service.Plugins)
                {
                    if (item != userAffected.Plugin)
                    {
                        if (changes[0] == true) { item.OnUserChangeOper(userAffected); }
                        if (changes[1] == true) { item.OnUserChangeService(userAffected); }
                        if (changes[2] == true) { item.OnUserChangeDeaf(userAffected); }
                        if (changes[3] == true) { item.OnUserChangeInvisible(userAffected); }
                        if (changes[4] == true) { item.OnUserChangeServerNotice(userAffected); }
                        if (changes[5] == true) { item.OnUserChangeGlobalNotice(userAffected); }
                        if (changes[6] == true) { item.OnUserChangeWallOps(userAffected); }
                        if (changes[7] == true) { item.OnUserChangeFakeHost(userAffected); }
                    }
                }
            }
            else
            {
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

                int crIndex = 4;
                User userAffected;

                for (int i = 0; i < spaceSplit[3].Length; i++)
                {
                    switch (spaceSplit[3][i])
                    {
                        case '+': change = true; break;
                        case '-': change = false; break;
                        case 'n': 
                        case 't': 
                        case 'i': 
                        case 'r':
                        case 'p':
                        case 's':
                        case 'm':
                        case 'O':
                        case 'c':
                            foreach (var item in channelList)
                            {
                                ChannelModes mode = ChannelModes.n;
                                switch (spaceSplit[3][i])
                                {
                                    case 'n': mode = ChannelModes.n; break;
                                    case 't': mode = ChannelModes.t; break;
                                    case 'i': mode = ChannelModes.i; break;
                                    case 'r': mode = ChannelModes.r; break;
                                    case 'p': mode = ChannelModes.p; break;
                                    case 's': mode = ChannelModes.s; break;
                                    case 'm': mode = ChannelModes.m; break;
                                    case 'o': mode = ChannelModes.O; break;
                                    case 'c': mode = ChannelModes.c; break;
                                }
                                (item as Channel).SetMode(mode, change);
                            }
                            foreach (var item in Service.Plugins)
                            {
                                if (from != null)
                                {
                                    if (item != from.Plugin) 
                                    { 
                                        item.OnChannelModeChange(
                                            spaceSplit[2], 
                                            spaceSplit[3][i], 
                                            change, 
                                            from, 
                                            OpMode
                                        ); 
                                    }
                                }
                                else
                                {
                                    if (item != serverFrom.Plugin)
                                    { 
                                        item.OnChannelModeChange(
                                            spaceSplit[2], 
                                            spaceSplit[3][i], 
                                            change, 
                                            serverFrom, 
                                            OpMode
                                        ); 
                                    }
                                }
                            }
                            break;
                        case 'l':
                            int limit = 0;
                            if (change == true)
                            {
                                if (spaceSplit.Count() > crIndex)
                                {
                                    limit = Convert.ToInt32(spaceSplit[crIndex]);
                                    crIndex += 1;
                                    foreach (Channel item in channelList)
                                    {
                                        item.SetMode(ChannelModes.l, limit);
                                    }
                                }
                            }
                            else
                            {
                                foreach (Channel item in channelList)
                                {
                                    item.SetMode(ChannelModes.l, 0);
                                }
                            }
                            foreach (var item in Service.Plugins)
                            {
                                if (from != null)
                                {
                                    if (item != from.Plugin)
                                    {
                                        item.OnChannelLimit(
                                            spaceSplit[2], 
                                            limit, 
                                            from, 
                                            OpMode
                                        );
                                    }
                                }
                                else
                                {
                                    if (item != serverFrom.Plugin)
                                    {
                                        item.OnChannelLimit(
                                            spaceSplit[2], 
                                            limit, 
                                            serverFrom, 
                                            OpMode
                                        );
                                    }
                                }
                            }
                            break;
                        case 'k':
                            string key = "";
                            if (change == true)
                            {
                                if (spaceSplit.Count() > crIndex)
                                {
                                    key = spaceSplit[crIndex];
                                    crIndex += 1;
                                    foreach (Channel item in channelList)
                                    {
                                        item.SetMode(ChannelModes.k, key);
                                    }
                                }
                            }
                            else
                            {
                                foreach (Channel item in channelList)
                                {
                                    item.SetMode(ChannelModes.k, "");
                                }
                            }
                            foreach (var item in Service.Plugins)
                            {
                                if (from != null)
                                {
                                    if (item != from.Plugin)
                                    {
                                        item.OnChannelKey(
                                            spaceSplit[2],
                                            key, 
                                            from,
                                            OpMode
                                        );
                                    }
                                }
                                else
                                {
                                    if (item != serverFrom.Plugin)
                                    {
                                        item.OnChannelKey(
                                            spaceSplit[2],
                                            key,
                                            serverFrom,
                                            OpMode
                                        );
                                    }
                                }
                            }
                            break;
                        case 'b':
                            if (spaceSplit.Count() > crIndex)
                            {
                                string ban = spaceSplit[crIndex];
                                crIndex += 1;
                                Ban newBan = new Ban(ban);
                                foreach (Channel item in channelList)
                                {
                                    if (change == true)
                                    {
                                        item.AddBan(newBan);
                                    }
                                    else
                                    {
                                        item.RemoveBans(newBan);
                                    }
                                }
                                foreach (var item in Service.Plugins)
                                {
                                    if (from != null)
                                    {
                                        if (item != from.Plugin)
                                        {
                                            if (change == true)
                                            { 
                                                item.OnChannelAddBan(
                                                    spaceSplit[2], 
                                                    newBan,
                                                    from,
                                                    OpMode
                                                ); 
                                            }
                                            else
                                            { 
                                                item.OnChannelRemoveBan(
                                                    spaceSplit[2],
                                                    newBan,
                                                    from,
                                                    OpMode
                                                ); 
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (item != serverFrom.Plugin)
                                        {
                                            if (change == true) 
                                            { 
                                                item.OnChannelAddBan(
                                                    spaceSplit[2],
                                                    newBan, 
                                                    serverFrom,
                                                    OpMode
                                                ); 
                                            }
                                            else 
                                            { 
                                                item.OnChannelRemoveBan(
                                                    spaceSplit[2], 
                                                    newBan, 
                                                    serverFrom,
                                                    OpMode
                                                ); 
                                            }
                                        }
                                    }
                                }
                            }
                            break;
                        case 'o':
                        case 'v':
                        case 'h':
                            userAffected = 
                                Service.GetUser(spaceSplit[crIndex]) as User;
                            if (userAffected == null)
                            {
                                Service.AddLog("Mode change for unknown user " + 
                                    spaceSplit[crIndex]);
                                continue;
                            }
                            crIndex++;
                            IChannel userChannel = 
                                userAffected.Server.GetChannel(spaceSplit[2]);
                            if (userChannel == null)
                            {
                                Service.AddLog("Channel " + spaceSplit[2] +
                                    " does not exist on " + 
                                    userAffected.Server.Name);
                                continue;
                            }
                            ChannelEntry userEntry = 
                                userChannel.GetEntry(userAffected);
                            if (userEntry == null)
                            {
                                Service.AddLog("User " + userAffected.Nick +
                                    " is not on " + spaceSplit[2]);
                                continue;
                            }
                            switch (spaceSplit[3][i])
                            {
                                case 'o':
                                    userEntry.Op = change;
                                    foreach (var item in Service.Plugins)
                                    {
                                        if (from != null)
                                        {
                                            if (item != from.Plugin)
                                            {
                                                if (change == true)
                                                {
                                                    item.OnChannelOp(
                                                        spaceSplit[2], 
                                                        userAffected, 
                                                        from, 
                                                        OpMode
                                                    ); 
                                                }
                                                else
                                                {
                                                    item.OnChannelDeOp(
                                                        spaceSplit[2],
                                                        userAffected,
                                                        from, 
                                                        OpMode
                                                    ); 
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (item != serverFrom.Plugin)
                                            {
                                                if (change == true)
                                                {
                                                    item.OnChannelOp(
                                                        spaceSplit[2], 
                                                        userAffected, 
                                                        serverFrom,
                                                        OpMode
                                                    ); 
                                                }
                                                else { 
                                                    item.OnChannelDeOp(
                                                        spaceSplit[2],
                                                        userAffected, 
                                                        serverFrom,
                                                        OpMode
                                                    ); 
                                                }
                                            }
                                        }
                                    }
                                    break;
                                case 'v':
                                    userEntry.Voice = change;
                                    foreach (var item in Service.Plugins)
                                    {
                                        if (from != null)
                                        {
                                            if (item != from.Plugin)
                                            {
                                                if (change == true) 
                                                { 
                                                    item.OnChannelVoice(
                                                        spaceSplit[2],
                                                        userAffected, 
                                                        from, 
                                                        OpMode
                                                    );
                                                }
                                                else 
                                                {
                                                    item.OnChannelDeVoice(
                                                        spaceSplit[2], 
                                                        userAffected,
                                                        from, 
                                                        OpMode
                                                    ); 
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (item != serverFrom.Plugin)
                                            {
                                                if (change == true)
                                                {
                                                    item.OnChannelVoice(
                                                        spaceSplit[2],
                                                        userAffected, 
                                                        serverFrom,
                                                        OpMode
                                                    ); 
                                                }
                                                else
                                                {
                                                    item.OnChannelDeVoice(
                                                        spaceSplit[2],
                                                        userAffected, 
                                                        serverFrom,
                                                        OpMode
                                                    ); 
                                                }
                                            }
                                        }
                                    }
                                    break;
                                case 'h':
                                    userEntry.HalfOp = change;
                                    foreach (var item in Service.Plugins)
                                    {
                                        if (from != null)
                                        {
                                            if (item != from.Plugin)
                                            {
                                                if (change == true)
                                                {
                                                    item.OnChannelHalfOp(
                                                        spaceSplit[2],
                                                        userAffected, 
                                                        from,
                                                        OpMode
                                                    ); 
                                                }
                                                else 
                                                {
                                                    item.OnChannelDeHalfOp(
                                                        spaceSplit[2],
                                                        userAffected, 
                                                        from,
                                                        OpMode
                                                    ); 
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (item != serverFrom.Plugin)
                                            {
                                                if (change == true)
                                                {
                                                    item.OnChannelHalfOp(
                                                        spaceSplit[2], 
                                                        userAffected, 
                                                        serverFrom,
                                                        OpMode
                                                    ); 
                                                }
                                                else { 
                                                    item.OnChannelDeHalfOp(
                                                        spaceSplit[2],
                                                        userAffected, 
                                                        serverFrom,
                                                        OpMode
                                                    ); 
                                                }
                                            }
                                        }
                                    }
                                    break;
                            }
                            break;
                    }
                }
            }
        }
    }
}
