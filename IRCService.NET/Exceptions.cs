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
using System.Runtime.Serialization;

namespace IRCServiceNET
{
    [Serializable]
    public class UserNotControlledException : Exception
    {
        public UserNotControlledException() :
            base("The user is not under your plugin's control"){ }
        public UserNotControlledException(string message) : base(message) { }
        public UserNotControlledException(string message, Exception inner) : 
            base(message, inner) { }
        protected UserNotControlledException(
          SerializationInfo info,
          StreamingContext context)
            : base(info, context) { }
    }

    [Serializable]
    public class NotPreparedForPluginsException : Exception
    {
        public NotPreparedForPluginsException() :
            base("The Service is not prepared for plugins") { }
        public NotPreparedForPluginsException(string message) : base(message) { }
        public NotPreparedForPluginsException(string message, Exception inner) : 
            base(message, inner) { }
        protected NotPreparedForPluginsException(
          SerializationInfo info,
          StreamingContext context)
            : base(info, context) { }
    }

    [Serializable]
    public class CannotRegisterPluginException : Exception
    {
        public CannotRegisterPluginException() :
            base("You must register a plugin when the service is disconnected") { }
        public CannotRegisterPluginException(string message) : base(message) { }
        public CannotRegisterPluginException(string message, Exception inner) :
            base(message, inner) { }
        protected CannotRegisterPluginException(
          SerializationInfo info,
          StreamingContext context)
            : base(info, context) { }
    }

    [Serializable]
    public class BurstCompletedException : Exception
    {
        public BurstCompletedException() :
            base("The Burst has already been completed") { }
        public BurstCompletedException(string message) : base(message) { }
        public BurstCompletedException(string message, Exception inner) : 
            base(message, inner) { }
        protected BurstCompletedException(
          SerializationInfo info,
          StreamingContext context)
            : base(info, context) { }
    }

    [Serializable]
    public class NickExistsException : InvalidOperationException
    {
        public NickExistsException() :
            base("The nick already exists on the network") { }
        public NickExistsException(string message) : base(message) { }
        public NickExistsException(string message, Exception inner) : 
            base(message, inner) { }
        protected NickExistsException(
          SerializationInfo info,
          StreamingContext context)
            : base(info, context) { }
    }

    [Serializable]
    public class NotAChannelOperatorException : InvalidOperationException
    {
        public NotAChannelOperatorException() :
            base("Only a channel operator can perform that action") { }
        public NotAChannelOperatorException(string message) : base(message) { }
        public NotAChannelOperatorException(string message, Exception inner) :
            base(message, inner) { }
        protected NotAChannelOperatorException(
          SerializationInfo info,
          StreamingContext context)
            : base(info, context) { }
    }

    [Serializable]
    public class NotAnIRCOperatorException : InvalidOperationException
    {
        public NotAnIRCOperatorException() :
            base("Only an IRC Operator can perform that action") { }
        public NotAnIRCOperatorException(string message) : base(message) { }
        public NotAnIRCOperatorException(string message, Exception inner) : 
            base(message, inner) { }
        protected NotAnIRCOperatorException(
          SerializationInfo info,
          StreamingContext context)
            : base(info, context) { }
    }

    [Serializable]
    public class NotAuthenticatedException : InvalidOperationException
    {
        public NotAuthenticatedException() :
            base("Only an authenticated user can perform that action") { }
        public NotAuthenticatedException(string message) : base(message) { }
        public NotAuthenticatedException(string message, Exception inner) : 
            base(message, inner) { }
        protected NotAuthenticatedException(
          SerializationInfo info,
          StreamingContext context)
            : base(info, context) { }
    }

    [Serializable]
    public class NotOnChannelException : InvalidOperationException
    {
        public NotOnChannelException() :
            base("Cannot perform the action without being on the channel") { }
        public NotOnChannelException(string message) : base(message) { }
        public NotOnChannelException(string message, Exception inner) : 
            base(message, inner) { }
        protected NotOnChannelException(
          SerializationInfo info,
          StreamingContext context)
            : base(info, context) { }
    }

    [Serializable]
    public class UserAlreadyAuthenticatedException : InvalidOperationException
    {
        public UserAlreadyAuthenticatedException() :
            base("The user is already authenticated") { }
        public UserAlreadyAuthenticatedException(string message) : base(message) { }
        public UserAlreadyAuthenticatedException(string message, Exception inner) 
            : base(message, inner) { }
        protected UserAlreadyAuthenticatedException(
          SerializationInfo info,
          StreamingContext context)
            : base(info, context) { }
    }

