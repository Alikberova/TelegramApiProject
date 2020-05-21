using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;
using TeleSharp.TL.Account;
using TLSharp.Core;

namespace TelegramApiProject.Wpf
{
    public class Client
    {
        private static TelegramClient client;
        private static readonly AppConfig config = Program.StaticConfig.GetSection("AppConfig").Get<AppConfig>();

        public static async Task<TelegramClient> GetClient()
        {
            try
            {
                //+19014794259
                if (client != null)
                {
                    return client;
                }

                string sessionPath = new PathService().SessionPath();

                client = new TelegramClient(config.ApiId, config.ApiHash, new FileSessionStore(), sessionPath);

                await client.ConnectAsync();

                return client;
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
            return null;
        }

        public static async Task<string> GetSmsCode(string phoneNumber)
        {
            var hash = await client.SendCodeRequestAsync(phoneNumber);
            return hash;
        }

        public static async Task AuthorizeWithSms(string phoneNumber, string hash, string smsCode)
        {
            await client.MakeAuthAsync(phoneNumber, hash, smsCode);
        }

        public static async Task AuthorizeWithPassword(string password)
        {
            var tlPassword = await client.GetPasswordSetting();
            await client.MakeAuthWithPasswordAsync(tlPassword, password);
        }
    }
}
