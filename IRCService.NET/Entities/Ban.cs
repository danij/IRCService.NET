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

namespace IRCServiceNET.Entities
{
    /// <summary>
    /// A Channel Ban
    /// </summary>
    public class Ban
    {
        /// <summary>
        /// Constructs a Ban by specifying the nick, the ident and the host
        /// </summary>
        /// <param name="nick"></param>
        /// <param name="ident"></param>
        /// <param name="host"></param>
        public Ban(string nick, string ident, string host)
        {
            Nick = nick;
            Ident = ident;
            Host = host;
        }
        /// <summary>
        /// Constructs a Ban by specifying a ban string in the form of nick!ident@host
        /// </summary>
        /// <param name="banString"></param>
        public Ban(string banString)
        {
            Nick = "*";
            Ident = "*";
            Host = "*";

            string[] hostSplit = banString.Split('@');
            if (hostSplit.Length < 2)
            {
                Host = banString;
                return;
            }
            Host = hostSplit[1];

            string[] identSplit = hostSplit[0].Split('!');
            if (identSplit.Length < 2)
            {
                Ident = hostSplit[0];
                return;
            }
            Ident = identSplit[1];
            Nick = identSplit[0];
        }
#region Properties
        /// <summary>
        /// Gets the nick
        /// </summary>
        public string Nick { get; protected set; }
        /// <summary>
        /// Gets the ident
        /// </summary>
        public string Ident { get; protected set; }
        /// <summary>
        /// Gets the host
        /// </summary>
        public string Host { get; protected set; }
#endregion

#region Methods
        /// <summary>
        /// Checks if the ban matches a user
        /// </summary>
        /// <param name="user"></param>
        /// <returns>TRUE if the ban matches the user</returns>
        public bool Match(User user)
        {
            bool nickMatch = WildCardHelper.WildCardMatch(Nick, user.Nick);
            bool identMatch = WildCardHelper.WildCardMatch(Ident, user.Ident);
            bool hostMatch = WildCardHelper.WildCardMatch(Host, user.Host);

            if (nickMatch)
            {
                if ( ! identMatch)
                {
                    if (user.FakeIdent.Length > 0)
                    {
                        identMatch = 
                            WildCardHelper.WildCardMatch(Ident, user.FakeIdent);
                    }
                }
                if (identMatch)
                {
                    if ( ! hostMatch)
                    {
                        if (user.FakeHost.Length > 0)
                        {
                            hostMatch = 
                                WildCardHelper.WildCardMatch(Host, user.FakeHost);
                        }
                        if ( ! hostMatch)
                        {
                            hostMatch = WildCardHelper.WildCardMatch(
                                Host,
                                user.IP.ToString()
                            );
                        }
                        if ( ! hostMatch && user.Login.Length > 0)
                        {
                            hostMatch = WildCardHelper.WildCardMatch(
                                Host,
                                user.LoginHostString
                            );
                        }
                    }
                }
            }
            if (nickMatch == true && identMatch == true && hostMatch == true)
            {
                return true;
            }
            return false;
        }
        /// <summary>
        /// Matches the ban agains another ban
        /// </summary>
        /// <param name="ban"></param>
        /// <returns>TRUE if the two bans match</returns>
        public bool Match(Ban ban)
        {
            bool nickMatch = WildCardHelper.WildCardMatch(Nick, ban.Nick);
            bool identMatch = WildCardHelper.WildCardMatch(Ident, ban.Ident);
            bool hostMatch = WildCardHelper.WildCardMatch(Host, ban.Host);

            return nickMatch && identMatch && hostMatch;
        }
        /// <summary>
        /// Matches the ban agains another ban specified as a ban string
        /// </summary>
        /// <param name="banString"></param>
        /// <returns>TRUE if the two bans match</returns>
        public bool Match(string banString)
        {
            Ban ban = new Ban(banString);
            return Match(ban);
        }

        /// <summary>
        /// Constructs a ban string in the form of nick!ident@host
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Nick + "!" + Ident + "@" + Host;
        }

#endregion


    }
}
