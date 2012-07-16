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

namespace IRCServiceNET.Protocols.P10.Parsers
{
    /// <summary>
    /// Base class for a command parser
    /// </summary>
    public abstract class CommandParser
    {
        /// <summary>
        /// Gets or sets the main parser
        /// </summary>
        public Parser Parser { get; set; }
        /// <summary>
        /// Gets the irc service
        /// </summary>
        public IRCService Service 
        {
            get { return Parser.Service; }
        }
        /// <summary>
        /// Gets the command header
        /// </summary>
        public abstract string CommandHeader { get; }
        /// <summary>
        /// Parses the command
        /// </summary>
        /// <param name="spaceSplit"></param>
        /// <param name="colonSplit"></param>
        /// <param name="fullRow"></param>
        public abstract void Parse(string[] spaceSplit, string[] colonSplit,
            string fullRow);
    }
}
