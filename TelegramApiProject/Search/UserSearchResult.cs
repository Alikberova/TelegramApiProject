using System.Collections.Generic;
using TelegramApiProject.User;
using TeleSharp.TL;

namespace TelegramApiProject.Search
{
    public class UserSearchResult
    {
        public List<UserModel> Users { get; set; }

        public List<TLUser> TlUsers { get; set; }
    }
}
