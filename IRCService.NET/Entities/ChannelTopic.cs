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

namespace IRCServiceNET.Entities
{
    /// <summary>
    /// A channels topic
    /// </summary>
    public class ChannelTopic
    {
        /// <summary>
        /// Gets or sets the value
        /// </summary>
        public string Value { get; protected set; }
        /// <summary>
        /// Gets or sets when the topic was set
        /// </summary>
        public DateTime DateTime { get; protected set; }
        /// <summary>
        /// Gets or sets who set the topic
        /// </summary>
        public IHasNumeric SetBy { get; protected set; }
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="channel"></param>
        public ChannelTopic(IHasNumeric setBy, string value, DateTime dateTime)
        {
            SetBy = setBy;
            Value = value;
            DateTime = dateTime;
        }
    }
}
