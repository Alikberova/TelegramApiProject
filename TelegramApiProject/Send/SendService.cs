using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TelegramApiProject.Search;
using TeleSharp.TL;
using TLSharp.Core;

namespace TelegramApiProject.Send
{
    public class SendService
    {
        private readonly UserSearchService _searchService;
        private readonly MessageServise _messageServise;

        public SendService(UserSearchService searchService, MessageServise messageServise)
        {
            _searchService = searchService;
            _messageServise = messageServise;
        }

        public async Task<SendResult> Send(TelegramClient client, SendModel sendModel)
        {
            var result = new SendResult() { BlackList = new List<TLUser>() };
            //List<TLUser> users = _searchService.Find(client, new UserSearchModel()).Result.TlUsers;

            if (sendModel.Message != null)
            {
                var blackListUsers = await SendMessages(client, sendModel);
                result.BlackList.AddRange(blackListUsers);
            }

            return result;
        }

        public async Task RunPeriodically(TelegramClient client, SendModel sendModel, CancellationToken token)
        {
            while (sendModel.Interval != null) //todo add condition
            {
                await SendMessages(client, sendModel);
                await Task.Delay(sendModel.Interval.Value, token);
            }
        }

        public async Task<List<TLUser>> SendMessages(TelegramClient client, SendModel sendModel)
        {
            List<TLUser> users = _searchService.Find(client, new UserSearchModel()).Result.TlUsers;
            List<TLUser> usersBlackList = new List<TLUser>();

            foreach (TLUser user in users)
            {
                string message = _messageServise.FormMessaage(user, sendModel.Message);
                await client.SendMessageAsync(new TLInputPeerUser() { UserId = user.Id }, message);

                Console.WriteLine(string.Format($"Sent message to {user.FirstName}"));
                usersBlackList.Add(user);
                Console.WriteLine($"Users black list count: " + usersBlackList.Count);
            }

            return usersBlackList;
        }

        public void Draft(Action action, TelegramClient client)
        {
            //var startTimeSpan = TimeSpan.Zero;
            //SendModel sendModel = new SendModel() { Interval = new TimeSpan(0, 0, 7), Message = "Test message here" };
            //var timer = new Timer((e) =>
            //{
            //    SendMessages(client, sendModel);
            //}, null, startTimeSpan, sendModel.Interval);

            //SenderDelegate sender = new SenderDelegate(Test);

            //Task.Factory.StartNew(() =>
            //{
            //    Thread.Sleep(interval);
            //    sender(client);
            //});
        }
    }
}
