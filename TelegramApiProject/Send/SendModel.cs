using System;

namespace TelegramApiProject.Send
{
    public class SendModel
    {
        public string Message { get; set; }
#nullable enable
        public  TimeSpan? Interval { get; set; }
    }
}