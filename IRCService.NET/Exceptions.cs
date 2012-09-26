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

namespace IRCServiceNET
{
    [Serializable]
    public class UserNotControlledException : Exception
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public UserNotControlledException() : 
            base("The user is not under your plugin's control") { }
        /// <summary>
        /// Custom constructor
        /// </summary>
        /// <param name="message"></param>
        public UserNotControlledException(string message) : base(message) { }
    }

    [Serializable]
    public class NotPreparedForPluginsException : Exception
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public NotPreparedForPluginsException() :
            base("The Service is not prepared for plugins") { }
    }

    [Serializable]
    public class CannotRegisterPluginException : Exception
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public CannotRegisterPluginException() :
            base("You must register a plugin when the service is disconnected")
        {
        }
    }

    [Serializable]
    public class BurstCompletedException : Exception
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public BurstCompletedException() :
            base("The Burst has already been completed") { }
    }

    [Serializable]
    public class NickExistsException : InvalidOperationException
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="nick"></param>
        public NickExistsException(string nick) :
            base("The nick " + nick + " already exists on the network") { }
    }

    [Serializable]
    public class NotAChannelOperatorException : InvalidOperationException
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public NotAChannelOperatorException() :
            base("Only a channel operator can perform that action") { }
    }

    [Serializable]
    public class NotAnIRCOperatorException : InvalidOperationException
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public NotAnIRCOperatorException() :
            base("Only an IRC Operator can perform that action") { }
    }

    [Serializable]
    public class NotAuthenticatedException : InvalidOperationException
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public NotAuthenticatedException() :
            base("Only an authenticated user can perform that action") { }
    }

    [Serializable]
    public class NotOnChannelException : InvalidOperationException
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public NotOnChannelException() :
            base("Cannot perform the action without being on the channel") { }        
    }

    [Serializable]
    public class UserAlreadyAuthenticatedException : InvalidOperationException
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public UserAlreadyAuthenticatedException() :
            base("The user is already authenticated") { }
            
    }

    [Serializable]
    public class ChannelModeratedException : InvalidOperationException
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public ChannelModeratedException() :
            base("At least voice is needed to send messages to a moderated channel") { }
    }

    [Serializable]
    public class ChannelLimitException : InvalidOperationException
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public ChannelLimitException() :
            base("The maximum number of users is already on that channel") { }
    }

    [Serializable]
    public class NoMessageException : ArgumentException
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public NoMessageException() :
            base("No message was specified") { }
    }

    [Serializable]
    public class InvalidChannelException : ArgumentException
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public InvalidChannelException() :
            base("Invalid channel") { }
    }

    [Serializable]
    public class InvalidChannelKeyException : ArgumentException
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public InvalidChannelKeyException() :
            base("The specified key does not match the channel's key") { }
    }

    [Serializable]
    public class BannedFromChannelException : ArgumentException
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public BannedFromChannelException() :
            base("The user is banned from that channel") { }
    }

    [Serializable]
    public class CannotUseColorsOnChannelException : InvalidOperationException
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public CannotUseColorsOnChannelException() :
            base("Colors are not allowed on that channel") { }
    }

    [Serializable]
    public class CannotSendCTCPToChannelException : InvalidOperationException
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public CannotSendCTCPToChannelException() :
            base("CTCPs or not allowed on that channel") { }
    }
}
