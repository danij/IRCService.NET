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
        public UserNotControlledException() : 
            base("The user is not under your plugin's control") { }
        public UserNotControlledException(string message) : base(message) { }
    }

    [Serializable]
    public class NotPreparedForPluginsException : Exception
    {
        public NotPreparedForPluginsException() :
            base("The Service is not prepared for plugins") { }
    }

    [Serializable]
    public class CannotRegisterPluginException : Exception
    {
        public CannotRegisterPluginException() :
            base("You must register a plugin when the service is disconnected")
        {
        }
    }

    [Serializable]
    public class BurstCompletedException : Exception
    {
        public BurstCompletedException() :
            base("The Burst has already been completed") { }
    }
}
