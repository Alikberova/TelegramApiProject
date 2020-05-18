using SQLite;
using System.Collections.Generic;
using TelegramApiProject.User;
using TeleSharp.TL;

namespace TelegramApiProject.Send
{
    public class SendResult
    {
        public List<UserModel> Users { get; set; }

        public List<TLUser> TlUsers { get; set; }
    }
}