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

namespace IRCServiceNET.Protocols.P10.Parsers
{
    /// <summary>
    /// Parses an end of burst
    /// </summary>
    public class EB_Parser : CommandParser
    {
        public override string CommandHeader
        {
            get { return "EB"; }
        }

        public override void Parse(string[] spaceSplit, string[] colonSplit, 
            string fullRow)
        {
            Server burstServer = Service.GetServer(spaceSplit[0]);
            if (burstServer == null)
            {
                Service.AddLog("EB from unknown server " + spaceSplit[0]);
                return;
            }
            if (burstServer == Service.UpLink)
            {
                Service.AddPluginServersAndClientsToNetwork();
                Service.SendBurst();
                Service.Status = ServiceStatus.BurstCompleted;
                var command = Service.CommandFactory.CreateEndOfBurstCommand();
                command.From = Service.MainServer;
                Service.SendCommand(command);
                var command2 = Service.CommandFactory.CreateAcknowledgeBurstCommand();
                command2.From = Service.MainServer;
                Service.SendCommand(command2);

                Service.SendActionToPlugins(p => p.OnBurstCompleted());
            }
        }
    }
}
