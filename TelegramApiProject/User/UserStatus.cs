using TeleSharp.TL;

namespace TelegramApiProject.User
{
    public class UserStatus
    {
        public TLUserStatusOnline TLUserStatusOnline { get; set; }

        public TLUserStatusOffline TLUserStatusOffline { get; set; }

        public TLUserStatusRecently TLUserStatusRecently { get; set; }

        public TLUserStatusLastWeek TLUserStatusLastWeek { get; set; }

        public TLUserStatusLastMonth TLUserStatusLastMonth { get; set; }

        public TLUserStatusEmpty TLUserStatusEmpty { get; set; }
    }
}
