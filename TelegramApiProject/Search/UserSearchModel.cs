using System;
using TeleSharp.TL;

namespace TelegramApiProject.Search
{
    public class UserSearchModel
    {
#nullable enable
        public TLAbsUserStatus? UserStatus { get; set; }

#nullable enable
        public DateTime? LastSeen { get; set; }

#nullable enable
        public bool? IsPhotoPresent { get; set; }

#nullable enable
        public bool? IsNicknamePresent { get; set; }
    }
}
