using TeleSharp.TL;

namespace TelegramApiProject.Send
{
    public class MessageServise
    {
        public string FormMessaage(TLUser user, string messageText, bool isNameincluded)
        {
            if (!string.IsNullOrEmpty(user.FirstName) && isNameincluded)
            {
                return $"{user.FirstName}, {messageText}";
            }

            return messageText;
        }
    }
}
