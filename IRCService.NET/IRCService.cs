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
using System.Net;
using System.Net.Sockets;
using IRCServiceNET.Helpers;
using IRCServiceNET.Entities;
using IRCServiceNET.Plugins;
using IRCServiceNET.Protocols;

namespace IRCServiceNET
{
    /// <summary>
    /// The irc service class
    /// </summary>
    public class IRCService : IDisposable
    {
        /// <summary>
        /// The socket used for network communications
        /// </summary>
        private Socket socket;
        /// <summary>
        /// The size of the buffer used for receiving data
        /// </summary>
        private const int receiveBufferSize = 8192;
        /// <summary>
        /// The buffer used for receiving data
        /// </summary>
        private byte[] receiveBuffer = new byte[receiveBufferSize];
        /// <summary>
        /// The string buffer used for receiving data
        /// </summary>
        private String receiveBufferString = "";
        /// <summary>
        /// The buffer used for sending data
        /// </summary>
        private byte[] sendBuffer;
        /// <summary>
        /// Used to control if the previous async write is not finished
        /// </summary>
        private bool waitToWrite;
        /// <summary>
        /// Is the service disposed?
        /// </summary>
        private bool disposed;
        /// <summary>
        /// Main server numeric
        /// </summary>
        protected string numeric = "";
        /// <summary>
        /// Main server name
        /// </summary>
        protected string name = "";
        /// <summary>
        /// Main server description
        /// </summary>
        protected string description = "";
        /// <summary>
        /// Network authentication username
        /// </summary>
        protected string username = "";
        /// <summary>
        /// Network authentication password
        /// </summary>
        protected string password = "";
        /// <summary>
        /// Start timestamp or the service
        /// </summary>
        protected UnixTimestamp startTimestamp = UnixTimestamp.CurrentTimestamp();
        /// <summary>
        /// Timestamp of the last successfull connection
        /// </summary>
        protected UnixTimestamp connectionTimestamp;
        /// <summary>
        /// Servers on the network
        /// </summary>
        protected Dictionary<string, Server> servers;
        /// <summary>
        /// Registered plugins
        /// </summary>
        protected List<IRCServicePlugin> plugins;
        /// <summary>
        /// Channels to send during burst
        /// </summary>
        protected Dictionary<string, Channel> toBurst;
        /// <summary>
        /// Encoding to use
        /// </summary>
        protected Encoding encoding;
        /// <summary>
        /// Object used for locking on sending and receivign data
        /// </summary>
        protected object lockObject = new object();

#region Private Methods

