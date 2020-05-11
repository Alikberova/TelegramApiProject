using System;
using TeleSharp.TL;

namespace TelegramApiProject
{
    public class User
    {
        public int Id { get; set; }

        #nullable enable
        public string? Nickname { get; set; }

        #nullable enable
        public string? LastSeen { get; set; }

        public string UserStatus { get; set; }

        public bool IsPhoto { get; set; }
    }
}
