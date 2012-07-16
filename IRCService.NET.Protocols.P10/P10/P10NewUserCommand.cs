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

namespace IRCServiceNET.Protocols.P10
{
    public class P10NewUserCommand : NewUserCommand
    {
        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(User.Server.Numeric);
            builder.Append(" N ");
            builder.Append(User.Nick);
            builder.Append(" ");
            builder.Append(User.Server.Depth.ToString());
            builder.Append(" ");
            builder.Append(User.ConnectionTimestamp.ToString());
            builder.Append(" ");
            builder.Append(User.Ident);
            builder.Append(" ");
            builder.Append(User.Host);
            builder.Append(" ");
            builder.Append(String.IsNullOrEmpty(Modes) ? "+" : Modes.Trim());
            if (ModeParameters != null && ModeParameters.Count() > 0)
            {                
                foreach (var item in ModeParameters)
                {
                    builder.Append(" ");
                    builder.Append(item);
                }
            }
            builder.Append(" ");
            builder.Append(User.Base64IP);
            builder.Append(" ");
            builder.Append(User.Numeric);
            builder.Append(" :");
            builder.Append(User.Name);

            return builder.ToString();
        }
    }
}
