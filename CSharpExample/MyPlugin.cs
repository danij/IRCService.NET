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
using IRCServiceNET;
using IRCServiceNET.Entities;
using System.Net;

namespace CSharpExample
{
    /// <summary>
    /// A sample plugin
    /// </summary>
    public class MyPlugin : IRCServicePlugin
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="service"></param>
        public MyPlugin(IRCService service) : base(service) { }
        /// <summary>
        /// Gets a simple bot
        /// </summary>
        public IUser Bot { get; private set; }
        /// <summary>
        /// Creates the bot
        /// </summary>
        public void CreateBot()
        {
            if (Service.GetUserByNick("bot") == null)
            {
                Bot = CreateUser(
                    Service.MainServer,
                    "bot",
                    "~bot",
                    "localhost",
                    "A simple bot",
                    IPAddress.Parse("127.0.0.1")
               );
            }
        }
        /// <summary>
        /// Occurs when the service is synced with the network
        /// </summary>
        public override void OnBurstCompleted()
        {
            base.OnBurstCompleted();

            Bot.Action.ChangeOper(true);
            Bot.Action.JoinChannel("#channel");
            Bot.Action.SendChannelNotice("#channel", "Hello everyone :)");
        }
        /// <summary>
        /// Occurs when a private message is received
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="message"></param>
        public override void OnPrivateMessage(IUser from, IUser to, 
            string message)
        {
            base.OnPrivateMessage(from, to, message);

            if (to == Bot)
            {
                if (message.Trim().ToLower() == "hello")
                {
                    to.Action.SendPrivateMessage(from, "Hi there!");
                }
            }
        }
    }
}
