using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeleSharp.TL;
using TeleSharp.TL.Channels;
using TeleSharp.TL.Messages;
using TLSharp.Core;

namespace TelegramApiProject.Search
{
    public class UserSearchService
    {
        public async Task<UserSearchResult> Find(TelegramClient client, UserSearchModel searchModel)
        {
            UserSearchResult searchResult = new UserSearchResult() { TlUsers = new List<TLUser>(), Users = new List<User>() };
            List<TLUser> users = await GetUsers(client);

            if (searchModel.IsPhotoPresent != null)
            {
                users = await GetUsersBasedOnPhotoPresense(client, users, searchModel.IsPhotoPresent);
                searchResult.TlUsers.AddRange(users);
            }
            if (searchModel.UserStatus != null)
            {
                users = await GetByUserStatus(client, users, searchModel.UserStatus);
                searchResult.TlUsers.AddRange(users);
            }
            if (searchModel.LastSeen != null)
            {
                users = await GetByLastSeen(client, users, searchModel.LastSeen);
                searchResult.TlUsers.AddRange(users);
            }
            if (searchModel.NicknameIsAbsent != null)
            {
                users = await GetByAbsentNickname(client, users, searchModel.NicknameIsAbsent);
                searchResult.TlUsers.AddRange(users);
            }
            return EnsureUniqueUsers(searchResult);
        }

        public UserSearchResult EnsureUniqueUsers(UserSearchResult searchResult)
        {
            searchResult.TlUsers = searchResult.TlUsers.GroupBy(x => x.Id).Select(y => y.First()).ToList();

            return searchResult;
        }

        private async Task<List<TLUser>> GetByAbsentNickname(TelegramClient client, List<TLUser> users, bool? isNicknameAbsent)
        {
            var result = new List<TLUser>();

            foreach (TLUser user in users)
            {
                bool isAbsent = user.Username == null ? true : false;

                if (isAbsent == isNicknameAbsent)
                {
                    result.Add(user);
                }
            }

            return result;
        }

        private async Task<List<TLUser>> GetByLastSeen(TelegramClient client, List<TLUser> users, DateTime? startedSearchedLastSeenTime)
        {
            var result = new List<TLUser>();

            foreach (TLUser user in users)
            {
                var status = user.Status;

                if (status is TLUserStatusOffline)
                {
                    var offlineStatus = status as TLUserStatusOffline;
                    DateTime actualLastSeen = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc)
                        .AddSeconds(offlineStatus.WasOnline)
                        .ToLocalTime();

                    if (startedSearchedLastSeenTime < actualLastSeen)
                    {
                        result.Add(user);
                    }
                }
            }

            return result;
        }

        private async Task<List<TLUser>> GetUsersBasedOnPhotoPresense(TelegramClient client, List<TLUser> users, bool? isPhotoPresent)
        {
            var result = new List<TLUser>();

            foreach (TLUser user in users)
            {
                bool actualPhotoPresense = user.Photo != null ? true : false;

                if (actualPhotoPresense == isPhotoPresent)
                {
                    result.Add(user);
                }
            }

            return result;
        }

        public async Task<List<TLUser>> GetByUserStatus(TelegramClient client, List<TLUser> users, TLAbsUserStatus searchedStatus)
        {
            var result = new List<TLUser>();

            foreach (TLUser user in users)
            {
                TLAbsUserStatus actualStatus = user.Status;
                var searchedStatusName = searchedStatus.GetType().FullName;
                if (actualStatus.ToString() == searchedStatusName)
                {
                    result.Add(user);
                }
            }

            return result;
        }

        public async Task<List<TLUser>> GetUsers(TelegramClient client)
        {
            var usersList = new List<TLUser>();

            try
            {
                var dialogs = (TLDialogsSlice)await client.GetUserDialogsAsync();

                foreach (TLAbsChat element in dialogs.Chats)
                {
                    if (element is TLChat) //chats with permissions
                    {
                        TLChat chat = element as TLChat;
                        var request = new TLRequestGetFullChat() { ChatId = chat.Id };

                        TeleSharp.TL.Messages.TLChatFull chatFull = await client.SendRequestAsync<TeleSharp.TL.Messages.TLChatFull>(request);

                        foreach (var absUser in chatFull.Users)
                        {
                            TLUser user = absUser as TLUser;

                            if (!user.Bot)
                            {
                                usersList.Add(user);
                            }
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            return usersList;
        }

        private async Task GetPublicChannelsAsync(TelegramClient client, TLDialogsSlice dialogs)
        {
            foreach (TLAbsChat element in dialogs.Chats)
            {
                if (element is TLChannel)
                {
                    var offset = 0;
                    TLChannel channel = element as TLChannel;

                    var chan = await client.SendRequestAsync<TeleSharp.TL.Messages.TLChatFull>(new TLRequestGetFullChannel()
                    {
                        Channel = new TLInputChannel()
                        { ChannelId = channel.Id, AccessHash = (long)channel.AccessHash }
                    });

                    TLInputPeerChannel inputPeer = new TLInputPeerChannel()
                    { ChannelId = channel.Id, AccessHash = (long)channel.AccessHash };
                }
            }
        }
    }
}
