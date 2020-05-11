using System;
using System.Collections.Generic;
using System.Text;
using TeleSharp.TL;

namespace TelegramApiProject.Send
{
    public class MessageServise
    {
        public string FormMessaage(TLUser user, string messageText)
        {
            if (string.IsNullOrEmpty(user.FirstName))
            {
                return $"{user.FirstName}, \n{messageText}";
            }

            return messageText;
        }
    }
}
