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
    /// Math Helper
    /// </summary>
    public class MathHelper
    {
        /// <summary>
        /// Fast power calculator for integers
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static int Pow(int x, int y)
        {
            if (y <= 0)
            {
                return 1;
            }
            if (y == 1)
            {
                return x;
            }
            int rez = x;
            for (int i = 1; i < y; i++)
            {
                rez *= x;
            }
            return rez;
        }
        /// <summary>
        /// Fast logarithm calculator for integers
        /// </summary>
        /// <param name="value"></param>
        /// <param name="baseValue"></param>
        /// <returns></returns>
        public static int Log(int value, int baseValue = 2)
        {
            int a;
            for (int i = 0; i < 32; i++)
            {
                a = 1;
                for (int ii = 0; ii < i; ii++)
                {
                    a *= baseValue;
                }
                if (a == value)
                {
                    return i;
                }
            }
            return 0;
        }
    }
}
