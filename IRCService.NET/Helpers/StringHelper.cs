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

namespace IRCServiceNET.Helpers
{
    /// <summary>
    /// String Helper
    /// </summary>
    public class StringHelper
    {
        /// <summary>
        /// Joins a string array to a string
        /// </summary>
        /// <param name="toJoin">The array to join</param>
        /// <param name="delimiter">The delimiter to use</param>
        /// <param name="start">The index from where to start</param>
        /// <returns></returns>
        public static string JoinArray(string[] toJoin, string delimiter, 
            int start = 0)
        {
            string result = "";
            if (start >= toJoin.Count())
            {
                return result;
            }
            if (start == (toJoin.Count() - 1))
            {
                result = toJoin[start];
                return result;
            }
            for (int i = start; i < toJoin.Count() - 1; i++)
            {
                result += toJoin[i] + delimiter;
            }
            result += toJoin[toJoin.Count() - 1];
            return result;
        }
    }
}
