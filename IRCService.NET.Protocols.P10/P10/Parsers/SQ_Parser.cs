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
using IRCServiceNET.Helpers;
using IRCServiceNET.Entities;

namespace IRCServiceNET.Protocols.P10.Parsers
{
    /// <summary>
    /// Parses a server disconnection
    /// </summary>
    public class SQ_Parser : CommandParser
    {
        public override string CommandHeader
        {
            get { return "SQ"; }
        }

        public override void Parse(string[] spaceSplit, string[] colonSplit, 
            string fullRow)
        {
            if (spaceSplit.Count() < 3)
            {
                return;
            }
            string reason = "";
            if (colonSplit.Count() > 1)
            {
                reason = StringHelper.JoinArray(colonSplit, ":", 1);
            }
            if (spaceSplit[2].IndexOf('.') == -1)
            {
                Service.RemoveServer(spaceSplit[2], reason);
            }
            else
            {
                Server serverToRemove = Service.GetServerByName(spaceSplit[2]);
                if (serverToRemove == null)
                {
                    return;
                }
                Service.RemoveServer(serverToRemove, reason);
            }
        }
    }
}
