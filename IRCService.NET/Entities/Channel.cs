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
using System.Diagnostics;

namespace IRCServiceNET.Entities
{
    /// <summary>
    /// An IRC Channel that contains the users from one server
    /// </summary>
    public class Channel : IChannel
    {
        /// <summary>
        /// Channel modes
        /// </summary>
        private int modes;
        /// <summary>
        /// Bans on the channel
        /// </summary>
        private List<Ban> bans;
        /// <summary>
        /// Channel creation timestamp
        /// </summary>
        private UnixTimestamp creationTimestamp;
        /// <summary>
        /// Object for thread synchronization
        /// </summary>
        private object lockObject = new object();
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="name"></param>
        /// <param name="creationTimeStamp"></param>
        public Channel(IRCService service, string name, 
            UnixTimestamp creationTimeStamp = null)
        {
            Name = name;
            Key = "";
            modes = 0;
            Limit = 0;
            Service = service;
            if (creationTimestamp == null)
            {
                creationTimestamp = UnixTimestamp.CurrentTimestamp();
            }
            creationTimestamp = creationTimeStamp;
            bans = new List<Ban>();
        }

#region Properties
        /// <summary>
        /// Gets or sets the service that owns the channel
        /// </summary>
        public IRCService Service { get; protected set; }
        /// <summary>
        /// Gets or sets the channel's creation timestamp
        /// </summary>
        public UnixTimestamp CreationTimeStamp
        {
            get { return creationTimestamp; }
            set
            {
                lock (lockObject)
                {
                    if (value.Timestamp < creationTimestamp.Timestamp)
                    {
                        foreach (var item in Service.Servers)
                        {
                            if (item.ChannelEntries.ContainsKey(this))
                            {
                                foreach (var entry in item.ChannelEntries[this])
                                {
                                    entry.Op = false;
                                    entry.HalfOp = false;
                                    entry.Voice = false;
                                }
                            }
                        }
                        ClearModes();
                    }
                    creationTimestamp = value;
                }
            }
        }
        /// <summary>
        /// Gets the channel's name
        /// </summary>
        public string Name { get; protected set; }
        /// <summary>
        /// Gets the channel's access key
        /// </summary>
        public string Key { get; protected set; }
        /// <summary>
        /// Gets the channel's limit
        /// </summary>
        public int Limit { get; protected set; }
        /// <summary>
        /// Counts the users in the channel
        /// </summary>
        public int UserCount
        {
            get 
            {
                lock (lockObject)
                {
                    return Entries.Count();
                }
            }
        }
        /// <summary>
        /// Is the channel empty?
        /// </summary>
        public bool IsEmpty
        {
            get 
            {
                lock (lockObject)
                {
                    return UserCount == 0;
                }
            }
        }
        /// <summary>
        /// Gets all channel entries
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ChannelEntry> Entries
        {
            get
            {
                lock (lockObject)
                {
                    var list = new List<ChannelEntry>();
                    foreach (var item in Service.Servers)
                    {
                        if (item.ChannelEntries.ContainsKey(this))
                        {
                            list.AddRange(item.ChannelEntries[this]);
                        }
                    }
                    return list;
                }
            }
        }
        /// <summary>
        /// Gets all the bans from the channel
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Ban> Bans
        {
            get
            {
                lock (lockObject)
                {
                    return bans.ToArray();
                }
            }            
        }
#endregion

#region Methods
        /// <summary>
        /// Gets a channel mode
        /// </summary>
        /// <param name="mode"></param>
        /// <returns></returns>
        public bool GetMode(ChannelModes mode)
        {
            lock (lockObject)
            {
                return (modes & (int)mode) > 0;
            }
        }
        /// <summary>
        /// Sets a boolean channel mode
        /// </summary>
        /// <param name="mode"></param>
        /// <param name="value"></param>
        public void SetMode(ChannelModes mode, bool value)
        {
            lock (lockObject)
            {
                if (value == true)
                {
                    modes = modes | (int)mode;
                }
                else
                {
                    modes = modes & (~(int)mode);
                }
            }
        }
        /// <summary>
        /// Sets the integer value of all the channel modes
        /// </summary>
        /// <param name="mode"></param>
        public void SetMode(int mode)
        {
            lock (lockObject)
            {
                modes = mode;
            }
        }
        /// <summary>
        /// Sets a channel mode with a string parameter
        /// </summary>
        /// <param name="mode"></param>
        /// <param name="parameter"></param>
        public void SetMode(ChannelModes mode, string parameter)
        {
            lock (lockObject)
            {
                modes = modes | (int)mode;
                switch (mode)
                {
                    case ChannelModes.k:
                        Key = parameter;
                        if (String.IsNullOrEmpty(Key))
                        {
                            SetMode(mode, false);
                        }
                        break;
                }
            }
        }
        /// <summary>
        /// Sets a channel mode with an integer parameter
        /// </summary>
        /// <param name="mode"></param>
        /// <param name="parameter"></param>
        public void SetMode(ChannelModes mode, int parameter)
        {
            lock (lockObject)
            {
                modes = modes | (int)mode;
                switch (mode)
                {
                    case ChannelModes.l:
                        if (parameter < 1)
                        {
                            parameter = 0;
                            SetMode(mode, false);
                        }
                        Limit = parameter;
                        break;
                }
            }
        }
        /// <summary>
        /// Clears all channel modes
        /// </summary>
        public void ClearModes()
        {
            lock (lockObject)
            {
                modes = 0;
                Key = "";
                Limit = 0;
            }
        }
        /// <summary>
        /// Adds a user to the channel
        /// </summary>
        /// <param name="user"></param>
        /// <param name="op"></param>
        /// <param name="voice"></param>
        /// <param name="halfop"></param>
        /// <returns>TRUE if the user is successfully added</returns>
        public bool AddUser(IUser user, bool op, bool voice, bool halfop)
        {
            lock (lockObject)
            {
                if (user.IsOnChannel(Name))
                {
                    return false;
                }
                ChannelEntry entry = new ChannelEntry(this, user);
                entry.Op = op;
                entry.Voice = voice;
                entry.HalfOp = halfop;

                var list = user.Server.ChannelEntries[this] as List<ChannelEntry>;
                list.Add(entry);
                (user as User).OnAddToChannel(entry);
                return true;
            }
        }
        /// <summary>
        /// Searches for an entry that contains the requested user
        /// </summary>
        /// <param name="user"></param>
        /// <returns>The entry or null if it is not found</returns>
        public ChannelEntry GetEntry(IUser user)
        {
            lock (lockObject)
            {
                foreach (ChannelEntry entry in Entries)
                {
                    if (entry.User == user)
                    {
                        return entry;
                    }
                }
                return null;
            }
        }
        /// <summary>
        /// Clears all the bans and channel modes
        /// </summary>
        public void Clear()
        {
            lock (lockObject)
            {
                ClearBans();
                ClearModes();
            }
        }        
        /// <summary>
        /// Adds a ban to the channel
        /// </summary>
        /// <param name="ban"></param>
        /// <returns></returns>
        public bool AddBan(Ban ban)
        {
            lock (lockObject)
            {
                bans.Add(ban);
                return true;
            }
        }
        /// <summary>
        /// Adds a range of bans to the channel
        /// </summary>
        /// <param name="ban"></param>
        /// <returns></returns>
        public bool AddBan(IEnumerable<Ban> ban)
        {
            lock (lockObject)
            {
                bans.AddRange(ban);
                return true;
            }
        }
        /// <summary>
        /// Removes all bans from the channel that match a specified ban
        /// </summary>
        /// <param name="ban"></param>
        /// <returns></returns>
        public int RemoveBans(Ban ban)
        {
            lock (lockObject)
            {
                int result = 0;
                foreach (Ban item in bans.ToArray())
                {
                    if (ban.Match(item))
                    {
                        bans.Remove(item);
                        result += 1;
                    }
                }
                return result;
            }
        }
        /// <summary>
        /// Clears all the bans from the channel
        /// </summary>
        public void ClearBans()
        {
            lock (lockObject)
            {
                bans.Clear();
            }
        }
        /// <summary>
        /// Removes Op from all the users on the channel
        /// </summary>
        public void ClearOp()
        {
            lock (lockObject)
            {
                foreach (var item in Entries)
                {
                    item.Op = false;
                }
            }
        }
        /// <summary>
        /// Removes Voice from all the users on the channel
        /// </summary>
        public void ClearVoice()
        {
            lock (lockObject)
            {
                foreach (var item in Entries)
                {
                    item.Voice = false;
                }
            }
        }
        /// <summary>
        /// Removes HalfOp from all the users on the channel
        /// </summary>
        public void ClearHalfOp()
        {
            lock (lockObject)
            {
                foreach (var item in Entries)
                {
                    item.HalfOp = false;
                }
            }
        }
#endregion

    }
}
