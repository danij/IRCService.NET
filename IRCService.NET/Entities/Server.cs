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

namespace IRCServiceNET.Entities
{
    /// <summary>
    /// An IRC Server
    /// </summary>
    public class Server : IHasNumeric
    {
        /// <summary>
        /// Plugin that controlles the server
        /// </summary>
        private IRCServicePlugin plugin;
        /// <summary>
        /// Users on the server
        /// </summary>
        private Dictionary<string, User> users;
        /// <summary>
        /// Channels on the server
        /// </summary>
        private Dictionary<string, Channel> channels;
        /// <summary>
        /// The ServerAction instance on the user
        /// </summary>
        private ServerAction action;
        /// <summary>
        /// Maximum number of users on a server
        /// </summary>
        public const int MaxCapacity = 262143;
        /// <summary>
        /// Constructs a new Server
        /// </summary>
        /// <param name="service">The service that the server belongs to</param>
        /// <param name="numeric">The servers numeric</param>
        /// <param name="name">The servers name</param>
        /// <param name="description">The servers description</param>
        /// <param name="created">The servers creation timestamp</param>
        /// <param name="maxUsers">The maximum ammount of users on the server (1-262143)</param>
        /// <param name="controlled">Is the server controlled by the service?</param>
        /// <param name="upLink">Tbe servers uplink</param>
        public Server(IRCService service, string numeric, string name, 
            string description, UnixTimestamp created, int maxUsers,
            bool controlled, Server upLink)
        {
            Service = service;
            Numeric = numeric;
            Name = name;
            Description = description;
            Created = created;
            MaxUsers = maxUsers;
            if (MaxUsers < 0)
            {
                MaxUsers = 1;
            }
            if (MaxUsers > 262143)
            {
                MaxUsers = 262143;
            }
            UpLink = upLink;
            Controlled = controlled;
            users = new Dictionary<string, User>();
            channels = 
                new Dictionary<string, Channel>(StringComparer.OrdinalIgnoreCase);
        }

#region Properties
        /// <summary>
        /// Gets the server's numeric (unique)
        /// </summary>
        public string Numeric { get; protected set; }
        /// <summary>
        /// Gets the server's name (unique)
        /// </summary>
        public string Name { get; protected set; }
        /// <summary>
        /// Gets the server's description
        /// </summary>
        public string Description { get; protected set; }
        /// <summary>
        /// Gets the maximum number of users on the server 
        /// </summary>
        public int MaxUsers { get; protected set; }
        /// <summary>
        /// Is the server controlled by the plugin?
        /// </summary>
        public bool Controlled { get; protected set; }
        /// <summary>
        /// Gets the uplink server
        /// </summary>
        public Server UpLink { get; protected set; }
        /// <summary>
        /// Gets the server's depth (= uplink's depth + 1)
        /// </summary>
        public int Depth
        {
            get
            {
                return UpLink == null ? 0 : UpLink.Depth;
            }
        }
        /// <summary>
        /// Gets the server's creation timestamp
        /// </summary>
        public UnixTimestamp Created { get; protected set; }
        /// <summary>
        /// Gets the server's maximum numeric
        /// </summary>
        public int MaxNumeric { get; protected set; }
        /// <summary>
        /// Gets the plugin that controls the server
        /// </summary>
        public IRCServicePlugin Plugin
        {
            get { return plugin; }
            set
            {
                plugin = Controlled ? value : null;
                if (Controlled)
                {
                    action = new ServerAction(this);
                }
            }
        }
        /// <summary>
        /// Gets the service that this server belongs to
        /// </summary>
        public IRCService Service { get; protected set; }
        /// <summary>
        /// Counts the users on the server
        /// </summary>
        public int UserCount
        {
            get
            {
                return users.Count();
            }
        }
        /// <summary>
        /// Is the server empty?
        /// </summary>
        public bool IsEmpty
        {
            get
            {
                return UserCount == 0;
            }
        }
        /// <summary>
        /// Gets all the users on the server
        /// </summary>
        /// <returns></returns>
        public IEnumerable<User> Users
        {
            get
            {
                return users.Values;
            }
        }
        /// <summary>
        /// Gets all the channels on the server
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Channel> Channels
        {
            get
            {
                return channels.Values;
            }
        }
        /// <summary>
        /// Gets a ServerAction instance or null if the server is not 
        /// owned by a plugin
        /// </summary>
        public ServerAction Action
        {
            get
            {
                if (Controlled && Plugin != null)
                {
                    return action;
                }
                return null;
            }
        }
#endregion

#region Methods
        /// <summary>
        /// Adds a new user to the server
        /// </summary>
        /// <param name="user"></param>
        /// <returns>TRUE if the user is successfully added</returns>
        public bool AddUser(User user)
        {
            if (user.Numeric.Length < 3)
            {
                return false;
            }
            if (users.Keys.Contains(user.Numeric))
            {
                return false;
            }
            users.Add(user.Numeric, user);
            int userNumeric =
                Base64Converter.NumericToInt(user.Numeric.Substring(2, 3));
            if (userNumeric > MaxNumeric)
            {
                MaxNumeric = userNumeric;
            }
            return true;
        }
        /// <summary>
        /// Removes a user from the server
        /// </summary>
        /// <param name="user"></param>
        /// <returns>TRUE if the user is succesfully removed</returns>
        public bool RemoveUser(User user)
        {
            if ( ! users.Remove(user.Numeric))
            {
                return false;
            }            
            var userChannels = channels.Values.ToArray();
            foreach (var item in userChannels)
            {
                item.RemoveUser(user, true);
                if (item.UserCount < 1)
                {
                    RemoveChannel(item);
                }
            }            
            return true;
        }
        /// <summary>
        /// Does the server contain the users numeric?
        /// </summary>
        /// <param name="numeric"></param>
        /// <returns></returns>
        public bool ContainsNumeric(string numeric)
        {
            return users.Keys.Contains(numeric);
        }
        /// <summary>
        /// Searches a user by numeric
        /// </summary>
        /// <param name="numeric"></param>
        /// <returns>The user of null if it is not found</returns>
        public User GetUser(string numeric)
        {
            if (users.Keys.Contains(numeric))
            {
                return users[numeric];
            }
            else
            {
                return null;
            }
        }
        /// <summary>
        /// Searches the user by nick
        /// </summary>
        /// <param name="nick"></param>
        /// <returns>The user or null if it is not found</returns>
        public User GetUserByNick(string nick)
        {
            try
            {
                User result = null;
                string lowerNick = nick.ToLower();
                foreach (var item in users.Values)
                {
                    if (item.Nick.ToLower() == lowerNick)
                    {
                        result = item;
                        break;
                    }
                }
                return result;
            }
            catch (Exception)
            {                

            }
            return null;
        }
        /// <summary>
        /// Checks if a user on the server has a specified nick
        /// </summary>
        /// <param name="nick"></param>
        /// <returns>TRUE if the nick is taken by a user on the server</returns>
        public bool NickExists(string nick)
        {
            return GetUserByNick(nick) != null;
        }
        /// <summary>
        /// Creates a new channel on the server
        /// </summary>
        /// <param name="name"></param>
        /// <param name="creationTimeStamp"></param>
        /// <returns></returns>
        public Channel CreateChannel(string name, 
            UnixTimestamp creationTimeStamp = null)
        {
            if (creationTimeStamp == null)
            {
                creationTimeStamp = UnixTimestamp.CurrentTimestamp();
            }
            return new Channel(this, name, creationTimeStamp);
        }
        /// <summary>
        /// Adds a new channel to the server
        /// </summary>
        /// <param name="channel"></param>
        /// <returns>TRUE if the channel is added succesfully</returns>
        public bool AddChannel(Channel channel)
        {
            if (channels.Keys.Contains(channel.Name))
            {
                return false;
            }
            channels.Add(channel.Name, channel);
            return true;
        }
        /// <summary>
        /// Removes a channel
        /// </summary>
        /// <param name="channel"></param>
        /// <returns>TRUE if the Channel is successfully removed</returns>
        public bool RemoveChannel(Channel channel)
        {
            return channels.Remove(channel.Name);
        }
        /// <summary>
        /// Searches a channel by name
        /// </summary>
        /// <param name="name"></param>
        /// <returns>The Channel or null if it is not found</returns>
        public Channel GetChannel(string name)
        {
            if (channels.Keys.Contains(name))
            {
                return channels[name];
            }
            return null;
        }
#endregion

    }
}
