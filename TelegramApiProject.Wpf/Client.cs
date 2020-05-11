using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TeleSharp.TL;
using TeleSharp.TL.Account;
using TLSharp.Core;

namespace TelegramApiProject.Web
{
    public class Client
    {
        public static TelegramClient client;
        public static TLUser loggedUser;
        static readonly AppConfig config = Startup.StaticConfig.GetSection("AppConfig").Get<AppConfig>();

        public static async Task<TelegramClient> GetClient()
        {
            if (client != null)
            {
                return client;
            }

            client = new TelegramClient(config.ApiId, config.ApiHash, new FileSessionStore(), "session");
            await client.ConnectAsync();

            loggedUser = await GetLoggedInUser(client);

            return client;
        }

        public static async Task<TLUser> GetLoggedInUser(TelegramClient client)
        {
            if (client.Session != null)
            {
                return client.Session.TLUser;
            }

            var hash = await client.SendCodeRequestAsync(config.PhoneNumber);
            Debugger.Break();
            var code = "";

            return await client.MakeAuthAsync(config.PhoneNumber, hash, code);
        }
    }
}
