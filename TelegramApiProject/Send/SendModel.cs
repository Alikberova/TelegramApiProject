using System;
using System.Collections.Generic;

namespace TelegramApiProject.Send
{
    public class SendModel
    {
#nullable enable
        public List<string>? Photos { get; set; }
#nullable enable
        public List<string>? Documents { get; set; }
#nullable enable
        public string? Message { get; set; }
#nullable enable
        public  TimeSpan? Interval { get; set; }

        public bool IsNameIncluded { get; set; }
    }
}