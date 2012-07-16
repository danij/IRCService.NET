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
    /// Provides an abstract base class for network actions
    /// </summary>
    public abstract class NetworkAction
    {
        /// <summary>
        /// The server
        /// </summary>
        protected Server Server { get; set; }
        /// <summary>
        /// The plugin that ownes the user
        /// </summary>
        protected IRCServicePlugin Plugin { get; set; }
        /// <summary>
        /// Checks if the server is controlled by a plugin
        /// </summary>
        protected virtual void CheckPlugin()
        {
            if ( ! Server.Controlled)
            {
                throw new UserNotControlledException();
            }
            if (Plugin == null)
            {
                throw new ArgumentNullException("Plugin");
            }
        }

    }
}
