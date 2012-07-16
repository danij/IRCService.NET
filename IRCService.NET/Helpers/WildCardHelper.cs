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
    /// A helper class that deals with wildcards
    /// </summary>
    public class WildCardHelper
    {
        /// <summary>
        /// Matches a string against a wildcard
        /// </summary>
        /// <param name="wildcardString"></param>
        /// <param name="toMatch"></param>
        /// <returns></returns>
        public static bool WildCardMatch(string wildcardString, string toMatch)
        {
            string wildcards = "";
            bool asterix = false;
            if (wildcardString == "*")
            {
                return true;
            }
            for (int i = 0; i < wildcardString.Length; i++)
            {
                if (wildcardString[i] == '*')
                {
                    if (asterix == false)
                    {
                        wildcards += "**";
                    }
                    asterix = true;
                }
                else
                {
                    wildcards += wildcardString[i];
                    asterix = false;
                }
            }
            if (wildcards == "*")
            {
                return true;
            }
            string[] entries = wildcards.Split('*');
            string toMatchLeft = toMatch;
            string toFind = "";
            int foundAt;

            for (int i = 0; i < entries.Length; i++)
            {
                if ((entries[i].Length == 0) && (i < entries.Length - 1))
                {
                    toFind = entries[i + 1];
                    if (toFind.IndexOf('?') == -1)
                    {
                        foundAt = toMatchLeft.IndexOf(toFind);
                    }
                    else
                    {
                        foundAt = -1;
                        for (int ii = 0; ii <= toMatchLeft.Length - toFind.Length;
                            ii++)
                        {
                            if (WildCardInternalMatch(toMatchLeft, toFind, ii))
                            {
                                foundAt = ii;
                                break;
                            }
                        }
                    }
                    if (foundAt == -1)
                    {
                        return false;
                    }
                    toMatchLeft = toMatchLeft.Substring(foundAt);
                }
                else
                {
                    if ((entries[i].Length == 0) && (i == entries.Length - 1))
                    {
                        continue;
                    }
                    toFind = entries[i];
                    if (i == entries.Length - 1)
                    {
                        if (toFind.Length != toMatchLeft.Length)
                        {
                            return false;
                        }
                    }
                    if (!WildCardInternalMatch(toMatchLeft, toFind, 0))
                    {
                        return false;
                    }

                }
            }
            return true;
        }
        /// <summary>
        /// Matches a string against the ? wildcard
        /// </summary>
        /// <param name="toMatch"></param>
        /// <param name="toFind"></param>
        /// <param name="startIndex"></param>
        /// <returns></returns>
        private static bool WildCardInternalMatch(string toMatch, string toFind, 
            int startIndex)
        {
            if (toMatch.Length - startIndex < toFind.Length)
            {
                return false;
            }
            for (int i = 0; i < toFind.Length; i++)
            {
                if (toFind[i] == '?')
                {
                    continue;
                }
                if (toFind[i] != toMatch[i + startIndex])
                {
                    return false;
                }
            }
            return true;
        }
    }
}
