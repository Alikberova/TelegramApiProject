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
        private readonly BlacklistService _blacklistService;

        public SendService(MessageServise messageServise, UserService userService, BlacklistService blacklistService)
        {
            _messageServise = messageServise;
            _userService = userService;
            _blacklistService = blacklistService;
        }

        public async Task RunPeriodically(TelegramClient client, SendModel sendModel, UserSearchResult searchResult, CancellationToken token)
        {
            if (sendModel.Photos == null && sendModel.Documents == null && string.IsNullOrEmpty(sendModel.Message)) return;

            while (sendModel.Interval != TimeSpan.Zero)
            {
                foreach (var user in searchResult.TlUsers)
                {
                    await Task.Delay(sendModel.Interval.Value, token);

                    searchResult.TlUsers = new List<TLUser>() { user };
                    await SendMessage(client, sendModel, searchResult);
                }
            }
        }

        public async Task<List<UserModel>> SendMessage(TelegramClient client, SendModel sendModel, UserSearchResult searchResult)
        {
            List<UserModel> users = new List<UserModel>();

            foreach (TLUser tlUser in searchResult.TlUsers)
            {
                if (_blacklistService.BlacklistContainsId(tlUser.Id)) continue;

                var peer = new TLInputPeerUser() { UserId = tlUser.Id, AccessHash = tlUser.AccessHash.Value };
                var text = sendModel?.Message;
                var photos = sendModel?.Photos;
                var docs = sendModel?.Documents;

                if (photos != null && photos.Count > 0)
                {
                    foreach (var photo in photos)
                    {
                        var loadedPhoto = (TLInputFile)await client.UploadFile(photo, new StreamReader(photo));

                        if (!string.IsNullOrEmpty(text) && photos.IndexOf(photo) < 1)
                        {
                            await client.SendUploadedPhoto(peer, loadedPhoto,
                                _messageServise.FormMessaage(tlUser, text, sendModel.IsNameIncluded));
                        }
                        else
                        {
                            await client.SendUploadedPhoto(peer, loadedPhoto, string.Empty);
                        }
                    }
                    
                    _blacklistService.WriteToBlacklistFile(tlUser.Id);
                }
                if (docs != null && docs.Count > 0)
                {
                    foreach (var doc in docs)
                    {
                        var loadedDoc = (TLInputFile)await client.UploadFile(doc, new StreamReader(doc));
                        new FileExtensionContentTypeProvider().TryGetContentType(doc, out string contentType);

                        if (!string.IsNullOrEmpty(text) && docs.IndexOf(doc) < 1)
                        {
                            await client.SendUploadedDocument(peer, loadedDoc, _messageServise.FormMessaage(tlUser, text, sendModel.IsNameIncluded),
                                contentType, new TLVector<TLAbsDocumentAttribute>());
                        }
                        else
                        {
                            await client.SendUploadedDocument(peer, loadedDoc, string.Empty, contentType, new TLVector<TLAbsDocumentAttribute>());
                        }
                    }

                    _blacklistService.WriteToBlacklistFile(tlUser.Id);
                }
                else if (!string.IsNullOrEmpty(text))
                {
                    string message = _messageServise.FormMessaage(tlUser, text, sendModel.IsNameIncluded);
                    await client.SendMessageAsync(peer, message);

                    _blacklistService.WriteToBlacklistFile(tlUser.Id);
                }

                var user = _userService.CreateCustomUserModel(tlUser);
                users.Add(user);
            }

            return users;
        }
    }
}