        /// <summary>
        /// Occurs when a connection is successfull or fails
        /// </summary>
        /// <param name="ar"></param>
        private void ConnectCallback(IAsyncResult ar)
        {
            lock (lockObject)
            {
                var socket = ar.AsyncState as Socket;
                if (socket != this.socket)
                {
                    return;
                }
                try
                {
                    socket.EndConnect(ar);
                }
                catch (Exception e)
                {
                    OnConnectionError(e);
                }

                if (socket.Connected == false)
                {
                    OnConnectionLost();
                    Disconnect();
                    return;
                }
                socket.BeginReceive(
                    receiveBuffer,
                    0,
                    receiveBufferSize,
                    0,
                    new AsyncCallback(ReceiveCallback),
                    socket
                );

                OnConnect();
            }
        }
        /// <summary>
        /// Occurs when data is received over the network
        /// </summary>
        /// <param name="ar"></param>
        private void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                lock (lockObject)
                {
                    var socket = ar.AsyncState as Socket;
                    if (socket != this.socket)
                    {
                        return;
                    }
                    if (socket.Connected == false)
                    {
                        OnConnectionLost();
                        Disconnect();
                        return;
                    }
                    int bytesRead = socket.EndReceive(ar);
                    if (bytesRead > 0)
                    {
                        receiveBufferString +=  ConnectionEncoding.GetString(
                            receiveBuffer, 
                            0, 
                            bytesRead
                        );
                        int lastRowSeparator = 
                            receiveBufferString.LastIndexOf('\n');
                        if (lastRowSeparator != -1)
                        {
                            string receivedRows =
                                receiveBufferString.Substring(0, 
                                lastRowSeparator + 1);
                            OnReceive(receivedRows);
                            if (lastRowSeparator == 
                                receiveBufferString.Length - 1)
                            {
                                receiveBufferString = "";
                            }
                            else
                            {
                                receiveBufferString =
                                    receiveBufferString.Substring(
                                    lastRowSeparator + 1,
                                    receiveBufferString.Length - lastRowSeparator - 1
                                );
                            }
                        }
                        socket.BeginReceive(
                            receiveBuffer,
                            0,
                            receiveBufferSize,
                            0,
                            new AsyncCallback(ReceiveCallback),
                            socket
                        );
                    }
                    else
                    {
                        Disconnect();
                    }
                }
            }
            catch (ObjectDisposedException) { }
            catch (Exception e)
            {
                OnConnectionError(e);
            }
        }
        /// <summary>
        /// Processes received data
        /// </summary>
        /// <param name="data">The data to be processesed</param>
        /// <param name="suppressEvent">Suppress the ReceiveRawDataEvent?</param>
        private void OnReceive(string data, bool suppressEvent = false)
        {
            string[] lines = data.Split('\n');
            string toProcess;
            foreach (string item in lines)
            {
                if (item.Length > 0)
                {
                    if (item[item.Length - 1] == '\r')
                    {
                        toProcess = item.Substring(0, item.Length - 1);
                    }
                    else
                    {
                        toProcess = item;
                    }
                    if ( ! suppressEvent)
                    {
                        OnReceiveRawData(toProcess);
                    }
                    Parser.Process(toProcess);
                }
            }
        }
        /// <summary>
        /// Sends raw data over the network
        /// </summary>
        /// <param name="data"></param>
        private void SendData(byte[] data)
        {
            try
            {               
                lock (lockObject)
                {
                    if (sendBuffer != null)
                    {
                        var newBuffer = new byte[data.Length + sendBuffer.Length];
                        System.Buffer.BlockCopy(
                            sendBuffer,
                            0,
                            newBuffer,
                            0,
                            sendBuffer.Length
                        );
                        System.Buffer.BlockCopy(
                            data,
                            0,
                            newBuffer,
                            sendBuffer.Length,
                            data.Length
                        );
                        data = newBuffer;
                        sendBuffer = null;
                    }
                    if (waitToWrite)
                    {
                        sendBuffer = data;
                        return;
                    }
                    waitToWrite = true;
                    socket.BeginSend(data, 0, data.Length, SocketFlags.None,
                        (ar) =>
                        {
                            lock (lockObject)
                            {
                                try
                                {
                                    var sendSocket = ar.AsyncState as Socket;
                                    if (sendSocket != this.socket)
                                    {
                                        return;
                                    }
                                    int sentBytes = sendSocket.EndSend(ar);
                                    waitToWrite = false;
                                    if (sendBuffer != null)
                                    {
                                        var toSend = sendBuffer;
                                        sendBuffer = null;
                                        SendData(toSend);
                                    }
                                }
                                catch (Exception e)
                                {
                                    OnConnectionError(e);
                                }
                            }
                        }, socket);
                }
            }
            catch (Exception e)
            {
                OnConnectionError(e);
            }
        }
#endregion

