using Microsoft.AspNetCore.StaticFiles;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using TelegramApiProject.Search;
using TelegramApiProject.User;
using TeleSharp.TL;
using TLSharp.Core;
using TLSharp.Core.Utils;

namespace TelegramApiProject.Send
{
    public class SendService
    {
        private readonly MessageServise _messageServise;
        private readonly UserService _userService;

        public SendService(MessageServise messageServise, UserService userService)
        {
            _messageServise = messageServise;
            _userService = userService;
        }

        public async Task RunPeriodically(TelegramClient client, SendModel sendModel, UserSearchResult searchResult, CancellationToken token)
        {
            if (sendModel.Photo == null && sendModel.Document == null && string.IsNullOrEmpty(sendModel.Message)) return;
            while (sendModel.Interval != TimeSpan.Zero)
            {
                await SendMessage(client, sendModel, searchResult);
                await Task.Delay(sendModel.Interval.Value, token);
            }
        }

        public async Task<List<UserModel>> SendMessage(TelegramClient client, SendModel sendModel, UserSearchResult searchResult)
        {
            List<UserModel> users = new List<UserModel>();

            foreach (TLUser tlUser in searchResult.TlUsers)
            {
                var peer = new TLInputPeerUser() { UserId = tlUser.Id, AccessHash = tlUser.AccessHash.Value };
                var text = sendModel.Message;
                var photo = sendModel.Photo;
                var doc = sendModel.Document;

                if (photo != null)
                {
                    var fileResult = (TLInputFile)await client.UploadFile(photo, new StreamReader(photo));

                    if (!string.IsNullOrEmpty(text))
                    {
                        await client.SendUploadedPhoto(peer, fileResult,
                            _messageServise.FormMessaage(tlUser, text, sendModel.IsNameIncluded));
                    }
                    else
                    {
                        await client.SendUploadedPhoto(peer, fileResult, string.Empty);
                    }
                }
                else if (doc != null)
                {
                    var fileResult = (TLInputFile)await client.UploadFile(doc, new StreamReader(doc));
                    new FileExtensionContentTypeProvider().TryGetContentType(doc, out string contentType);

                    if (!string.IsNullOrEmpty(text))
                    {
                        await client.SendUploadedDocument(peer, fileResult, _messageServise.FormMessaage(tlUser, text, sendModel.IsNameIncluded),
                            contentType, new TLVector<TLAbsDocumentAttribute>());
                    }
                    else
                    {
                        await client.SendUploadedDocument(peer, fileResult, string.Empty, contentType, new TLVector<TLAbsDocumentAttribute>());
                    }
                }
                else if (!string.IsNullOrEmpty(text))
                {
                    string message = _messageServise.FormMessaage(tlUser, text, sendModel.IsNameIncluded);
                    await client.SendMessageAsync(peer, message);
                }

                var user = _userService.CreateCustomUserModel(tlUser);
                user.TotalMessageCount++;
                users.Add(user);
            }

            return users;
        }
    }
}
