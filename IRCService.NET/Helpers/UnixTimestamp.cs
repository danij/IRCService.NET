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
    /// Converts a DateTime to and from a Unix timestamp 
    /// (number of seconds since 1-1-1970)
    /// </summary>
    public class UnixTimestamp
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public UnixTimestamp()
        {
            Timestamp = 0;
        }
        /// <summary>
        /// Constructs a UnixTimestamp from an integer value
        /// </summary>
        /// <param name="timestamp"></param>
        public UnixTimestamp(int timestamp)
        {
            Timestamp = timestamp;
        }
        /// <summary>
        /// Constructs a UnixTimestamp from a DateTime value
        /// </summary>
        /// <param name="dateTime"></param>
        public UnixTimestamp(DateTime dateTime)
        {
            DateTime = dateTime;
        }
        /// <summary>
        /// Gets or sets the timestamp
        /// </summary>
        public int Timestamp { get; set; }
        /// <summary>
        /// Gets or sets the timestamp
        /// </summary>
        public DateTime DateTime
        {
            get
            {
                return DateTimeFromTimestamp(Timestamp);
            }
            set
            {
                Timestamp = TimestampFromDateTime(value);
            }        
        }
        /// <summary>
        /// Converts a UnixTimestamp to a string
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Timestamp.ToString();
        }
        /// <summary>
        /// Calculates the current timestamp
        /// </summary>
        /// <returns></returns>
        public static UnixTimestamp CurrentTimestamp()
        {
            return new UnixTimestamp(
                (int)(DateTime.UtcNow - new DateTime(1970,1,1,0,0,0)).TotalSeconds
            );
        }
        /// <summary>
        /// Converts a DateTime to a timestamp
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int TimestampFromDateTime(DateTime value)
        {
            return (int)(value - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds;
        }
        /// <summary>
        /// Converts a timestamp to a DateTime
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static DateTime DateTimeFromTimestamp(int value)
        {
            DateTime result = new DateTime(1970, 1, 1, 0, 0, 0);
            result = result.AddSeconds(value);
            result = result.Add(DateTime.Now - DateTime.UtcNow);
            return result;
        }
    }
}
