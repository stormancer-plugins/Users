﻿using MsgPack.Serialization;

namespace Stormancer.Server.Users
{
    public class LoginResult
    {
        [MessagePackMember(0)]
        public string ErrorMsg { get; set; } = "";

        [MessagePackMember(1)]
        public bool Success { get; set; }
        
        [MessagePackMember(2)]
        public string UserId { get; set; } = "";

        [MessagePackMember(3)]
        public string Username { get; set; } = "";
    }
}
