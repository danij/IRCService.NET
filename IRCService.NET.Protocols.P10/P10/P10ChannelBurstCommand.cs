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

namespace IRCServiceNET.Protocols.P10
{
    public class P10ChannelBurstCommand : ChannelBurstCommand
    {
        public override string ToString()
        {
            if (BurstTimestamp.Timestamp < 1)
            {
                BurstTimestamp = UnixTimestamp.CurrentTimestamp();
            }

            StringBuilder headerBuilder = new StringBuilder();
            headerBuilder.Append(Server.Numeric);
            headerBuilder.Append(" B ");
            headerBuilder.Append(Channel.Name);
            headerBuilder.Append(" ");
            headerBuilder.Append(BurstTimestamp.ToString());
            headerBuilder.Append(" ");

            StringBuilder builder = new StringBuilder();

            StringBuilder currentBuilder = new StringBuilder();

            ChannelEntry[] entries = Channel.Entries.ToArray();
            Array.Sort(entries, (entry1, entry2) =>
            {
                int entry1Coeff = 0;
                int entry2Coeff = 0;
                if (entry1.Voice) { entry1Coeff += 1; }
                if (entry1.HalfOp) { entry1Coeff += 2; }
                if (entry1.Op) { entry1Coeff += 4; }
                if (entry2.Voice) { entry2Coeff += 1; }
                if (entry2.HalfOp) { entry2Coeff += 2; }
                if (entry2.Op) { entry2Coeff += 4; }
                return Math.Sign(entry1Coeff - entry2Coeff);
            });

            string lastEntry = "";
            foreach (ChannelEntry entry in entries)
            {
                if (currentBuilder.Length < 1)
                {
                    currentBuilder.Append(headerBuilder.ToString());
                }

                string userStatus = "";
                if (entry.Voice) { userStatus += "v"; }
                if (entry.HalfOp) { userStatus += "h"; }
                if (entry.Op) { userStatus += "o"; }

                currentBuilder.Append(entry.User.Numeric);
                if ((userStatus != "") && (userStatus != lastEntry))
                {
                    currentBuilder.Append(":");
                    currentBuilder.Append(userStatus);
                    lastEntry = userStatus;
                }
                currentBuilder.Append(",");

                if (currentBuilder.Length > 400)
                {
                    currentBuilder[currentBuilder.Length - 1] = '\n';
                    builder.Append(currentBuilder.ToString());
                    currentBuilder.Clear();
                    lastEntry = "";
                }
            }
            if (currentBuilder.Length > 0)
            {
                currentBuilder[currentBuilder.Length - 1] = '\n';
                builder.Append(currentBuilder.ToString());
            }

            return builder.ToString();
        }
    }
}