#region Protected Methods
        /// <summary>
        /// Occurs when a connection is reset
        /// </summary>
        protected void Reset()
        {
            lock (lockObject)
            {
                socket = new Socket(
                    AddressFamily.InterNetwork,
                    SocketType.Stream,
                    ProtocolType.Tcp
                );
                servers = new Dictionary<string, Server>();
                plugins = new List<IRCServicePlugin>();
                toBurst =
                    new Dictionary<string, Channel>(StringComparer.OrdinalIgnoreCase);
                MainServer = null;
                UpLink = null;
                Status = ServiceStatus.Disconnected;
                PreparedForPlugins = false;
                OnServiceReset();
            }
        }
        /// <summary>
        /// Occurs when a new connection is established
        /// </summary>
        protected void OnConnect()
        {
            Status = ServiceStatus.Connected;
            connectionTimestamp = UnixTimestamp.CurrentTimestamp();

            OnConnectionEstablished();

            if (MainServer == null)
            {
                PrepareForPlugins();
            }

            var authCommand = CommandFactory.CreateServerAuthenticationCommand();
            authCommand.Password = password;
            SendCommand(authCommand, false);

            var introCommand = CommandFactory.CreateServerIntroductionCommand();
            introCommand.Server = MainServer;
            introCommand.StartTimeStamp = startTimestamp;
            SendCommand(introCommand, false);
        }
        /// <summary>
        /// Takes care of the actual disposing
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            lock (lockObject)
            {
                if (disposed)
                {
                    return;
                }
                if (disposing)
                {
                    socket.Dispose();
                }
                disposed = true;
            }
        }
       
 #endregion

#region Properties
        /// <summary>
        /// Gets or sets the host where the service should connect to
        /// </summary>
        public String Host { get; set; }
        /// <summary>
        /// Gets or sets the port on which the service should connect to
        /// </summary>
        public int Port { get; set; }
        /// <summary>
        /// Gets or sets the numeric of the service's main server
        /// </summary>
        public string Numeric
        {
            get { return numeric; }
            set
            {
                if ( ! socket.Connected)
                {
                    numeric = value;                
                }
            }
        }
        /// <summary>
        /// Gets or sets the name of the service's main server
        /// </summary>
        public string Name
        {
            get { return name; }
            set
            {
                if ( ! socket.Connected)
                {
                    name = value;
                }
            }
        }
        /// <summary>
        /// Gets or sets the description of the service's main server
        /// </summary>
        public string Description
        {
            get { return description; }
            set
            {
                if ( ! socket.Connected)
                {
                    description = value;
                }
            }
        }
        /// <summary>
        /// Gets or sets the username for the connection
        /// </summary>
        public string Username
        {
            get { return username; }
            set
            {
                if ( ! socket.Connected)
                {
                    username = value;
                }
            }

        }
        /// <summary>
        /// Gets or sets the password for the connection
        /// </summary>
        public string Password
        {
            get { return password; }
            set
            {
                if ( ! socket.Connected)
                {
                    password = value;
                }
            }
        }
        /// <summary>
        /// Gets or sets command factory of a server-server protocol
        /// </summary>
        public ICommandFactory CommandFactory { get; protected set; }
        /// <summary>
        /// Gets or sets the parser for a server-server protocol
        /// </summary>
        public IParser Parser { get; protected set; }
        /// <summary>
        /// Gets or sets the uplink server
        /// </summary>
        public Server UpLink { get; set; }
        /// <summary>
        /// Gets or sets the service status
        /// </summary>
        public ServiceStatus Status { get; set; } 
        /// <summary>
        /// Is the burst completed?
        /// </summary>
        public bool BurstCompleted
        {
            get { return Status == ServiceStatus.BurstCompleted; }
        }
        /// <summary>
        /// Gets or sets the suffix used for users that are logged on
        /// ex. .users.network.tld
        /// </summary>
        public string LoginHost { get; set; }
        /// <summary>
        /// Counts all the users on the network
        /// </summary>
        public int UserCount
        {
            get
            {
                lock (lockObject)
                {
                    return servers.Select(p => p.Value.UserCount).Sum();
                }
            }
        }
        /// <summary>
        /// Gets the main server of the service
        /// </summary>
        public Server MainServer { get; protected set; }
        /// <summary>
        /// Gets all the servers on the network
        /// </summary>
        public IEnumerable<Server> Servers
        {
            get
            {
                lock (lockObject)
                {
                    return servers.Values.ToArray();
                }
            }
        }
        /// <summary>
        /// Gets all the users on the network
        /// </summary>
        public IEnumerable<IUser> Users
        {
            get
            {
                lock (lockObject)
                {
                    var allUsers = new List<IUser>();
                    foreach (var pair in servers)
                    {
                        allUsers.AddRange(pair.Value.Users);
                    }
                    return allUsers;
                }
            }
        }
        /// <summary>
        /// Gets all the registered plugins
        /// </summary>
        /// <returns></returns>
        public IEnumerable<IRCServicePlugin> Plugins
        {
            get
            {
                lock (lockObject)
                {
                    return plugins.ToArray();
                }                
            }
        }
        /// <summary>
        /// Gets or sets the encoding
        /// </summary>
        public Encoding ConnectionEncoding
        {
            get
            {
                if (encoding == null)
                {
                    encoding = Encoding.UTF8;
                }
                return encoding;
            }
            set { encoding = value; }
        }
        /// <summary>
        /// Checks if plugins have already been registered since the last reset
        /// </summary>
        public bool PreparedForPlugins { get; protected set; }
            
