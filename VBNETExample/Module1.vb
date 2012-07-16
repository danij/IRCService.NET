'IRCService.NET. Generic IRC service library for .NET
'Copyright (C) 2010-2012 Dani J

'This program is free software: you can redistribute it and/or modify
'it under the terms of the GNU General Public License as published by
'the Free Software Foundation, either version 3 of the License, or
'(at your option) any later version.

'This program is distributed in the hope that it will be useful,
'but WITHOUT ANY WARRANTY; without even the implied warranty of
'MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
'GNU General Public License for more details.

'You should have received a copy of the GNU General Public License
'along with this program.  If not, see <http://www.gnu.org/licenses/>.

Imports IRCServiceNET
Imports IRCServiceNET.Protocols.P10
Imports IRCServiceNET.Protocols.P10.Parsers

Module Module1
    Private service As IRCService
    Private plugin As MyPlugin

    Sub Main()
        service = New IRCService(New Parser(), New P10CommandFactory())

        service.Host = "127.0.0.1"
        service.Port = 4400
        service.Name = "services.server.name"
        service.Description = "sample service"
        service.Numeric = "ab"
        service.Password = "pass"

        service.PrepareForPlugins()
        plugin = New MyPlugin(service)
        plugin.Name = "Plugin"
        plugin.MainServer = service.MainServer
        plugin.CreateBot()

        service.Connect()

        Console.WriteLine("Press any key to stop the program")
        Console.ReadKey()
    End Sub

End Module
