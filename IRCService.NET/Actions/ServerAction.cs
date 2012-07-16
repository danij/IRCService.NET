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
using IRCServiceNET.Entities;
using IRCServiceNET.Plugins;

namespace IRCServiceNET.Actions
{
    /// <summary>
    /// A class that contains methods that correspond to network actions 
    /// that a server can perform
    /// </summary>
    public class ServerAction : NetworkAction
    {
        public ServerAction(Server server)
        {
            if (server == null)
            {
                throw new ArgumentNullException("Server");
            }
            Server = server;
            Plugin = server.Plugin;
            CheckPlugin();
        }
        /// <summary>
        /// Sends a private message from the server
        /// </summary>
        /// <param name="user"></param>
        /// <param name="message"></param>
        /// <returns>TRUE if the message is successfully sent</returns>
        public bool SendPrivateMessage(User user, string message)
        {
            if (user == null)
            {
                return false;
            }
            var command = 
                Plugin.Service.CommandFactory.CreateSendMessageCommand();
            command.From = Server;
            command.To = user;
            command.Message = message;

            Plugin.Service.SendCommand(command);
            return true;
        }
        /// <summary>
        /// Sends a notice from the server
        /// </summary>
        /// <param name="server"></param>
        /// <param name="user"></param>
        /// <param name="message"></param>
        /// <returns>TRUE if the notice is successfully sent</returns>
        public bool SendNotice(User user, string message)
        {
            if (user == null)
            {
                return false;
            }
            var command =
                Plugin.Service.CommandFactory.CreateSendMessageCommand();
            command.From = Server;
            command.To = user;
            command.Message = message;
            command.UseNotice = true;

            Plugin.Service.SendCommand(command);
            return true;
        }
        /// <summary>
        /// Logs a user in
        /// </summary>
        /// <param name="toLogIn"></param>
        /// <param name="login"></param>
        /// <returns>TRUE if the login is successfull</returns>
        public bool LoginUser(User toLogIn, string login)
        {
            if (toLogIn == null)
            {
                return false;
            }
            if (toLogIn.Login.Length > 0)
            {
                return false;
            }
            var command =
                Plugin.Service.CommandFactory.CreateAuthenticateUserCommand();
            command.From = Server;
            command.To = toLogIn;
            command.Username = login;

            Plugin.Service.SendCommand(command);
            return true;
        }

    }
}
