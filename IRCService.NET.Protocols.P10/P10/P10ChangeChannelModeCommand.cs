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

namespace IRCServiceNET.Protocols.P10
{
    public class P10ChangeChannelModeCommand : ChangeChannelModeCommand
    {
        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();            
            builder.Append(From.Numeric);
            builder.Append(" ");
            builder.Append(UseOpMode ? "OM " : "M ");
            builder.Append(Channel);
            builder.Append(" ");
            builder.Append(Modes);
            if (Parameters != null && Parameters.Count() > 0)
            {
                foreach (var item in Parameters)
                {
                    if (item != null)
                    {
                        builder.Append(" ");
                        if (item is IHasNumeric)
                        {
                            builder.Append((item as IHasNumeric).Numeric);
                        }
                        else if (item is Ban)
	                    {
                            builder.Append((item as Ban).ToString());
	                    }
                        else if (item is string)
                        {
                            builder.Append(item as string);
                        }
                    }
                }
            }
            return builder.ToString();
        }
    }
}
