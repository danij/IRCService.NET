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
    /// A generic IRC Channel that contains the users from one server
    /// </summary>
    public interface IChannel
    {        
        /// <summary>
        /// Gets the server that owns the channel
        /// </summary>
        IServer Server { get; }
        /// <summary>
        /// Gets or sets the channel's creation timestamp
        /// </summary>
        UnixTimestamp CreationTimeStamp { get; }        
        /// <summary>
        /// Gets the channel's name
        /// </summary>
        string Name { get; }
        /// <summary>
        /// Gets the channel's access key
        /// </summary>
        string Key { get; }
        /// <summary>
        /// Gets the channel's limit
        /// </summary>
        int Limit { get; }
        /// <summary>
        /// Counts the users in the channel
        /// </summary>
        int UserCount { get; }
        /// <summary>
        /// Is the channel empty?
        /// </summary>
        bool IsEmpty { get; }
        /// <summary>
        /// Gets all channel entries
        /// </summary>
        /// <returns></returns>
        IEnumerable<ChannelEntry> Entries { get; }
        /// <summary>
        /// Gets a channel mode
        /// </summary>
        /// <param name="mode"></param>
        /// <returns></returns>
        bool GetMode(ChannelModes mode);
        /// <summary>
        /// Searches for an entry that contains the requested user
        /// </summary>
        /// <param name="user"></param>
        /// <returns>The entry or null if it is not found</returns>
        ChannelEntry GetEntry(IUser user);
        /// <summary>
        /// Returns all the bans from the channel
        /// </summary>
        /// <returns></returns>
        IEnumerable<Ban> Bans { get; }
    }
}
