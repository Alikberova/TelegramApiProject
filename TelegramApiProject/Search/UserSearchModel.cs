using System;
using System.Collections.Generic;
using System.Text;
using TeleSharp.TL;

namespace TelegramApiProject.Search
{
    public class UserSearchModel
    {
#nullable enable
        public string TargetGroupName { get; set; }

        public TLAbsUserStatus UserStatus { get; set; }

#nullable enable
        public DateTime? LastSeen { get; set; } //todo maybe dif type

#nullable enable
        public bool? IsPhotoPresent { get; set; }

#nullable enable
        public bool? NicknameIsAbsent { get; set; }
    }
}
