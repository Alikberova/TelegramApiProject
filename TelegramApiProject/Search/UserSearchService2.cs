using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelegramApiProject.User;
using TeleSharp.TL;
using TeleSharp.TL.Channels;
using TeleSharp.TL.Messages;
using TLSharp.Core;

namespace TelegramApiProject.Search
{
    public class UserSearchService2
    {
        public async Task<UserSearchResult> Find(TelegramClient client, UserSearchModel searchModel)
        {
            UserSearchResult searchResult = new UserSearchResult() { TlUsers = new List<TLUser>(), Users = new List<UserModel>() };
            List<TLUser> users = await GetUsers(client);

            if (searchModel.IsPhotoPresent != null)
            {
                var res = await GetUsersBasedOnPhotoPresense(client, users, searchModel.IsPhotoPresent);
                searchResult.TlUsers.AddRange(res.TlUsers);
                searchResult.Users.AddRange(res.Users);
            }
            if (searchModel.UserStatus != null)
            {
                var res = await GetByUserStatus(client, users, searchModel.UserStatus);
                searchResult.TlUsers.AddRange(res.TlUsers);
                searchResult.Users.AddRange(res.Users);
            }
            if (searchModel.LastSeen != null)
            {
                var res = await GetByLastSeen(client, users, searchModel.LastSeen);
                searchResult.TlUsers.AddRange(res.TlUsers);
                searchResult.Users.AddRange(res.Users);
            }
            if (searchModel.IsNicknamePresent != null)
            {
                var res = await GetByPresenetNickname(client, users, searchModel.IsNicknamePresent);
                searchResult = res;
                searchResult.TlUsers.AddRange(res.TlUsers);
                searchResult.Users.AddRange(res.Users);
            }
            return EnsureUniqueUsers(searchResult);
        }

        public UserSearchResult EnsureUniqueUsers(UserSearchResult searchResult)
        {
            searchResult.TlUsers = searchResult.TlUsers.GroupBy(x => x.Id).Select(y => y.First()).ToList();

            searchResult.Users = searchResult.Users.GroupBy(x => x.Id).Select(y => y.First()).ToList();

            return searchResult;
        }

        private UserModel CreateCustomUser(TLUser tlUser,  string userStatus = null, string lastSeen = null, bool? isPhoto = null)
        {
            var props = tlUser.GetType().GetProperties();
            var user = new UserModel();
            try
            {
                var userProps = user.GetType().GetProperties();
                for (int i = 0; i < userProps.Length; i++)
                {
                    for (int y = 0; y < props.Length; y++)
                    {
                        var customUserProp = userProps[i];
                        var tlUserProp = props[y];

                        if (tlUserProp.Name == customUserProp.Name)
                        {
                            //assign props of TLUser to User
                            user.GetType().GetProperty(customUserProp.Name).SetValue(
                                user, tlUser.GetType().GetProperty(tlUserProp.Name).GetValue(tlUser));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debugger.Break();
            }

            user.UserStatus = userStatus;
            user.LastSeen = lastSeen;
            if (isPhoto != null) user.IsPhoto = (bool)isPhoto;

            return user;
        }

        private async Task<UserSearchResult> GetByPresenetNickname(TelegramClient client, List<TLUser> users, bool? isNicknamePresent)//todo delete client
        {
            var result = new UserSearchResult() { TlUsers = new List<TLUser>(), Users = new List<UserModel>() };

            foreach (TLUser user in users)
            {
                bool isPresent = user.Username == null ? false : true;

                if (isPresent == isNicknamePresent)
                {
                    result.TlUsers.Add(user);
                    result.Users.Add(CreateCustomUser(user));
                }
            }

            return result;
        }

        private async Task<UserSearchResult> GetByLastSeen(TelegramClient client, List<TLUser> users, DateTime? startedSearchedLastSeenTime)
        {
            var result = new UserSearchResult() { TlUsers = new List<TLUser>(), Users = new List<UserModel>() };

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
                        result.TlUsers.Add(user);
                        result.Users.Add(CreateCustomUser(user, lastSeen: actualLastSeen.ToString()));
                    }
                }
            }

            return result;
        }

        private async Task<UserSearchResult> GetUsersBasedOnPhotoPresense(TelegramClient client, List<TLUser> users, bool? isPhotoPresent)
        {
            var result = new UserSearchResult() { TlUsers = new List<TLUser>(), Users = new List<UserModel>() };

            foreach (TLUser user in users)
            {
                bool actualPhotoPresense = user.Photo != null ? true : false;

                if (actualPhotoPresense == isPhotoPresent)
                {
                    result.TlUsers.Add(user);
                    result.Users.Add(CreateCustomUser(user, isPhoto: isPhotoPresent));
                }
            }

            return result;
        }

        public async Task<UserSearchResult> GetByUserStatus(TelegramClient client, List<TLUser> users, TLAbsUserStatus searchedStatus)
        {
            var result = new UserSearchResult() { TlUsers = new List<TLUser>(), Users = new List<UserModel>() };

            foreach (TLUser user in users)
            {
                TLAbsUserStatus actualStatus = user.Status;
                var searchedStatusName = searchedStatus.GetType().FullName;

                if (actualStatus.ToString() == searchedStatusName)
                {
                    var statusStr = searchedStatusName.Substring(14);
                    result.TlUsers.Add(user);
                    result.Users.Add(CreateCustomUser(user, userStatus: searchedStatusName.Substring(11))); //todo didn't check
                }
            }

            return result;
        }

        public async Task<List<TLUser>> GetUsers(TelegramClient client)
        {
            var usersList = new List<TLUser>();

            try
            {
                var dialogs = (TLDialogs)await client.GetUserDialogsAsync();
                //var dialogs2 = (TLDialogsSlice)await client.GetUserDialogsAsync();

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
