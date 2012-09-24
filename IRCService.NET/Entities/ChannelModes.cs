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
    /// Channel Modes
    /// </summary>
    public enum ChannelModes
    {
        /// <summary>
        /// No external messages
        /// </summary>
        n = 1,
        /// <summary>
        /// Only operators can change the topic
        /// </summary>
        t = 2,
        /// <summary>
        /// Secret
        /// </summary>
        s = 4,
        /// <summary>
        /// Invite only
        /// </summary>
        i = 8,
        /// <summary>
        /// Moderated
        /// </summary>
        m = 16,
        /// <summary>
        /// Private
        /// </summary>
        p = 32,
        /// <summary>
        /// Key
        /// </summary>
        k = 64,
        /// <summary>
        /// User limit
        /// </summary>
        l = 128,
        /// <summary>
        /// Only registered users can join the channel
        /// </summary>
        r = 256,
        /// <summary>
        /// Only irc operators can join the channel
        /// </summary>
        O = 512,
        /// <summary>
        /// Colors are not allowed
        /// </summary>
        c = 1024,
        /// <summary>
        /// CTCP is not allowed
        /// </summary>
        C = 2048
    }
}