#endregion
 
#region Events
        /// <summary>
        /// Occurs when raw data is received
        /// </summary>
        public event EventHandler<LogEventArgs> ReceiveRawData;
        private void OnReceiveRawData(string data)
        {
            if (ReceiveRawData != null)
            {
                ReceiveRawData(this, new LogEventArgs() { Data = data });
            }
        }
        /// <summary>
        /// Occurs when raw data is sent
        /// </summary>
        public event EventHandler<LogEventArgs> SendRawData;
        private void OnSendRawData(string data)
        {
            if (SendRawData != null)
            {
                SendRawData(this, new LogEventArgs() { Data = data });
            }
        }
        /// <summary>
        /// Occurs when an event need to be logged
        /// </summary>
        public event EventHandler<LogEventArgs> Log;
        private void OnLog(string data)
        {
            if (Log != null)
            {
                Log(this, new LogEventArgs() { Data = data });
            }
        }
        /// <summary>
        /// Occurs when a connection is established
        /// </summary>
        public event EventHandler ConnectionEstablished;
        private void OnConnectionEstablished()
        {
            if (ConnectionEstablished != null)
            {
                ConnectionEstablished(this, EventArgs.Empty);
            }
        }
        /// <summary>
        /// Occurs when a connection is lost
        /// </summary>
        public event EventHandler ConnectionLost;
        private void OnConnectionLost()
        {
            if (ConnectionLost != null)
            {
                ConnectionLost(this, EventArgs.Empty);
            }
        }
        /// <summary>
        /// Occurs when an error is encountered while reading or writing data
        /// over the network
        /// </summary>
        public event EventHandler<ExceptionEventArgs> ConnectionError;
        private void OnConnectionError(Exception exception)
        {
            if (ConnectionError != null)
            {
                ConnectionError(this, 
                    new ExceptionEventArgs() {  Exception = exception });
            }
        }
        /// <summary>
        /// Occurs when the service is reset
        /// </summary>
        public event EventHandler ServiceReset;
        private void OnServiceReset()
        {
            if (ServiceReset != null)
            {
                ServiceReset(this, EventArgs.Empty);
            }
        }
        /// <summary>
        /// Occurs when an error is received from the uplink server
        /// </summary>
        public event EventHandler<LogEventArgs> UplinkError;
        public void OnUplinkError(string message)
        {
            if (UplinkError != null)
            {
                UplinkError(this, new LogEventArgs() { Data = message });
            }
        }
#endregion
        
