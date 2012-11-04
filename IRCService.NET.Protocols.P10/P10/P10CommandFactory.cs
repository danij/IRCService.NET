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

namespace IRCServiceNET.Protocols.P10
{
    public class P10CommandFactory : ICommandFactory
    {
        public NewServerCommand CreateNewServerCommand()
        {
            return new P10NewServerCommand();
        }
        public NewUserCommand CreateNewUserCommand()
        {
            return new P10NewUserCommand();
        }
        public ServerQuitCommand CreateServerQuitCommand()
        {
            return new P10ServerQuitCommand();
        }
        public UserQuitCommand CreateUserQuitCommand()
        {
            return new P10UserQuitCommand();
        }
        public ChangeUserModeCommand CreateChangeUserModeCommand()
        {
            return new P10ChangeUserModeCommand();
        }
        public ChangeNickCommand CreateChangeNickCommand()
        {
            return new P10ChangeNickCommand();
        }
        public SendMessageCommand CreateSendMessageCommand()
        {
            return new P10SendMessageCommand();
        }
        public UserDisconnectCommand CreateUserDisconnectCommand()
        {
            return new P10UserDisconnectCommand();
        }
        public JoinChannelCommand CreateJoinChannelCommand()
        {
            return new P10JoinChannelCommand();
        }
        public ChangeChannelModeCommand CreateChangeChannelModeCommand()
        {
            return new P10ChangeChannelModeCommand();
        }
        public PartChannelCommand CreatePartChannelCommand()
        {
            return new P10PartChannelCommand();
        }
        public KickUserCommand CreateKickUserCommand()
        {
            return new P10KickUserCommand();
        }
        public EndOfBurstCommand CreateEndOfBurstCommand()
        {
            return new P10EndOfBurstCommand();
        }
        public ErrorCommand CreateErrorCommand()
        {
            return new P10ErrorCommand();
        }
        public AcknowledgeBurstCommand CreateAcknowledgeBurstCommand()
        {
            return new P10AcknowledgeBurstCommand();
        }
        public PingReplyCommand CreatePingReplyCommand()
        {
            return new P10PingReplyCommand();
        }
        public ServerAuthenticationCommand CreateServerAuthenticationCommand()
        {
            return new P10ServerAuthenticationCommand();
        }
        public ServerIntroductionCommand CreateServerIntroductionCommand()
        {
            return new P10ServerIntroductionCommand();
        }
        public ServerNoticeCommand CreateServerNoticeCommand()
        {
            return new P10ServerNoticeCommand();
        }
        public AuthenticateUserCommand CreateAuthenticateUserCommand()
        {
            return new P10AuthenticateUserCommand();
        }
        public ChannelBurstCommand CreateChannelBurstCommand()
        {
            return new P10ChannelBurstCommand();
        }
        public WallopsCommand CreateWallopsCommand()
        {
            return new P10WallopsCommand();
        }
        public ChangeChannelTopicCommand CreateChangeChannelTopicCommand()
        {
            return new P10ChangeChannelTopicCommand();
        }
        public InviteUserCommand CreateInviteUserCommand()
        {
            return new P10InviteUserCommand();
        }
    }
}
