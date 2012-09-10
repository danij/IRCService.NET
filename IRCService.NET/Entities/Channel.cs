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
        /// Channel limit
        /// </summary>
        private int limit;
        /// <summary>
        /// Users on the channel
        /// </summary>
        private List<ChannelEntry> users;
        /// <summary>
        /// Bans on the channel
        /// </summary>
        private List<Ban> bans;
        /// <summary>
        /// Channel creation timestamp
        /// </summary>
        private UnixTimestamp creationTimestamp;
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="server"></param>
        /// <param name="name"></param>
        /// <param name="creationTimeStamp"></param>
        public Channel(IServer server, string name, 
            UnixTimestamp creationTimeStamp)
        {
            Server = server;
            Name = name;
            Key = "";
            modes = 0;
            limit = 0;
            creationTimestamp = creationTimeStamp;
            users = new List<ChannelEntry>();
            bans = new List<Ban>();
        }

#region Properties
        /// <summary>
        /// Gets the server that owns the channel
        /// </summary>
        public IServer Server { get; protected set; }
        /// <summary>
        /// Gets or sets the channel's creation timestamp
        /// </summary>
        public UnixTimestamp CreationTimeStamp
        {
            get { return creationTimestamp; }
            set
            {
                if (value.Timestamp < creationTimestamp.Timestamp)
                {
                    for (int i = 0; i < users.Count(); i++)
                    {
                        users[i].Op = false;
                        users[i].HalfOp = false;
                        users[i].Voice = false;
                    }
                    ClearModes();
                }
                creationTimestamp = value;
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
        /// Counts the users in the channel
        /// </summary>
        public int UserCount
        {
            get { return users.Count; }
        }
        /// <summary>
        /// Is the channel empty?
        /// </summary>
        public bool IsEmpty
        {
            get { return UserCount == 0; }
        }
        /// <summary>
        /// Gets all channel entries
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ChannelEntry> Entries
        {
            get
            {
                return users;
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
                return bans;
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
            return (modes >> MathHelper.Log((int)mode) & 1) == 1;
        }
        /// <summary>
        /// Sets a boolean channel mode
        /// </summary>
        /// <param name="mode"></param>
        /// <param name="value"></param>
        public void SetMode(ChannelModes mode, bool value)
        {
            if (value == true)
            {
                modes = modes | (int)mode;
            }
            else
            {
                if ((modes >> MathHelper.Log((int)mode) & 1) == 1)
                {
                    modes = modes ^ (int)mode;
                }
            }
        }
        /// <summary>
        /// Sets the integer value of all the channel modes
        /// </summary>
        /// <param name="mode"></param>
        public void SetMode(int mode)
        {
            modes = mode;
        }
        /// <summary>
        /// Sets a channel mode with a string parameter
        /// </summary>
        /// <param name="mode"></param>
        /// <param name="parameter"></param>
        public void SetMode(ChannelModes mode, string parameter)
        {
            modes = modes | (int)mode;
            switch (mode)
            {
                case ChannelModes.k:
                    Key = parameter;
                    break;
            }
        }
        /// <summary>
        /// Sets a channel mode with an integer parameter
        /// </summary>
        /// <param name="mode"></param>
        /// <param name="parameter"></param>
        public void SetMode(ChannelModes mode, int parameter)
        {
            modes = modes | (int)mode;
            switch (mode)
            {
                case ChannelModes.l:
                    limit = parameter;
                    break;
            }
        }
        /// <summary>
        /// Clears all channel modes
        /// </summary>
        public void ClearModes()
        {
            modes = 0;
            Key = "";
            limit = 0;
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
            ChannelEntry entry = new ChannelEntry(this, user);
            entry.Op = op;
            entry.Voice = voice;
            entry.HalfOp = halfop;
            foreach (ChannelEntry item in users)
            {
                if (item.User == user)
                {
                    return false;
                }
            }
            users.Add(entry);
            (user as User).OnAddToChannel(entry);
            return true;
        }
        /// <summary>
        /// Removes a user from the channel
        /// </summary>
        /// <param name="user">The user to remove</param>
        /// <param name="removeChannel">Removes the channel if it is empty</param>
        /// <returns>TRUE if the user is successfully removed</returns>
        public bool RemoveUser(IUser user, bool removeChannel = true)
        {
            for (int i = 0; i < users.Count; i++)
            {
                if (users[i].User == user)
                {                    
                    (users[i].User as User).OnRemoveFromChannel(this);
                    users.RemoveAt(i);
                    if (users.Count() == 0 && removeChannel)
                    {
                        (Server as Server).RemoveChannel(this);
                    }
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// Searches for an entry that contains the requested user
        /// </summary>
        /// <param name="user"></param>
        /// <returns>The entry or null if it is not found</returns>
        public ChannelEntry GetEntry(IUser user)
        {
            foreach (ChannelEntry entry in users)
            {
                if (entry.User == user)
                {
                    return entry;
                }
            }
            return null;
        }
        /// <summary>
        /// Clears all the users, bans and channel modes
        /// </summary>
        public void Clear()
        {
            users.Clear();
            ClearBans();
            ClearModes();
        }        
        /// <summary>
        /// Adds a ban to the channel
        /// </summary>
        /// <param name="ban"></param>
        /// <returns></returns>
        public bool AddBan(Ban ban)
        {
            bans.Add(ban);
            return true;
        }
        /// <summary>
        /// Adds a range of bans to the channel
        /// </summary>
        /// <param name="ban"></param>
        /// <returns></returns>
        public bool AddBan(IEnumerable<Ban> ban)
        {
            bans.AddRange(ban);
            return true;
        }
        /// <summary>
        /// Removes all bans from the channel that match a specified ban
        /// </summary>
        /// <param name="ban"></param>
        /// <returns></returns>
        public int RemoveBans(Ban ban)
        {
            List<Ban> toRemove = new List<Ban>();
            foreach (Ban toremove in bans)
            {
                if (ban.Match(toremove))
                {
                    toRemove.Add(toremove);
                }
            }
            foreach (Ban item in toRemove)
            {
                bans.Remove(item);
            }
            return toRemove.Count;
        }
        /// <summary>
        /// Clears all the bans from the channel
        /// </summary>
        public void ClearBans()
        {
            bans.Clear();
        }
        /// <summary>
        /// Removes Op from all the users on the channel
        /// </summary>
        public void ClearOp()
        {
            users.ForEach(e => e.Op = false);
        }
        /// <summary>
        /// Removes Voice from all the users on the channel
        /// </summary>
        public void ClearVoice()
        {
            users.ForEach(e => e.Voice = false);
        }
        /// <summary>
        /// Removes HalfOp from all the users on the channel
        /// </summary>
        public void ClearHalfOp()
        {
            users.ForEach(e => e.HalfOp = false);
        }
#endregion

    }
}
