using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TelegramApiProject.Search;
using TelegramApiProject.Send;
using TeleSharp.TL;
using TeleSharp.TL.Channels;
using TeleSharp.TL.Messages;
using TLSharp.Core;

namespace TelegramApiProject.Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HomeController : ControllerBase
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserSearchService _userSearchService;
        private readonly SendService _sendService;

        public HomeController(ILogger<HomeController> logger, UserSearchService userSearchService,
            SendService sendService)
        {
            _logger = logger;
            _userSearchService = userSearchService;
            _sendService = sendService;
        }

        [HttpGet]
        public string Get()
        {
            return "home";
        }

        [HttpGet]
        [Route("a")]
        public async Task<string> GetUsers()
        {
            var client = await Client.GetClient();
            
            UserSearchModel searchModel = new UserSearchModel()
            {
                TargetGroupName = "TestGroupForBot",
                NicknameIsAbsent = true,
                UserStatus = new TLUserStatusOffline(),
                IsPhotoPresent = false,
                LastSeen = DateTime.Today.AddDays(-10)
            };

            UserSearchResult searchResultas = await _userSearchService.Find(client, searchModel);
            var users = searchResultas.TlUsers;

            return "GetUsers";
        }

        [HttpGet]
        [Route("a")]
        public async Task<string> Send()
        {
            var client = await Client.GetClient();

            SendModel sendModel = new SendModel() { Interval = new TimeSpan(0, 0, 7), Message = "Sample text" };

            await _sendService.SendMessages(client, sendModel);

            return "sender";
        }

        private List<User> GetFakeUsers()
        {
            return new List<User>()
            {
                new User() { Id = 123, IsPhoto = true, Nickname = "example", LastSeen = "offline" },
                new User() { Id = 8756, IsPhoto = false, Nickname = "amsefpl", LastSeen = "offline" },
                new User() { Id = 70215, IsPhoto = true, Nickname = "qwerty", LastSeen = "online" }
            };
        }

        private async Task Draft(TelegramClient client)
        {
            var req = new TeleSharp.TL.Messages.TLRequestGetMessages() { Id = new TLVector<int>() };
            //TeleSharp.TL.TLAbsChat
            try
            {
                var t2 = await client.GetUserDialogsAsync() as TLDialogs;//await client.GetUserDialogsAsync();
                var dialogs = (TLDialogsSlice)await client.GetUserDialogsAsync();
                var dialogs1 = (TLDialogs)await client.GetUserDialogsAsync();
                var dialogs2 = (TLAbsDialogs)await client.GetUserDialogsAsync();
                var messages = dialogs.Messages;
                var chats = dialogs.Chats;
                var users = dialogs.Users;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                //Unable to cast object of type 'TeleSharp.TL.Messages.TLDialogsSlice' to type 'TeleSharp.TL.Messages.TLDialogs'.
            }

            //var chats = dialogs.Chats
            //            .OfType<TLChat>()
            //            .ToList();
            //var res = await client.SendRequestAsync<TLVector<TLAbsChats>>(req);
            var peer = new TLInputPeerSelf();
            var req1 = new TLRequestGetDialogs() { OffsetPeer = peer };

            await Task.Delay(250);

            var res1 = await client.SendRequestAsync<TLDialogsSlice>(req1);
            var req45 = new TLRequestGetDialogs() { OffsetPeer = peer, Limit = 100 };

            await Task.Delay(250);

            var res = await client.SendRequestAsync<TLDialogsSlice>(req);

        }
    }
}