#region Public Methods
        /// <summary>
        /// Main constructor
        /// </summary>
        /// <param name="parser"></param>
        /// <param name="commandFactory"></param>
        public IRCService(IParser parser, ICommandFactory commandFactory)
        {
            Host = "127.0.0.1";
            Port = 4400;
            if (parser == null)
            {
                throw new ArgumentNullException("parser");
            }
            if (commandFactory == null)
            {
                throw new ArgumentNullException("commandFactory");
            }
            Parser = parser;
            parser.Service = this;
            CommandFactory = commandFactory;            
            Reset();
        }
        /// <summary>
        /// Connects to a network
        /// </summary>
        public virtual void Connect()
        {
            lock (lockObject)
            {
                if (disposed)
                {
                    throw new ObjectDisposedException("IRCService",
                        "The service is disposed");
                }
                if (socket.Connected)
                {
                    Disconnect();
                }
                socket.BeginConnect(
                    Host,
                    Port,
                    new AsyncCallback(ConnectCallback),
                    socket
                );
            }
        }
        /// <summary>
        /// Sends a command over the network
        /// </summary>
        /// <param name="command">The command to be sent</param>
        /// <param name="process">Should the command data be processed?</param>
        public virtual void SendCommand(ICommand command, bool process = true)
        {
            lock (lockObject)
            {
                string commandString = command.ToString() + '\n';
                if (socket == null)
                {
                    return;
                }
                OnSendRawData(commandString);

                byte[] data = ConnectionEncoding.GetBytes(commandString);
                SendData(data);

                if (process)
                {
                    OnReceive(commandString, true);
                }
            }
        }
        /// <summary>
        /// Adds a server to the list of servers
        /// </summary>
        /// <param name="server"></param>
        /// <returns>TRUE if the server is successfully added</returns>
        public bool AddServer(Server server)
        {
            lock (lockObject)
            {
                if (servers.Keys.Contains(server.Numeric))
                {
                    return false;
                }
                servers.Add(server.Numeric, server);
                return true;
            }
        }
        /// <summary>
        /// Searches for a server by numeric
        /// </summary>
        /// <param name="numeric"></param>
        /// <returns>The server if found or null if not</returns>
        public Server GetServer(string numeric)
        {
            lock (lockObject)
            {
                if (servers.Keys.Contains(numeric))
                {
                    return servers[numeric];
                }
                else
                {
                    return null;
                }
            }
        }
        /// <summary>
        /// Searches for a server by name
        /// </summary>
        /// <param name="name"></param>
        /// <returns>The server if found or null if not</returns>
        public Server GetServerByName(string name)
        {
            lock (lockObject)
            {
                foreach (KeyValuePair<string, Server> pair in servers)
                {
                    if (pair.Value.Name.ToLower() == name.ToLower())
                    {
                        return pair.Value;
                    }
                }
                return null;
            }
        }
        /// <summary>
        /// Searches for a server by a users numeric
        /// </summary>
        /// <param name="userNumeric"></param>
        /// <returns>The server if found or null if not</returns>
        public Server GetServerByUser(string userNumeric)
        {
            lock (lockObject)
            {
                if (userNumeric.Length < 2)
                {
                    return null;
                }
                return GetServer(userNumeric.Substring(0, 2));
            }
        }
        /// <summary>
        /// Removes a server from the server list and notifies all plugins
        /// </summary>
        /// <param name="server"></param>
        /// <param name="reason"></param>
        /// <returns>TRUE if the server is successfully removed</returns>
        public bool RemoveServer(Server server, string reason)
        {
            lock (this)
            {
                return RemoveServer(server.Numeric, reason);
            }
        }
        /// <summary>
        /// Removes a server from the server list and notifies all plugins
        /// </summary>
        /// <param name="serverNumeric"></param>
        /// <param name="reason"></param>
        /// <returns>TRUE if the server is successfully removed</returns>
        public bool RemoveServer(string serverNumeric, string reason)
        {
            lock (lockObject)
            {
                if ( ! servers.Keys.Contains(serverNumeric))
                {
                    return false;
                }
                Server server = servers[serverNumeric];

                var toRemove = (from p in servers
                                where p.Value.UpLink == server
                                select p.Key).ToArray();

                SendActionToPlugins(p => p.OnServerDisconnect(server, reason));
                if ( ! servers.Remove(serverNumeric))
                {
                    return false;
                }
                foreach (string removeNumeric in toRemove)
                {
                    if ( ! RemoveServer(removeNumeric, reason))
                    {
                        return false;
                    }
                }
                return true;
            }
        }
        /// <summary>
        /// Does any user on the network have the specified nick?
        /// </summary>
        /// <param name="nick"></param>
        /// <returns>TRUE if a user is found</returns>
        public bool NickExists(string nick)
        {
            lock (lockObject)
            {
                foreach (KeyValuePair<string, Server> pair in servers)
                {
                    if (pair.Value.NickExists(nick))
                    {
                        return true;
                    }
                }
                return false;
            }
        }
        /// <summary>
        /// Searches for a user by numeric
        /// </summary>
        /// <param name="numeric"></param>
        /// <returns>The user if found or null if not</returns>
        public IUser GetUser(string numeric)
        {
            lock (lockObject)
            {
                if (numeric.Length != 5)
                {
                    return null;
                }
                var userServer = GetServer(numeric.Substring(0, 2));
                if (userServer == null)
                {
                    return null;
                }
                return userServer.GetUser(numeric);
            }
        }
        /// <summary>
        /// Searches for a user by nick
        /// </summary>
        /// <param name="nick"></param>
        /// <returns>The user if found or null if not</returns>
        public IUser GetUserByNick(string nick)
        {
            lock (lockObject)
            {
                IUser user;
                foreach (var pair in servers)
                {
                    user = pair.Value.GetUserByNick(nick);
                    if (user != null)
                    {
                        return user;
                    }
                }
                return null;
            }
        }
        /// <summary>
        /// Is the server on the network?
        /// </summary>
        /// <param name="server"></param>
        /// <returns>TRUE if the server is found</returns>
        public bool ContainsServer(Server server)
        {
            lock (lockObject)
            {
                return ContainsServer(server.Numeric);
            }
        }
        /// <summary>
        /// Is the server on the network?
        /// </summary>
        /// <param name="numeric"></param>
        /// <returns>TRUE if the server is found</returns>
        public bool ContainsServer(string numeric)
        {
            lock (lockObject)
            {
                return servers.Keys.Contains(numeric);
            }
        }        
        /// <summary>
        /// Returns all the users from a channel
        /// </summary>
        /// <param name="channel"></param>
        /// <returns></returns>
        public IEnumerable<ChannelEntry> GetChannelUsers(string channel)
        {
            lock (lockObject)
            {
                var entries = new List<ChannelEntry>();
                IChannel currentChannel = null;
                foreach (var pair in servers)
                {
                    currentChannel = pair.Value.GetChannel(channel);
                    if (currentChannel != null)
                    {
                        entries.AddRange(currentChannel.Entries);
                    }
                }
                return entries;
            }
        }
        /// <summary>
        /// Returns a channel from a server that has users on it
        /// </summary>
        /// <param name="channelName"></param>
        /// <returns>The channel or null if it is not found</returns>
        public IChannel GetChannel(string channelName)
        {
            lock (lockObject)
            {
                IChannel result = null;
                foreach (var pair in servers)
                {
                    result = pair.Value.GetChannel(channelName);
                    if (result != null)
                    {
                        break;
                    }
                }
                return result;
            }
        }
        /// <summary>
        /// Logs a new event
        /// </summary>
        /// <param name="data"></param>
        public virtual void AddLog(string data)
        {
            OnLog(data);
        }
        /// <summary>
        /// Disconnects from the network
        /// </summary>
        public virtual void Disconnect()
        {
            lock (lockObject)
            {
                if (socket.Connected)
                {
                    socket.Disconnect(true);
                    OnConnectionLost();
                }
                Reset();
            }
        }
        /// <summary>
        /// Disposes of the irc service
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

