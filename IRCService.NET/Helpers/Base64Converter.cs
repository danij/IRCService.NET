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
    /// Converts integers to Base64 Numerics 
    /// </summary>
    public class Base64Converter
    {
        /// <summary>
        /// Valid characters
        /// </summary>
        private const string base64str =
            "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789[]";
        /// <summary>
        /// Converts an integer to a Base64 numeric
        /// </summary>
        /// <param name="value"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string IntToNumeric(Int64 value, int length)
        {
            string result = "";
            if (length < 6 && value >= (MathHelper.Pow(64, length)))
            {
                return result;
            }
            Int64 q, r, pow1;
            Int64 toCalculate = value + 1;
            for (int i = length - 1; i >= 1; i--)
            {
                pow1 = MathHelper.Pow(64, i);
                q = (toCalculate / pow1);
                r = toCalculate % pow1;
                if ((q > 0) && (r == 0))
                {
                    q -= 1;
                    r = pow1;
                }
                toCalculate = r;
                result += base64str[(int)q];
                if (i == 1)
                {
                    result += base64str[(int)(r - 1)];
                }
            }
            return result;
        }
        /// <summary>
        /// Converts an integer to a Base64 numeric
        /// </summary>
        /// <param name="value"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string IntToNumeric(uint value, int length)
        {
            return IntToNumeric((int)value, length);
        }
        /// <summary>
        /// Converts a Base64 numeric to an integer value
        /// </summary>
        /// <param name="numeric"></param>
        /// <returns></returns>
        public static int NumericToInt(string numeric)
        {
            if (numeric.Length < 1)
            {
                return 0;
            }

            int result = 0;
            int a = 0;
            for (int i = numeric.Length; i >= 1; i--)
            {
                a *= 64;
                if (a == 0)
                {
                    a = 1;
                }
                int index = base64str.IndexOf(numeric[i - 1]);
                if (index > 0)
                {
                    result += index * a;                    
                }
            }
            return result;
        }
    }
}
