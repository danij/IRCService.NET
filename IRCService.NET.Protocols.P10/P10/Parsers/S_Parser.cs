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
using IRCServiceNET.Helpers;

namespace IRCServiceNET.Protocols.P10.Parsers
{
    /// <summary>
    /// Parses a new server introduction
    /// </summary>
    public class S_Parser : CommandParser
    {
        public override string CommandHeader
        {
            get { return "S"; }
        }

        public override void Parse(string[] spaceSplit, string[] colonSplit,
            string fullRow)
        {
            if (spaceSplit.Count() < 10)
            {
                return;
            }
            Server upLink = Service.GetServer(spaceSplit[0]);
            if (upLink == null)
            {
                Service.AddLog("S from unknown server " + spaceSplit[0]);
                return;
            }
            string numeric = spaceSplit[7].Substring(0, 2);
            if (Service.ContainsServer(numeric))
            {
                Service.AddLog("Numeric Collision on server " + numeric);
                return;
            }
            string description = "";
            if (colonSplit.Count() > 1)
            {
                description = StringHelper.JoinArray(colonSplit, ":", 1);
            }

            Server server =
                new Server(Service, numeric, spaceSplit[2], description,
                    new UnixTimestamp(Convert.ToInt32(spaceSplit[4])), 
                    Base64Converter.NumericToInt(spaceSplit[7].Substring(2, 3)),
                    false, upLink);
            Service.AddServer(server);
            Service.SendActionToPlugins(p => p.OnNewServer(server));

            return;
        }
    }
}
