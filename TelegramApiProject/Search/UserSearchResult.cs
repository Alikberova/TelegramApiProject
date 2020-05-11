using System;
using System.Collections.Generic;
using System.Text;
using TeleSharp.TL;

namespace TelegramApiProject.Search
{
    public class UserSearchResult
    {
        public List<User> Users { get; set; }

        public List<TLUser> TlUsers { get; set; }
    }
}
