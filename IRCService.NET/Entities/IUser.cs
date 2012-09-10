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
using System.Text;
using System.Net;
using IRCServiceNET.Helpers;
using IRCServiceNET.Plugins;
using IRCServiceNET.Actions;

namespace IRCServiceNET.Entities
{
    /// <summary>
    /// A generic IRC User
    /// </summary>
    public interface IUser : IHasNumeric
    {
        /// <summary>
        /// Gets the user's nick 
        /// </summary>
        string Nick { get; }
        /// <summary>
        /// Gets the user's ident
        /// </summary>
        string Ident { get; }
        /// <summary>
        /// Gets the user's host
        /// </summary>
        string Host { get; }
        /// <summary>
        /// Gets the user's base64 IP
        /// </summary>
        string Base64IP { get; }
        /// <summary>
        /// Gets the user's IP address
        /// </summary>
        IPAddress IP { get; }
        /// <summary>
        /// Gets the server that the user is connected to
        /// </summary>
        IServer Server { get; }
        /// <summary>
        /// Gets the user's name
        /// </summary>
        String Name { get; }
        /// <summary>
        /// Gets the user's login username
        /// </summary>
        String Login { get; }
        /// <summary>
        /// Gets the user's full login string
        /// </summary>
        String LoginHostString { get; }
        /// <summary>
        /// Is the user invisible?
        /// </summary>
        bool IsInvisible { get; }
        /// <summary>
        /// Is the user an IRC operator?
        /// </summary>
        bool IsOper { get; }
        /// <summary>
        /// Is the user a service?
        /// </summary>
        bool IsService { get; }
        /// <summary>
        /// Is the user deaf?
        /// (Does not receive channel messages)
        /// </summary>
        bool IsDeaf { get; }
        /// <summary>
        /// Does the user listen to server notices?
        /// </summary>
        bool IsServerNotice { get; }
        /// <summary>
        /// Does the user listens to global notices?
        /// </summary>
        bool IsGlobalNotice { get; }
        /// <summary>
        /// Does the user listens to wallops?
        /// </summary>
        bool IsWallOps { get; }
        /// <summary>
        /// Gets the user's fake ident
        /// </summary>
        String FakeIdent { get; }
        /// <summary>
        /// Gets the user's fake host
        /// </summary>
        String FakeHost { get; }
        /// <summary>
        /// Gets the user's connection timestamp
        /// </summary>
        UnixTimestamp ConnectionTimestamp { get; }
        /// <summary>
        /// Gets the plugin that controls the user
        /// </summary>
        IRCServicePlugin Plugin { get; }
        /// <summary>
        /// Gets a UserAction instance or null if the user is not owned by
        /// a plugin
        /// </summary>
        UserAction Action { get; }
        /// <summary>
        /// Is the user on a channel?
        /// </summary>
        /// <param name="channel"></param>
        /// <returns></returns>
        bool IsOnChannel(string channel);
        /// <summary>
        /// Get the channel entry for a specific user
        /// </summary>
        /// <param name="channel"></param>
        /// <returns></returns>
        ChannelEntry GetChannelEntry(string channel);
    }
}
