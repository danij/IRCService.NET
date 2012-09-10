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
    /// An IRC User
    /// </summary>
    public class User : IUser
    {
        /// <summary>
        /// The channels the user is on
        /// </summary>
        private Dictionary<string, ChannelEntry> channels;
        /// <summary>
        /// The UserAction instance of the user
        /// </summary>
        private UserAction action;
        /// <summary>
        /// The user's ip address 
        /// </summary>
        private IPAddress IPAddress;
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="server"></param>
        /// <param name="numeric"></param>
        /// <param name="nick"></param>
        /// <param name="ident"></param>
        /// <param name="host"></param>
        /// <param name="name"></param>
        /// <param name="connectionTimestamp"></param>
        /// <param name="base64IP"></param>
        public User(IServer server, string numeric, string nick, string ident, 
            string host, string name, UnixTimestamp connectionTimestamp, 
            string base64IP, IRCServicePlugin plugin = null)
        {
            Numeric = numeric;
            Nick = nick;
            Ident = ident;
            Host = host;
            Name = name;
            Base64IP = base64IP;
            Server = server;
            ConnectionTimestamp = connectionTimestamp;
            FakeIdent = "";
            FakeHost = "";
            Login = "";
            Plugin = plugin;
            channels = new Dictionary<string, ChannelEntry>
                (StringComparer.OrdinalIgnoreCase);
            if (Server.Controlled)
            {
                action = new UserAction(this);
            }
        }
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="server"></param>
        /// <param name="numeric"></param>
        /// <param name="nick"></param>
        /// <param name="ident"></param>
        /// <param name="host"></param>
        /// <param name="name"></param>
        /// <param name="connectionTimestamp"></param>
        /// <param name="IPAddress"></param>
        public User(IServer server, string numeric, string nick, string ident, 
            string host, string name, UnixTimestamp connectionTimestamp, 
            IPAddress IPAddress, IRCServicePlugin plugin = null)
            : this(server, numeric, nick, ident, host, name, 
            connectionTimestamp, "", plugin)
        {            
            if (IPAddress.AddressFamily == 
                System.Net.Sockets.AddressFamily.InterNetworkV6)
            {
                throw new NotSupportedException("IPv6 is not yet supported");
            }
            this.IPAddress = IPAddress;
            var split = IPAddress.ToString().Split('.');
            Int64 integerIP = 0;
            integerIP |= (Convert.ToInt64(split[0]) << 24);
            integerIP |= (Convert.ToInt64(split[1]) << 16);
            integerIP |= (Convert.ToInt64(split[2]) << 8);
            integerIP |= (Convert.ToInt64(split[3]));
            Base64IP = Base64Converter.IntToNumeric(integerIP, 6);
        }

#region Properties
        /// <summary>
        /// Gets the user's numeric
        /// </summary>
        public string Numeric { get; protected set; }
        /// <summary>
        /// Gets or sets the user's nick 
        /// </summary>
        public string Nick { get; set; }
        /// <summary>
        /// Gets the user's ident
        /// </summary>
        public string Ident { get; protected set; }
        /// <summary>
        /// Gets the user's host
        /// </summary>
        public string Host { get; protected set; }
        /// <summary>
        /// Gets the user's base64 IP
        /// </summary>
        public string Base64IP { get; protected set; }
        /// <summary>
        /// Gets the user's IP address
        /// </summary>
        public IPAddress IP
        {
            get
            {
                if (IPAddress == null)
                {
                    int base64IP = Base64Converter.NumericToInt(Base64IP);
                    byte[] ipBytes = BitConverter.GetBytes(base64IP);
                    IPAddress =  IPAddress.Parse(
                        ipBytes[3] + "." + ipBytes[2] + "." + 
                        ipBytes[1] + "." + ipBytes[0]
                    );
                }
                return IPAddress;
            }
        }
        /// <summary>
        /// Gets the server that the user is connected to
        /// </summary>
        public IServer Server { get; protected set; }
        /// <summary>
        /// Gets the user's name
        /// </summary>
        public String Name { get; protected set; }
        /// <summary>
        /// Gets the user's login username
        /// </summary>
        public String Login { get; set; }
        /// <summary>
        /// Gets the user's full login string
        /// </summary>
        public String LoginHostString
        {
            get
            {
                return Login + Server.Service.LoginHost;
            }
        }
        /// <summary>
        /// Gets or sets if the user if invisible
        /// </summary>
        public bool IsInvisible { get; set; }
        /// <summary>
        /// Gets or sets if the user is an IRC operator
        /// </summary>
        public bool IsOper { get; set; }
        /// <summary>
        /// Gets or sets if the user is a service
        /// </summary>
        public bool IsService { get; set; }
        /// <summary>
        /// Gets or sets if the user is deaf?
        /// (Does not receive channel messages)
        /// </summary>
        public bool IsDeaf { get; set; }
        /// <summary>
        /// Gets or sets if the user listens to server notices
        /// </summary>
        public bool IsServerNotice { get; set; }
        /// <summary>
        /// Gets or sets if the user listens to global notices
        /// </summary>
        public bool IsGlobalNotice { get; set; }
        /// <summary>
        /// Gets or sets if the user listens to wallops
        /// </summary>
        public bool IsWallOps { get; set; }
        /// <summary>
        /// Gets or sets the user's fake ident
        /// </summary>
        public String FakeIdent { get; set; }
        /// <summary>
        /// Gets or sets the user's fake host
        /// </summary>
        public String FakeHost { get; set; }
        /// <summary>
        /// Gets or sets the user's connection timestamp
        /// </summary>
        public UnixTimestamp ConnectionTimestamp { get; protected set; }
        /// <summary>
        /// Gets the plugin that controls the user
        /// </summary>
        public IRCServicePlugin Plugin { get; protected set; }
        /// <summary>
        /// Gets a UserAction instance or null if the user is not owned by
        /// a plugin
        /// </summary>
        public UserAction Action
        {
            get
            {
                if (Server.Controlled && Plugin != null)
                {
                    return action;
                }
                return null;
            }
        }
#endregion

#region Methods
        /// <summary>
        /// For internal use only
        /// </summary>
        public bool OnAddToChannel(ChannelEntry channel)
        {
            if (channels.ContainsKey(channel.Channel.Name))
            {
                return false;
            }
            channels.Add(channel.Channel.Name, channel);
            return true;
        }
        /// <summary>
        /// For internal use only
        /// </summary>
        public bool OnRemoveFromChannel(IChannel channel)
        {
            return channels.Remove(channel.Name);
        }
        /// <summary>
        /// Is the user on a channel?
        /// </summary>
        /// <param name="channel"></param>
        /// <returns></returns>
        public bool IsOnChannel(string channel)
        {
            return channels.ContainsKey(channel);
        }
        /// <summary>
        /// Get the channel entry for a specific user
        /// </summary>
        /// <param name="channel"></param>
        /// <returns></returns>
        public ChannelEntry GetChannelEntry(string channel)
        {
            if (!channels.ContainsKey(channel))
            {
                return null;
            }
            return new ChannelEntry(channels[channel]);
        }        
#endregion
    }
}
