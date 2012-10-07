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
using IRCServiceNET.Plugins;
using IRCServiceNET.Actions;
using System.Net;

namespace IRCServiceNET.Entities
{
    /// <summary>
    /// A generic IRC Server
    /// </summary>
    public interface IServer : IHasNumeric
    {
        /// <summary>
        /// Gets the server's name (unique)
        /// </summary>
        string Name { get; }
        /// <summary>
        /// Gets the server's description
        /// </summary>
        string Description { get; }
        /// <summary>
        /// Gets the maximum number of users on the server 
        /// </summary>
        int MaxUsers { get; }
        /// <summary>
        /// Is the server controlled by the plugin?
        /// </summary>
        bool IsControlled { get; }
        /// <summary>
        /// Gets the uplink server
        /// </summary>
        IServer UpLink { get; }
        /// <summary>
        /// Gets the server's depth (= uplink's depth + 1)
        /// </summary>
        int Depth { get; }
        /// <summary>
        /// Gets the server's creation timestamp
        /// </summary>
        UnixTimestamp Created { get; }
        /// <summary>
        /// Gets the server's maximum numeric
        /// </summary>
        int MaxNumeric { get; }
        /// <summary>
        /// Gets the plugin that controls the server
        /// </summary>
        IRCServicePlugin Plugin { get; }
        /// <summary>
        /// Gets the service that this server belongs to
        /// </summary>
        IRCService Service { get; }
        /// <summary>
        /// Counts the users on the server
        /// </summary>
        int UserCount { get; }
        /// <summary>
        /// Is the server empty?
        /// </summary>
        bool IsEmpty { get; }
        /// <summary>
        /// Gets all the users on the server
        /// </summary>
        /// <returns></returns>
        IEnumerable<IUser> Users { get; }
        /// <summary>
        /// Gets the channels and channel entries on the server
        /// </summary>
        IDictionary<IChannel, IEnumerable<ChannelEntry>> ChannelEntries { get; }
        /// <summary>
        /// Gets a ServerAction instance or null if the server is not 
        /// owned by a plugin
        /// </summary>
        ServerAction Action { get; }  
        /// <summary>
        /// Does the server contain the users numeric?
        /// </summary>
        /// <param name="numeric"></param>
        /// <returns></returns>
        bool ContainsNumeric(string numeric);
        /// <summary>
        /// Searches a user by numeric
        /// </summary>
        /// <param name="numeric"></param>
        /// <returns>The user of null if it is not found</returns>
        IUser GetUser(string numeric);
        /// <summary>
        /// Searches the user by nick
        /// </summary>
        /// <param name="nick"></param>
        /// <returns>The user or null if it is not found</returns>
        IUser GetUserByNick(string nick);
        /// <summary>
        /// Checks if a user on the server has a specified nick
        /// </summary>
        /// <param name="nick"></param>
        /// <returns>TRUE if the nick is taken by a user on the server</returns>
        bool NickExists(string nick);
        /// <summary>
        /// Searches a channel by name
        /// </summary>
        /// <param name="name"></param>
        /// <returns>The Channel or null if it is not found</returns>
        IChannel GetChannel(string name);
        /// <summary>
        /// Counts the users with the specified IP address
        /// </summary>
        /// <param name="IP"></param>
        /// <returns></returns>
        int CountUsers(IPAddress IP);
    }
}
