namespace TelegramApiProject.User
{
    public class UserModel
    {
        public int Id { get; set; }

        #nullable enable
        public string? Username { get; set; }

        #nullable enable
        public string? FirstName { get; set; }

        #nullable enable
        public string? Phone { get; set; }

        #nullable enable
        public string? LastSeen { get; set; }

        public string UserStatus { get; set; }

        public bool IsPhoto { get; set; }

        public int TotalMessageCount { get; set; }
    }
}
