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
using IRCServiceNET.Plugins;
using IRCServiceNET.Helpers;
using IRCServiceNET.Entities;

namespace IRCServiceNET.Actions
{
    /// <summary>
    /// A class that contains methods that correspond to network actions 
    /// that a user can perform
    /// </summary>
    public class UserAction : NetworkAction
    {
        /// <summary>
        /// Changes a user's mode
        /// </summary>
        /// <param name="mode"></param>
        /// <param name="value"></param>
        private void ChangeUserMode(string mode, bool value)
        {
            var command =
                Plugin.Service.CommandFactory.CreateChangeUserModeCommand();
            command.User = User; 
            command.Mode = mode; 
            command.Value = value;
            Plugin.Service.SendCommand(command, false);
        }
        /// <summary>
        /// The user
        /// </summary>
        protected User User { get; set; }
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="user"></param>
        public UserAction(User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("User");
            }
            User = user;
            Server = user.Server;
            Plugin = user.Plugin;
            CheckPlugin();
        }

#region Methods
        /// <summary>
        /// Performs a quit
        /// </summary>
        /// <param name="reason"></param>
        public void Quit(string reason)
        {
            User.Server.RemoveUser(User);
            if (User.Server.Service.BurstCompleted)
            {
                var command = 
                    Server.Service.CommandFactory.CreateUserQuitCommand();
                command.User = User;
                command.Reason = reason;
                Server.Service.SendCommand(command, false);
            }
            User.Server.Service.SendActionToPlugins(
                p => p.OnUserQuit(User, reason), 
                Plugin
            );
        }
        /// <summary>
        /// Changes the invisible mode
        /// </summary>
        /// <param name="value"></param>
        public void ChangeInvisible(bool value)
        {
            if (User.IsInvisible != value)
            {
                User.IsInvisible = value;
                ChangeUserMode("i", value);
                Plugin.Service.SendActionToPlugins(
                    p => p.OnUserChangeInvisible(User), Plugin
                );
            }
        }
        /// <summary>
        /// Changes the oper mode
        /// </summary>
        /// <param name="value"></param>
        public void ChangeOper(bool value)
        {
            if (User.IsOper != value)
            {
                User.IsOper = value;
                ChangeUserMode("o", value);
                Plugin.Service.SendActionToPlugins(
                    p => p.OnUserChangeOper(User), Plugin
                );
            }
        }
        /// <summary>
        /// Changes the service mode
        /// </summary>
        /// <param name="value"></param>
        public void ChangeService(bool value)
        {
            if (User.IsService != value)
            {
                User.IsService = value;
                ChangeUserMode("k", value);
                Plugin.Service.SendActionToPlugins(
                    p => p.OnUserChangeService(User), Plugin
                );
            }
        }
        /// <summary>
        /// Changes the deaf mode
        /// </summary>
        /// <param name="value"></param>
        public void ChangeDeaf(bool value)
        {
            if (User.IsDeaf != value)
            {
                User.IsDeaf = value;
                ChangeUserMode("d", value);
                Plugin.Service.SendActionToPlugins(
                    p => p.OnUserChangeDeaf(User), Plugin
                );
            }
        }
        /// <summary>
        /// Change the wallops mode
        /// </summary>
        /// <param name="value"></param>
        public void ChangeWallOps(bool value)
        {
            if (User.IsWallOps != value)
            {
                User.IsWallOps = value;
                ChangeUserMode("w", value);
                Plugin.Service.SendActionToPlugins(
                    p => p.OnUserChangeWallOps(User), Plugin
                );
            }
        }
        /// <summary>
        /// Changes the global notice mode
        /// </summary>
        /// <param name="value"></param>
        public void ChangeGlobalNotice(bool value)
        {
            if (User.IsGlobalNotice != value)
            {
                User.IsGlobalNotice = value;
                ChangeUserMode("g", value);
                Plugin.Service.SendActionToPlugins(
                    p => p.OnUserChangeGlobalNotice(User), Plugin
                );
            }
        }
        /// <summary>
        /// Changes the server notice mode
        /// </summary>
        /// <param name="value"></param>
        public void ChangeServerNotice(bool value)
        {
            if (User.IsServerNotice != value)
            {
                User.IsServerNotice = value;
                ChangeUserMode("s", value);
                Plugin.Service.SendActionToPlugins(
                    p => p.OnUserChangeServerNotice(User), Plugin
                );
            }
        }
        /// <summary>
        /// Changes the fake host
        /// </summary>
        /// <param name="newIdent"></param>
        /// <param name="newHost"></param>
        public void ChangeFakeHost(string newIdent, string newHost)
        {
            User.FakeIdent = newIdent;
            User.FakeHost = newHost;
            if (User.FakeIdent.Length > 0 && User.FakeHost.Length > 0)
            {
                ChangeUserMode("+h " + newIdent + "@" + newHost, true);           
            }
            else
            {
                ChangeUserMode("h", false);
            }
            Plugin.Service.SendActionToPlugins(
                p => p.OnUserChangeFakeHost(User), Plugin
            );
        }
        /// <summary>
        /// Changes the nick
        /// </summary>
        /// <param name="newNick"></param>
        /// <returns>TRUE if the nick is successfully changed</returns>
        public bool ChangeNick(string newNick)
        {
            if (Plugin.Service.NickExists(newNick))
            {
                return false;
            }
            User.Nick = newNick;
            var command = Plugin.Service.CommandFactory.CreateChangeNickCommand();
            command.User = User;
            Plugin.Service.SendCommand(command, false);
            Plugin.Service.SendActionToPlugins(
                p=> p.OnUserChangeNick(User), Plugin
            );
            return true;
        }
        /// <summary>
        /// Sends a private message to another user
        /// </summary>
        /// <param name="to"></param>
        /// <param name="message"></param>
        /// <returns>TRUE if the message is successfully sent</returns>
        public bool SendPrivateMessage(User to, string message)
        {
            if (to == null || to == User)
            {
                return false;
            }
            var command = Plugin.Service.CommandFactory.CreateSendMessageCommand();
            command.From = User;
            command.To = to;
            command.Message = message;
            Plugin.Service.SendCommand(command);
            return true;
        }
        /// <summary>
        /// Sends a private notice to another user
        /// </summary>
        /// <param name="to"></param>
        /// <param name="message"></param>
        /// <returns>TRUE if the message is successfully sent</returns>
        public bool SendPrivateNotice(User to, string message)
        {
            if (to == null || to == User)
            {
                return false;
            }
            var command = Plugin.Service.CommandFactory.CreateSendMessageCommand();
            command.From = User;
            command.To = to;
            command.Message = message;
            command.UseNotice = true;
            Plugin.Service.SendCommand(command);
            return true;
        }
        /// <summary>
        /// Sends a message to a channel
        /// </summary>
        /// <param name="to"></param>
        /// <param name="message"></param>
        /// <returns>TRUE if the message is successfully sent</returns>
        public bool SendChannelMessage(string to, string message)
        {
            if (String.IsNullOrEmpty(to))
            {
                return false;
            }
            var command = Plugin.Service.CommandFactory.CreateSendMessageCommand();
            command.From = User;
            command.To = to;
            command.Message = message;
            Plugin.Service.SendCommand(command);
            return true;
        }
        /// <summary>
        /// Sends a message as a notice to a channel
        /// </summary>
        /// <param name="to"></param>
        /// <param name="message"></param>
        /// <returns>TRUE if the message is successfully sent</returns>
        public bool SendChannelNotice(string to, string message)
        {
            if (String.IsNullOrEmpty(to))
            {
                return false;
            }
            var command = Plugin.Service.CommandFactory.CreateSendMessageCommand();
            command.From = User;
            command.To = to;
            command.Message = message;
            command.UseNotice = true;
            Plugin.Service.SendCommand(command);
            return true;
        }
        /// <summary>
        /// Sends a CTCP reply to a user
        /// </summary>
        /// <param name="to"></param>
        /// <param name="ctcp"></param>
        /// <param name="parameter"></param>
        /// <returns>TRUE if the reply is successfully sent</returns>
        public bool SendCTCPReply(User to, string ctcp, string parameter)
        {
            return SendPrivateNotice(to, IRCConstants.CTCP + ctcp + " " +
                parameter + IRCConstants.CTCP);
        }
        /// <summary>
        /// Sends a global message
        /// </summary>
        /// <param name="to"></param>
        /// <param name="message"></param>
        /// <returns>TRUE if the message is sent successfully</returns>
        public bool SendGlobalMessage(string to, string message)
        {
            var command = Plugin.Service.CommandFactory.CreateSendMessageCommand();
            command.From = User;
            command.To = "$" + to;
            command.Message = message;
            Plugin.Service.SendCommand(command);
            return true;
        }
        /// <summary>
        /// Disconnects another user
        /// </summary>
        /// <param name="toDisconnect"></param>
        /// <param name="reason"></param>
        /// <returns>TRUE if the user is succesfully disconnected</returns>
        public bool DisconnectUser(User toDisconnect, string reason)
        {
            if (toDisconnect == null)
            {
                return false;
            }
            var command = 
                Plugin.Service.CommandFactory.CreateUserDisconnectCommand();
            command.From = User;
            command.To = toDisconnect;
            command.Reason = reason;
            Plugin.Service.SendCommand(command);
            return true;
        }
        /// <summary>
        /// Joins a channel
        /// </summary>
        /// <param name="channelName">The channel to join</param>
        /// <param name="op">Get OP on the channel?</param>
        /// <returns>TRUE if the channel is successfully joined</returns>
        public bool JoinChannel(string channelName, bool op = false)
        {
            if (channelName.Length < 2)
            {
                return false;
            }
            if (channelName[0] != '#')
            {
                return false;
            }
            Channel channel = Plugin.Service.GetChannel(channelName);
            if (channel == null)
            {
                var command = 
                    Plugin.Service.CommandFactory.CreateJoinChannelCommand();
                command.From = User;
                command.Channel = channelName;
                command.Create = true;
                Plugin.Service.SendCommand(command);
            }
            else
            {
                var joinCommand = 
                    Plugin.Service.CommandFactory.CreateJoinChannelCommand();
                joinCommand.From = User;
                joinCommand.Channel = channelName;
                Plugin.Service.SendCommand(joinCommand);
                if (op)
                {
                    var changeModeCommand =
                        Plugin.Service.CommandFactory.CreateChangeChannelModeCommand();
                    changeModeCommand.From = User.Server;
                    changeModeCommand.Channel = channelName;
                    changeModeCommand.UseOpMode = true;
                    changeModeCommand.Modes = "+o";
                    changeModeCommand.Parameters = new object[] { User };
                    Plugin.Service.SendCommand(changeModeCommand);
                }
            }
            return true;
        }
        /// <summary>
        /// Parts a channel
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="reason"></param>
        /// <returns>TRUE if the channel is successfully parted</returns>
        public bool PartChannel(string channel, string reason)
        {
            if (channel.Length < 2)
            {
                return false;
            }
            if (channel[0] != '#')
            {
                return false;
            }
            if ( ! User.IsOnChannel(channel))
            {
                return false;
            }
            var command = 
                Plugin.Service.CommandFactory.CreatePartChannelCommand();
            command.From = User;
            command.Channel = channel;
            command.Reason = reason;
            Plugin.Service.SendCommand(command);
            return true;
        }
        /// <summary>
        /// Kicks a user from a channel
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="user"></param>
        /// <param name="reason"></param>
        /// <returns>TRUE if the user is sucessfully kicked</returns>
        public bool KickUser(string channel, User user, string reason)
        {
            if (channel.Length < 2)
            {
                return false;
            }
            if (channel[0] != '#')
            {
                return false;
            }
            if (user == null)
            {
                return false;
            }
            ChannelEntry currentEntry = User.GetChannelEntry(channel);
            if (currentEntry == null)
            {
                return false;
            }
            if ((currentEntry.Op == false) && (currentEntry.HalfOp == false))
            {
                return false;
            }
            ChannelEntry userEntry = user.GetChannelEntry(channel);
            if (userEntry == null)
            {
                return false;
            }
            if ((userEntry.HalfOp == true) && (currentEntry.HalfOp == true))
            {
                return false;
            }
            var command =
                Plugin.Service.CommandFactory.CreateKickUserCommand();
            command.From = User;
            command.User = user;
            command.Channel = channel;
            command.Reason = reason;
            Plugin.Service.SendCommand(command);
            return true;
        }
        /// <summary>
        /// Changes channel modes
        /// </summary>
        /// <param name="channel">The channel</param>
        /// <param name="useOpMode">Use opmode?</param>
        /// <param name="modes">The modes to change</param>
        /// <param name="parameters">Parameters</param>
        /// <returns></returns>
        public bool SetChannelMode(string channel, bool useOpMode, string modes,
            params object[] parameters)
        {
            if (channel == null)
            {
                return false;
            }
            if (channel[0] != '#')
            {
                return false;
            }
            ChannelEntry entry = User.GetChannelEntry(channel);
            if (entry != null)
            {
                if ((entry.Op) == false && (entry.HalfOp == false) && ! useOpMode)
                {
                    return false;
                }
            }
            var command =
                Plugin.Service.CommandFactory.CreateChangeChannelModeCommand();
            command.From = User;
            command.Channel = channel;
            command.UseOpMode = useOpMode;
            command.Modes = modes;
            command.Parameters = parameters;
            Plugin.Service.SendCommand(command);
            return true;
        }
#endregion
    }
}