    [Serializable]
    public class ChannelModeratedException : InvalidOperationException
    {
        public ChannelModeratedException() :
            base("At least voice is needed to send messages to a moderated channel") { }
        public ChannelModeratedException(string message) : base(message) { }
        public ChannelModeratedException(string message, Exception inner) : 
            base(message, inner) { }
        protected ChannelModeratedException(
          SerializationInfo info,
          StreamingContext context)
            : base(info, context) { }
    }

    [Serializable]
    public class ChannelLimitException : InvalidOperationException
    {
        public ChannelLimitException() :
            base("The maximum number of users is already on that channel") { }
        public ChannelLimitException(string message) : base(message) { }
        public ChannelLimitException(string message, Exception inner) : 
            base(message, inner) { }
        protected ChannelLimitException(
          SerializationInfo info,
          StreamingContext context)
            : base(info, context) { }
    }

    [Serializable]
    public class CannotKickServiceException : InvalidOperationException
    {
        public CannotKickServiceException() :
            base("Cannot kick a channel service from a channel") { }
        public CannotKickServiceException(string message) : base(message) { }
        public CannotKickServiceException(string message, Exception inner) : 
            base(message, inner) { }
        protected CannotKickServiceException(
          SerializationInfo info,
          StreamingContext context)
            : base(info, context) { }
    }

    [Serializable]
    public class NoMessageException : ArgumentException
    {
        public NoMessageException() :
            base("No message was specified") { }
        public NoMessageException(string message) : base(message) { }
        public NoMessageException(string message, Exception inner) : 
            base(message, inner) { }
        protected NoMessageException(
          SerializationInfo info,
          StreamingContext context)
            : base(info, context) { }
    }

    [Serializable]
    public class InvalidChannelException : ArgumentException
    {
        public InvalidChannelException() : base("Invalid channel") { }
        public InvalidChannelException(string message) : base(message) { }
        public InvalidChannelException(string message, Exception inner) : 
            base(message, inner) { }
        protected InvalidChannelException(
          SerializationInfo info,
          StreamingContext context)
            : base(info, context) { }
    }

    [Serializable]
    public class InvalidChannelKeyException : ArgumentException
    {
        public InvalidChannelKeyException() :
            base("The specified key does not match the channel's key") { }
        public InvalidChannelKeyException(string message) : base(message) { }
        public InvalidChannelKeyException(string message, Exception inner) : 
            base(message, inner) { }
        protected InvalidChannelKeyException(
          SerializationInfo info,
          StreamingContext context)
            : base(info, context) { }
    }

    [Serializable]
    public class BannedFromChannelException : ArgumentException
    {
        public BannedFromChannelException() :
            base("The user is banned from that channel") { }
        public BannedFromChannelException(string message) : base(message) { }
        public BannedFromChannelException(string message, Exception inner) : 
            base(message, inner) { }
        protected BannedFromChannelException(
          SerializationInfo info,
          StreamingContext context)
            : base(info, context) { }
    }

    [Serializable]
    public class CannotUseColorsOnChannelException : InvalidOperationException
    {
        public CannotUseColorsOnChannelException() :
            base("Colors are not allowed on that channel") { }
        public CannotUseColorsOnChannelException(string message) : base(message) { }
        public CannotUseColorsOnChannelException(string message, Exception inner) :
            base(message, inner) { }
        protected CannotUseColorsOnChannelException(
          SerializationInfo info,
          StreamingContext context)
            : base(info, context) { }
    }

    [Serializable]
    public class CannotSendCTCPToChannelException : InvalidOperationException
    {
        public CannotSendCTCPToChannelException() :
            base("CTCPs or not allowed on that channel") { }
        public CannotSendCTCPToChannelException(string message) : base(message) { }
        public CannotSendCTCPToChannelException(string message, Exception inner) :
            base(message, inner) { }
        protected CannotSendCTCPToChannelException(
          SerializationInfo info,
          StreamingContext context)
            : base(info, context) { }
    }

    [Serializable]
    public class UserAlreadyOnChannelException : InvalidOperationException
    {
        public UserAlreadyOnChannelException() :
            base("The user is already on that channel") { }
        public UserAlreadyOnChannelException(string message) : base(message) { }
        public UserAlreadyOnChannelException(string message, Exception inner) : base(message, inner) { }
        protected UserAlreadyOnChannelException(
          SerializationInfo info,
          StreamingContext context)
            : base(info, context) { }
    }

    [Serializable]
    public class CommandTooLongException : Exception
    {
        public CommandTooLongException() { }
        public CommandTooLongException(string message) : base(message) { }
        public CommandTooLongException(string message, Exception inner) : base(message, inner) { }
        protected CommandTooLongException(
          SerializationInfo info,
          StreamingContext context)
            : base(info, context) { }
    }

}
