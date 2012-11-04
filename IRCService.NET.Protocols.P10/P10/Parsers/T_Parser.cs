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
    /// Parses a channel topic 
    /// </summary>
    public class T_Parser : CommandParser
    {
        public override string CommandHeader
        {
            get { return "T"; }
        }

        public override void Parse(string[] spaceSplit, string[] colonSplit,
            string fullRow)
        {
            if (spaceSplit.Length < 5)
            {
                return;
            }

            var channel = Service.GetChannel(spaceSplit[2]) as Channel;

            if (channel == null)
            {
                Service.AddLog("Topic change for unknown channel " + spaceSplit[2]);
                return;
            }

            string value = "";
            if (colonSplit.Length > 1)
            {
                value = StringHelper.JoinArray(colonSplit, ":", 1);
            }

            IHasNumeric setBy = null;
            if (spaceSplit[0].Length == 5)
            {
                setBy = Service.GetUser(spaceSplit[0]);
            }
            else
            {
                setBy = Service.GetServer(spaceSplit[0]);
            }

            int setTS = 0;
            Int32.TryParse(spaceSplit[4], out setTS);
            DateTime setAt = UnixTimestamp.DateTimeFromTimestamp(setTS);

            channel.Topic = new ChannelTopic(setBy, value, setAt);
            Service.SendActionToPlugins(p => p.OnChannelTopicChange(channel));
        }
    }
}
