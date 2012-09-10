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
Imports IRCServiceNET.Plugins
Imports IRCServiceNET.Entities

''' <summary>
''' A sample plugin
''' </summary>
''' <remarks></remarks>
Public Class MyPlugin
    Inherits IRCServicePlugin

    ''' <summary>
    ''' A simple bot
    ''' </summary>
    ''' <remarks></remarks>
    Private _bot As IUser
    ''' <summary>
    ''' Default constructor
    ''' </summary>
    ''' <param name="service"></param>
    ''' <remarks></remarks>
    Public Sub New(service As IRCService)
        MyBase.New(service)
    End Sub
    ''' <summary>
    ''' Gets a simple bot
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Bot() As IUser
        Get
            Return _bot
        End Get
        Private Set(value As IUser)
            _bot = value
        End Set
    End Property
    ''' <summary>
    ''' Creates the bot
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub CreateBot()
        If Service.GetUserByNick("Bot") Is Nothing Then
            Bot = CreateUser(
                    Service.MainServer,
                    "bot",
                    "~bot",
                    "localhost",
                    "A simple bot",
                    IPAddress.Parse("127.0.0.1")
                  )
        End If
    End Sub
    ''' <summary>
    ''' Occurs when the service is synced with the network
    ''' </summary>
    ''' <remarks></remarks>
    Public Overrides Sub OnBurstCompleted()
        MyBase.OnBurstCompleted()

        Bot.Action.ChangeOper(True)
        Bot.Action.JoinChannel("#channel")
        Bot.Action.SendChannelNotice("#channel", "Hello everyone :)")
    End Sub
    ''' <summary>
    ''' Occurs when a private message is received
    ''' </summary>
    ''' <param name="from"></param>
    ''' <param name="to"></param>
    ''' <param name="message"></param>
    ''' <remarks></remarks>
    Public Overrides Sub OnPrivateMessage(from As IRCServiceNET.Entities.IUser,
                                          [to] As IRCServiceNET.Entities.IUser,
                                          message As String)
        MyBase.OnPrivateMessage(from, [to], message)

        If [to] Is Bot Then
            If message.Trim().ToLower() = "hello" Then
                [to].Action.SendPrivateMessage(from, "Hi there!")
            End If
        End If
    End Sub
End Class
