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

namespace IRCServiceNET.Entities
{
    /// <summary>
    /// Contains a User-Channel pair and information 
    /// on what modes the user has on the channel
    /// </summary>
    public class ChannelEntry
    {
        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="user"></param>
        public ChannelEntry(Channel channel, User user)
        {
            Channel = channel;
            User = user;
        }
        /// <summary>
        /// Copy constructor
        /// </summary>
        /// <param name="other"></param>
        public ChannelEntry(ChannelEntry other)
        {
            Channel = other.Channel;
            User = other.User;
            Op = other.Op;
            Voice = other.Voice;
            HalfOp = other.HalfOp;
        }
        /// <summary>
        /// Gets the Channel
        /// </summary>
        public Channel Channel { get; protected set; }
        /// <summary>
        /// Gets the User
        /// </summary>
        public User User { get; protected set; }
        /// <summary>
        /// Gets or sets if the user has Op privileges on the channel
        /// </summary>
        public bool Op { get; set; }
        /// <summary>
        /// Gets or sets if the user has Voice privileges on the channel
        /// </summary>
        public bool Voice { get; set; }
        /// <summary>
        /// Gets or sets if the user has HalfOp privileges on the channel
        /// </summary>
        public bool HalfOp { get; set; }
    }
}
