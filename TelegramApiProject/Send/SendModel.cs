using System;

namespace TelegramApiProject.Send
{
    public class SendModel
    {
#nullable enable
        public string? Photo { get; set; }
#nullable enable
        public string? Document { get; set; }
#nullable enable
        public string? Message { get; set; }
#nullable enable
        public  TimeSpan? Interval { get; set; }

        public bool IsNameIncluded { get; set; }
    }
}