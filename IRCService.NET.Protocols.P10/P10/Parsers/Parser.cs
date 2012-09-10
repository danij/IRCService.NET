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
    /// P10 protocol parser
    /// </summary>
    public class Parser : IParser
    {
        /// <summary>
        /// Parsers for each type of command
        /// </summary>
        private Dictionary<string, CommandParser> parsers;
        /// <summary>
        /// Default constructor
        /// </summary>
        public Parser()
        {
            parsers = new Dictionary<string, CommandParser>();

            var commandParserType = typeof(CommandParser);
            var parserTypes = 
                AppDomain.CurrentDomain.GetAssemblies().AsQueryable()
                .SelectMany(s => s.GetTypes())
                .Where(p => commandParserType.IsAssignableFrom(p))
                .Where(p => !p.IsInterface && !p.IsAbstract);

            foreach (var item in parserTypes)
            {
                CommandParser parser = 
                    (CommandParser)Activator.CreateInstance(item);
                parser.Parser = this;
                if ( ! parsers.ContainsKey(parser.CommandHeader))
                {
                    parsers.Add(parser.CommandHeader, parser);
                }
            }
            parsers.Add("OM", new M_Parser { Parser = this, OpMode = true });
        }
        /// <summary>
        /// Gets or sets the irc service
        /// </summary>
        public IRCService Service { get; set; }
        /// <summary>
        /// Processes incomming data
        /// </summary>
        /// <param name="data"></param>
        public void Process(string data)
        {
            string[] spaceSplit = data.Split(' ');
            string[] colonSplit = data.Split(':');

            if (spaceSplit[0] == "SERVER")
            {
                parsers[spaceSplit[0]].Parse(spaceSplit, colonSplit, data);
            }
            else if (spaceSplit[0] == "ERROR")
            {
                parsers[spaceSplit[0]].Parse(spaceSplit, colonSplit, data);
            }
            else if (spaceSplit.Count() > 1)
            {
                if (parsers.ContainsKey(spaceSplit[1]))
                {
                    parsers[spaceSplit[1]].Parse(spaceSplit, colonSplit, data);
                }
            }
        }        
    }
}
