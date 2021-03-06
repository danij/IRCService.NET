﻿//IRCService.NET. Generic IRC service library for .NET
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
using System.Diagnostics;
using IRCServiceNET.Helpers;
using IRCServiceNET.Entities;
using System.Net;

namespace IRCServiceNET.Plugins
{
    /// <summary>
    /// Defines an abstract base class for a service plugin
    /// </summary>
    public abstract class IRCServicePlugin
    {
        private List<IServer> servers;
        private IServer mainServer;
        private object lockObject = new object();

        /// <summary>
        /// Searches for a free numeric on a specified server
        /// </summary>
        /// <param name="server"></param>
        /// <returns></returns>
        private string FindFreeNumeric(IServer server)
        {
            lock (lockObject)
            {
                if (server.MaxNumeric < 260000)
                {
                    return server.Numeric +
                        Base64Converter.IntToNumeric(server.MaxNumeric + 1, 3);
                }
                string numeric = "";
                for (uint i = 0; i < Server.MaxCapacity; i++)
                {
                    numeric = server.Numeric + Base64Converter.IntToNumeric(i, 3);
                    if (server.GetUser(numeric) == null)
                    {
                        break;
                    }
                }
                return numeric;
            }
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="service"></param>
        public IRCServicePlugin(IRCService service)
        {
            if (service == null)
            {
                throw new ArgumentNullException("service");
            }
            Service = service;
            service.RegisterPlugin(this);
            servers = new List<IServer>();
        }

#region Properties
        /// <summary>
        /// Gets or sets the service
        /// </summary>
        public IRCService Service { get; protected set; }
        /// <summary>
        /// Gets or sets the server's name
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Gets or sets the plugin's main server
        /// </summary>
        public IServer MainServer
        {
            get { return mainServer; }
            set 
            { 
                mainServer = value;
                ((Server)mainServer).Plugin = this;
            }
        }
        /// <summary>
        /// Gets all servers owned by the plugin
        /// </summary>
        /// <returns></returns>
        public IEnumerable<IServer> Servers
        {
            get
            {
                return servers;
            }
        }
#endregion

#region Methods
        /// <summary>
        /// Adds a server to the network
        /// </summary>
        /// <param name="numeric"></param>
        /// <param name="name"></param>
        /// <param name="description"></param>
        /// <param name="maxUsers"></param>
        /// <param name="upLink"></param>
        /// <returns>The new server if it is successfully added 
        /// or null if not</returns>
        public IServer AddServer(string numeric, string name, string description, 
            int maxUsers, IServer upLink = null)
        {
            lock (lockObject)
            {
                if (Service.GetServer(numeric) != null)
                {
                    return null;
                }
                var newServer = new Server(Service, numeric, name, description,
                    UnixTimestamp.CurrentTimestamp(), maxUsers, true, upLink);
                newServer.Plugin = this;
                servers.Add(newServer);

                Service.AddServer(newServer);
                servers.Sort((x, y) => x.Depth - y.Depth);

                if (Service.Status == ServiceStatus.BurstCompleted)
                {
                    var command = Service.CommandFactory.CreateNewServerCommand();
                    command.Server = newServer;
                    Service.SendCommand(command, false);
                    Service.SendActionToPlugins(p => p.OnNewServer(newServer), this);
                }

                return newServer;
            }
        }
        /// <summary>
        /// Removes a owned server from the network
        /// </summary>
        /// <param name="server"></param>
        /// <param name="reason"></param>
        /// <returns>TRUE if the server is successfully removed</returns>
        public bool RemoveServer(IServer server, string reason)
        {
            lock (lockObject)
            {
                if (server == null)
                {
                    return false;
                }
                if (!server.IsControlled)
                {
                    return false;
                }
                foreach (var item in Service.Servers.ToArray())
                {
                    if (item.UpLink == server)
                    {
                        if (!RemoveServer(item, ""))
                        {
                            return false;
                        }
                    }
                }
                if (Service.Status == ServiceStatus.BurstCompleted)
                {
                    var command = Service.CommandFactory.CreateServerQuitCommand();
                    command.Server = server;
                    command.Reason = reason;
                    Service.SendCommand(command, false);
                    Service.SendActionToPlugins(
                        p => p.OnServerDisconnect(server, reason),
                        this
                    );
                }
                servers.Remove(server);
                return true;
            }
        }
        /// <summary>
        /// Creates and adds a new user to the network
        /// </summary>
        /// <param name="server"></param>
        /// <param name="nick"></param>
        /// <param name="ident"></param>
        /// <param name="host"></param>
        /// <param name="name"></param>
        /// <param name="IP"></param>
        /// <returns>The new user if it is successfully created and added 
        /// or null if not</returns>
        public IUser CreateUser(IServer server, string nick, string ident, 
            string host, string name, IPAddress IP)
        {
            lock (lockObject)
            {
                if (server == null)
                {
                    return null;
                }
                if (Service.NickExists(nick))
                {
                    return null;
                }
                var newUser = new User(server, FindFreeNumeric(server), nick, ident,
                    host, name, UnixTimestamp.CurrentTimestamp(), IP, this);
                if (newUser == null)
                {
                    return null;
                }
                (server as Server).AddUser(newUser);

                if (Service.BurstCompleted)
                {
                    var command = Service.CommandFactory.CreateNewUserCommand();
                    command.User = newUser;
                    Service.SendCommand(command, false);
                    Service.SendActionToPlugins(p => p.OnNewUser(newUser), this);
                }

                return newUser;
            }
        }

#region Virtual Functions
        /// <summary>
        /// Occurs when the burst is completed
        /// </summary>
        public virtual void OnBurstCompleted() { }
        /// <summary>
        /// Occurs when a new server joins the network
        /// </summary>
        /// <param name="server"></param>
        public virtual void OnNewServer(IServer server) { }
        /// <summary>
        /// Occurs when a server disconnects
        /// </summary>
        /// <param name="server"></param>
        /// <param name="reason"></param>
        public virtual void OnServerDisconnect(IServer server, string reason)
        {
            servers.Remove(server);
        }
        /// <summary>
        /// Occurs when a server sends a global notice
        /// </summary>
        /// <param name="server"></param>
        /// <param name="message"></param>
        public virtual void OnServerNotice(IServer server, string message) { }
        /// <summary>
        /// Occurs when a new user connects
        /// </summary>
        /// <param name="newUser"></param>
        public virtual void OnNewUser(IUser newUser) { }
        /// <summary>
        /// Occurs when a user successfully logs in
        /// </summary>
        /// <param name="newUser"></param>
        public virtual void OnUserLogin(IUser newUser) { }
        /// <summary>
        /// Occurs when a user quits
        /// </summary>
        /// <param name="user"></param>
        /// <param name="reason"></param>
        public virtual void OnUserQuit(IUser user, string reason) { }
        /// <summary>
        /// Occurs when a user is disconnected by force by a user
        /// </summary>
        /// <param name="user"></param>
        /// <param name="byWho"></param>
        /// <param name="reason"></param>
        public virtual void OnUserKilled(IUser user, IUser byWho, string reason) { }
        /// <summary>
        /// Occurs when a user is disconnected by force by a server
        /// </summary>
        /// <param name="user"></param>
        /// <param name="byWho"></param>
        /// <param name="reason"></param>
        public virtual void OnUserKilled(IUser user, IServer byWho, string reason) { }
        /// <summary>
        /// Occurs when a user changes his nick
        /// </summary>
        /// <param name="user"></param>
        public virtual void OnUserChangeNick(IUser user) { }
        /// <summary>
        /// Occurs when a user changes his fake host
        /// </summary>
        /// <param name="user"></param>
        public virtual void OnUserChangeFakeHost(IUser user) { }
        /// <summary>
        /// Occurs when a user changes his invisible mode
        /// </summary>
        /// <param name="user"></param>
        public virtual void OnUserChangeInvisible(IUser user) { }
        /// <summary>
        /// Occurs when a user changes his oper mode
        /// </summary>
        /// <param name="user"></param>
        public virtual void OnUserChangeOper(IUser user) { }
        /// <summary>
        /// Occurs when a user changes his service mode
        /// </summary>
        /// <param name="user"></param>
        public virtual void OnUserChangeService(IUser user) { }
        /// <summary>
        /// Occurs when a user changes his wallops mode
        /// </summary>
        /// <param name="user"></param>
        public virtual void OnUserChangeWallOps(IUser user) { }
        /// <summary>
        /// Occurs when a user changes his deaf mode
        /// </summary>
        /// <param name="user"></param>
        public virtual void OnUserChangeDeaf(IUser user) { }
        /// <summary>
        /// Occurs when a user changes his global notice mode
        /// </summary>
        /// <param name="user"></param>
        public virtual void OnUserChangeGlobalNotice(IUser user) { }
        /// <summary>
        /// Occurs when a user changes his server notice mode
        /// </summary>
        /// <param name="user"></param>
        public virtual void OnUserChangeServerNotice(IUser user) { }
        /// <summary>
        /// Occurs when a new channel is created
        /// </summary>
        /// <param name="channel"></param>
        public virtual void OnNewChannel(IChannel channel) { }
        /// <summary>
        /// Occurs when a user joins a channel
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="user"></param>
        public virtual void OnChannelJoin(IChannel channel, IUser user) { }
        /// <summary>
        /// Occurs when a user leaves a channel
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="user"></param>
        /// <param name="reason"></param>
        public virtual void OnChannelPart(IChannel channel, IUser user, 
            string reason) { }
        /// <summary>
        /// Occurs when a user is kicked from a channel by a user
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="user"></param>
        /// <param name="byWho"></param>
        /// <param name="reason"></param>
        public virtual void OnChannelKick(IChannel channel, IUser user, IUser byWho,
            string reason) { }
        /// <summary>
        /// Occurs when a user is kicked from a channel by a server
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="user"></param>
        /// <param name="byWho"></param>
        /// <param name="reason"></param>
        public virtual void OnChannelKick(IChannel channel, IUser user, IServer byWho, 
            string reason) { }
        /// <summary>
        /// Occurs when a ban is added to a channel by a user
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="ban"></param>
        /// <param name="byWho"></param>
        /// <param name="opMode"></param>
        public virtual void OnChannelAddBan(IChannel channel, Ban ban, IUser byWho, 
            bool opMode) { }
        /// <summary>
        /// Occurs when a ban is added to a channel by a server
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="ban"></param>
        /// <param name="byWho"></param>
        /// <param name="opMode"></param>
        public virtual void OnChannelAddBan(IChannel channel, Ban ban, IServer byWho, 
            bool opMode) { }
        /// <summary>
        /// Occurs when a ban is removed from a channel by a user
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="ban"></param>
        /// <param name="byWho"></param>
        /// <param name="opMode"></param>
        public virtual void OnChannelRemoveBan(IChannel channel, Ban ban, IUser byWho,
            bool opMode) { }
        /// <summary>
        /// Occurs when a ban is removed from a channel by a server
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="ban"></param>
        /// <param name="byWho"></param>
        /// <param name="opMode"></param>
        public virtual void OnChannelRemoveBan(IChannel channel, Ban ban, 
            IServer byWho, bool opMode) { }
        /// <summary>
        /// Occurs when a user is granted op on a channel by a user
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="to"></param>
        /// <param name="from"></param>
        /// <param name="opMode"></param>
        public virtual void OnChannelOp(IChannel channel, IUser to, IUser from, 
            bool opMode) { }
        /// <summary>
        /// Occurs when a user is granted op on a channel by a server
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="to"></param>
        /// <param name="from"></param>
        /// <param name="opMode"></param>
        public virtual void OnChannelOp(IChannel channel, IUser to, IServer from, 
            bool opMode) { }
        /// <summary>
        /// Occurs when a user removes a user's op on a channel
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="to"></param>
        /// <param name="from"></param>
        /// <param name="opMode"></param>
        public virtual void OnChannelDeOp(IChannel channel, IUser to, IUser from, 
            bool opMode) { }
        /// <summary>
        /// Occurs when a server removes a user's op on a channel
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="to"></param>
        /// <param name="from"></param>
        /// <param name="opMode"></param>
        public virtual void OnChannelDeOp(IChannel channel, IUser to, IServer from,
            bool opMode) { }
        /// <summary>
        /// Occurs when a user is granted halfop on a channel by a user
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="to"></param>
        /// <param name="from"></param>
        /// <param name="opMode"></param>
        public virtual void OnChannelHalfOp(IChannel channel, IUser to, IUser from, 
            bool opMode) { }
        /// <summary>
        /// Occurs when a user is granted halfop on a channel by a server
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="to"></param>
        /// <param name="from"></param>
        /// <param name="opMode"></param>
        public virtual void OnChannelHalfOp(IChannel channel, IUser to, IServer from, 
            bool opMode) { }
        /// <summary>
        /// Occurs when a user removes a user's halfop on a channel
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="to"></param>
        /// <param name="from"></param>
        /// <param name="opMode"></param>
        public virtual void OnChannelDeHalfOp(IChannel channel, IUser to, IUser from, 
            bool opMode) { }
        /// <summary>
        /// Occurs when a server removes a user's halfop on a channel
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="to"></param>
        /// <param name="from"></param>
        /// <param name="opMode"></param>
        public virtual void OnChannelDeHalfOp(IChannel channel, IUser to, IServer from,
            bool opMode) { }
        /// <summary>
        /// Occurs when a user is granted voice on a channel by a user
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="to"></param>
        /// <param name="from"></param>
        /// <param name="opMode"></param>
        public virtual void OnChannelVoice(IChannel channel, IUser to, IUser from, 
            bool opMode) { }
        /// <summary>
        /// Occurs when a user is granted voice on a channel by a server
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="to"></param>
        /// <param name="from"></param>
        /// <param name="opMode"></param>
        public virtual void OnChannelVoice(IChannel channel, IUser to, IServer from, 
            bool opMode) { }
        /// <summary>
        /// Occurs when a user removes a user's a voice on a channel
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="to"></param>
        /// <param name="from"></param>
        /// <param name="opMode"></param>
        public virtual void OnChannelDeVoice(IChannel channel, IUser to, IUser from, 
            bool opMode) { }
        /// <summary>
        /// Occurs when a server removes a user's voice on a channel
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="to"></param>
        /// <param name="from"></param>
        /// <param name="opMode"></param>
        public virtual void OnChannelDeVoice(IChannel channel, IUser to, IServer from,
            bool opMode) { }
        /// <summary>
        /// Occurs when a user changes a channel's key
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="key"></param>
        /// <param name="from"></param>
        /// <param name="opMode"></param>
        public virtual void OnChannelKey(IChannel channel, string key, IUser from, 
            bool opMode) { }
        /// <summary>
        /// Occurs when a server changes a channel's key
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="key"></param>
        /// <param name="from"></param>
        /// <param name="opMode"></param>
        public virtual void OnChannelKey(IChannel channel, string key, IServer from,
            bool opMode) { }
        /// <summary>
        /// Occurs when a user changes a channel's limit
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="limit"></param>
        /// <param name="from"></param>
        /// <param name="opMode"></param>
        public virtual void OnChannelLimit(IChannel channel, int limit, IUser from, 
            bool opMode) { }
        /// <summary>
        /// Occurs when a server changes a channel's limit
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="limit"></param>
        /// <param name="from"></param>
        /// <param name="opMode"></param>
        public virtual void OnChannelLimit(IChannel channel, int limit, IServer from,
            bool opMode) { }
        /// <summary>
        /// Occurs when a user changes a channel's mode
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="mode"></param>
        /// <param name="status"></param>
        /// <param name="from"></param>
        /// <param name="opMode"></param>
        public virtual void OnChannelModeChange(IChannel channel, char mode, 
            bool status, IUser from, bool opMode) { }
        /// <summary>
        /// Occurs when a server changes a channel's mode
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="mode"></param>
        /// <param name="status"></param>
        /// <param name="from"></param>
        /// <param name="opMode"></param>
        public virtual void OnChannelModeChange(IChannel channel, char mode, 
            bool status, IServer from, bool opMode) { }
        /// <summary>
        /// Occurs when a user cleares all the modes on a channel
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="modes"></param>
        /// <param name="from"></param>
        public virtual void OnChannelClearModes(IChannel channel, string modes, 
            IUser from) { }
        /// <summary>
        /// Occurs when a server cleares all the modes on a channel
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="modes"></param>
        /// <param name="from"></param>
        public virtual void OnChannelClearModes(IChannel channel, string modes, 
            IServer from) { }
        /// <summary>
        /// Occurs when a global message is sent by a user
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="message"></param>
        public virtual void OnGlobalMessage(IUser from, string to, string message) { }
        /// <summary>
        /// Occurs when a global message is sent by a server
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="message"></param>
        public virtual void OnGlobalMessage(IServer from, string to, string message) { }
        /// <summary>
        /// Occurs when a global notice is sent by a user
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="message"></param>
        public virtual void OnGlobalNotice(IUser from, string to, string message) { }
        /// <summary>
        /// Occurs when a global notice is sent by a server
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="message"></param>
        public virtual void OnGlobalNotice(IServer from, string to, string message) { }
        /// <summary>
        /// Occurs when a user owned by the plugin receives a private message
        /// from a user
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="message"></param>
        public virtual void OnPrivateMessage(IUser from, IUser to, string message) { }
        /// <summary>
        /// Occurs when a user owned by the plugin receives a private message
        /// from a server
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="message"></param>
        public virtual void OnPrivateMessage(IServer from, IUser to, string message) { }
        /// <summary>
        /// Occurs when a user owned by the plugin receives a CTPCP request
        /// from a user
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="ctcp"></param>
        /// <param name="parameter"></param>
        public virtual void OnPrivateCTCP(IUser from, IUser to, string ctcp, 
            string parameter) { }
        /// <summary>
        /// Occurs when a user owned by the plugin receives a CTCP request 
        /// from a server
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="ctcp"></param>
        /// <param name="parameter"></param>
        public virtual void OnPrivateCTCP(IServer from, IUser to, string ctcp, 
            string parameter) { }
        /// <summary>
        /// Occurs when a user owned by the plugin receives a notice from a user
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="message"></param>
        public virtual void OnNotice(IUser from, IUser to, string message) { }
        /// <summary>
        /// Occurs when a user owned by the plugin receives a notice from a server
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="message"></param>
        public virtual void OnNotice(IServer from, IUser to, string message) { }
        /// <summary>
        /// Occurs when a user owned by the plugin receives a CTCP reply 
        /// from a user
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="ctcp"></param>
        /// <param name="parameter"></param>
        public virtual void OnCTCPReply(IUser from, IUser to, string ctcp, 
            string parameter) { }
        /// <summary>
        /// Occurs when a user owned by the plugin receives a CTCP reply 
        /// from a server
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="ctcp"></param>
        /// <param name="parameter"></param>
        public virtual void OnCTCPReply(IServer from, IUser to, string ctcp, 
            string parameter) { }
        /// <summary>
        /// Occurs when a user sends a channel message
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="message"></param>
        public virtual void OnChannelMessage(IUser from, string to, 
            string message) { }
        /// <summary>
        /// Occurs when a server sends a channel message
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="message"></param>
        public virtual void OnChannelMessage(IServer from, string to, 
            string message) { }
        /// <summary>
        /// Occurs when a user sends a channel CTCP request
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="ctcp"></param>
        /// <param name="parameter"></param>
        public virtual void OnChannelCTCP(IUser from, string to, string ctcp, 
            string parameter) { }
        /// <summary>
        /// Occurs when a server sends a channel CTCP request
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="ctcp"></param>
        /// <param name="parameter"></param>
        public virtual void OnChannelCTCP(IServer from, string to, string ctcp, 
            string parameter) { }
        /// <summary>
        /// Occurs when a user sends a channel notice
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="message"></param>
        public virtual void OnChannelNotice(IUser from, string to, string message) { }
        /// <summary>
        /// Occurs when a server sends a channel notice
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="message"></param>
        public virtual void OnChannelNotice(IServer from, string to, string message) { }
        /// <summary>
        /// Occurs when a channel topic is changed
        /// </summary>
        /// <param name="channel"></param>
        public virtual void OnChannelTopicChange(IChannel channel) { }
        /// <summary>
        /// Occurs when a server sends a wallops
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="message"></param>
        public virtual void OnWallops(IServer from, string message) { }
        /// <summary>
        /// Occurs when a user sends a wallops
        /// </summary>
        /// <param name="from"></param>
        /// <param name="message"></param>
        public virtual void OnWallops(IUser from, string message) { }
        /// <summary>
        /// Occurs when a user is invited to a channel
        /// </summary>
        /// <param name="user"></param>
        /// <param name="channel"></param>
        public virtual void OnUserInvitedToChannel(IUser user, IChannel channel, 
            IHasNumeric by) { }
    #endregion

#endregion
    }
}
