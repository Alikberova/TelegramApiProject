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
        public string? LastSeen { get; set; }

        #nullable enable
        public string? UserStatus { get; set; }

        public bool WithPhoto { get; set; }
    }
}