#region Plugin related
        /// <summary>
        /// Prepares the service for accepting plugins
        /// </summary>
        public virtual void PrepareForPlugins()
        {
            lock (lockObject)
            {
                if (MainServer == null)
                {
                    MainServer = new Server(
                        this,
                        numeric,
                        name,
                        description,
                        startTimestamp,
                        Server.MaxCapacity,
                        true,
                        null
                    );
                    AddServer(MainServer);
                }
                PreparedForPlugins = true;
            }
        }
        /// <summary>
        /// Registers a plugin
        /// </summary>
        /// <param name="plugin"></param>
        public virtual void RegisterPlugin(IRCServicePlugin plugin)
        {
            lock (lockObject)
            {
                if (MainServer == null)
                {
                    throw new NotPreparedForPluginsException();
                }
                if (Status != ServiceStatus.Disconnected)
                {
                    throw new CannotRegisterPluginException();
                }
                plugins.Add(plugin);
            }
        }
        /// <summary>
        /// Registers all the plugin servers and their clients on the network
        /// </summary>
        public virtual void AddPluginServersAndClientsToNetwork()
        {
            lock (lockObject)
            {
                IEnumerable<IUser> users;
                foreach (IRCServicePlugin plugin in plugins)
                {
                    foreach (var server in plugin.Servers)
                    {
                        var command = CommandFactory.CreateNewServerCommand();
                        command.Server = server;
                        SendCommand(command, false);

                        users = server.Users;
                        foreach (var user in users)
                        {
                            AddPluginUser(user);
                        }
                    }
                }
                users = MainServer.Users;
                foreach (User user in users)
                {
                    AddPluginUser(user);
                }
            }
        }
        /// <summary>
        /// Registers a plugin's user on the network
        /// </summary>
        /// <param name="user"></param>
        private void AddPluginUser(IUser user)
        {
            string userModes = "+";
            string[] userModeParameters = null;
            if (user.IsInvisible) { userModes += "i"; }
            if (user.IsOper) { userModes += "o"; }
            if (user.IsService) { userModes += "k"; }
            if (user.IsDeaf) { userModes += "d"; }
            if (user.IsWallOps) { userModes += "w"; }
            if (user.IsGlobalNotice) { userModes += "g"; }
            if (user.IsServerNotice) { userModes += "s"; }
            if (user.FakeIdent.Length > 0 && user.FakeHost.Length > 0)
            {
                userModes += "h";
                userModeParameters = 
                    new string[] { user.FakeIdent + "@" + user.FakeHost };
            }
            var command = CommandFactory.CreateNewUserCommand();
            command.User = user;
            command.Modes = userModes;
            command.ModeParameters = userModeParameters;
            SendCommand(command, false);
        }
        /// <summary>
        /// Sends an action to the registered plugins
        /// </summary>
        /// <param name="action">The action to be taken</param>
        /// <param name="exclude">Plugin that should not receive the action</param>
        public void SendActionToPlugins(Action<IRCServicePlugin> action, 
            IRCServicePlugin exclude = null)
        {
            foreach (var item in plugins)
            {
                if (item != exclude)
                {
                    action(item);
                }
            }
        }
        /// <summary>
        /// Adds a channel entry to be sent on burst
        /// </summary>
        /// <param name="user"></param>
        /// <param name="channel"></param>
        /// <param name="op"></param>
        /// <param name="voice"></param>
        /// <param name="halfop"></param>
        public virtual void AddtoBurst(User user, string channel, 
            bool op, bool voice, bool halfop)
        {
            lock (lockObject)
            {
                if (Status == ServiceStatus.BurstCompleted)
                {
                    throw new BurstCompletedException();
                }
                if (user == null)
                {
                    return;
                }
                Channel channelToBurst;
                if ( ! toBurst.ContainsKey(channel))
                {
                    channelToBurst = new Channel(null, channel, null);
                    toBurst.Add(channel, channelToBurst);
                }
                else
                {
                    channelToBurst = toBurst[channel];
                }
                if (channelToBurst.GetEntry(user) != null)
                {
                    return;
                }
                channelToBurst.AddUser(user, op, voice, halfop);
            }
        }
        /// <summary>
        /// Sends the service burst
        /// </summary>
        public virtual void SendBurst()
        {
            lock (this)
            {
                UnixTimestamp burstTimestamp = null;

                foreach (var pair in toBurst)
                {
                    var existingChannel = GetChannel(pair.Key);
                    if (existingChannel != null)
                    {
                        burstTimestamp = existingChannel.CreationTimeStamp;
                    }
                    else
                    {
                        burstTimestamp = UnixTimestamp.CurrentTimestamp();
                    }

                    var command = CommandFactory.CreateChannelBurstCommand();
                    command.Server = MainServer;
                    command.Channel = pair.Value;
                    command.BurstTimestamp = burstTimestamp;

                    SendCommand(command);
                }

                toBurst = null;
            }
        }
#endregion

#endregion
    }
}
