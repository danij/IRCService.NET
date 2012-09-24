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

namespace IRCServiceNET
{
    public class IRCConstants
    {
        public const char CTCP = (char)1;
        public const char BOLD = (char)2;
        public const char COLOR = (char)3;
        public const char REVERSE = (char)0x16;
        public const char UNDERLINE = (char)0x1F;
    }
    /// <summary>
    /// 4-bit IRC Colors
    /// </summary>
    public enum IRCColor
    {
        White = 0,
        Black = 1,
        Blue = 2,
        Green = 3,
        Red = 4,
        Brown = 5,
        Violet = 6,
        Orange = 7,
        Yellow = 8,
        LightGreen = 9,
        Teal = 10,
        Aqua = 11,
        LighBlue = 12,
        Pink = 13,
        DarkGrey = 14,
        LightGrey = 15
    }
}
