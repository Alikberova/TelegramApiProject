using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TelegramApiProject.User;
using TeleSharp.TL;
using TeleSharp.TL.Messages;
using TLSharp.Core;

namespace TelegramApiProject.Search
{
    public class UserSearchService
    {
        private readonly UserService _userService;

        public UserSearchService(UserService userService)
        {
            _userService = userService;
        }

        public async Task<UserSearchResult> Find(TelegramClient client, UserSearchModel searchModel)
        {
            UserSearchResult users = await GetUsers(client);

            if (searchModel.IsPhotoPresent != null)
            {
                users = GetUsersBasedOnPhotoPresense(users, searchModel.IsPhotoPresent);
            }
            if (searchModel.UserStatus != null)
            {
                users = GetByUserStatus(users, searchModel.UserStatus);
            }
            if (searchModel.LastSeen != null)
            {
                users = GetByLastSeen(users, searchModel.LastSeen);
            }
            if (searchModel.IsNicknamePresent != null)
            {
                users = GetByPresenetNickname(users, searchModel.IsNicknamePresent);
            }
            
            return EnsureUniqueUsers(users);
        }

        public UserSearchResult EnsureUniqueUsers(UserSearchResult users)
        {
            users.TlUsers = users.TlUsers.GroupBy(x => x.Id).Select(y => y.First()).ToList();

            users.Users = users.Users.GroupBy(x => x.Id).Select(y => y.First()).ToList();

            return users;
        }

        private UserSearchResult GetByPresenetNickname(UserSearchResult users, bool? isNicknamePresent)
        {
            var result = new UserSearchResult() { TlUsers = new List<TLUser>(), Users = new List<UserModel>() };

            foreach (TLUser user in users.TlUsers)
            {
                bool isPresent = user.Username == null ? false : true;

                if (isPresent == isNicknamePresent)
                {
                    result.TlUsers.Add(user);
                    result.Users.Add(_userService.CreateCustomUserModel(user));
                }
            }

            return result;
        }

        private UserSearchResult GetByLastSeen(UserSearchResult users, DateTime? startedSearchedLastSeenTime)
        {
            var result = new UserSearchResult() { TlUsers = new List<TLUser>(), Users = new List<UserModel>() };

            foreach (TLUser user in users.TlUsers)
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
                        result.Users.Add(_userService.CreateCustomUserModel(user));
                    }
                }
            }

            return result;
        }

        private UserSearchResult GetUsersBasedOnPhotoPresense(UserSearchResult users, bool? isPhotoPresent)
        {
            var result = new UserSearchResult() { TlUsers = new List<TLUser>(), Users = new List<UserModel>() };

            foreach (TLUser user in users.TlUsers)
            {
                bool actualPhotoPresense = user.Photo != null ? true : false;

                if (actualPhotoPresense == isPhotoPresent)
                {
                    result.TlUsers.Add(user);
                    result.Users.Add(_userService.CreateCustomUserModel(user));
                }
            }

            return result;
        }

        public UserSearchResult GetByUserStatus(UserSearchResult users, TLAbsUserStatus searchedStatus)
        {
            var result = new UserSearchResult() { TlUsers = new List<TLUser>(), Users = new List<UserModel>() };

            foreach (TLUser user in users.TlUsers)
            {
                TLAbsUserStatus actualStatus = user.Status;
                var searchedStatusName = searchedStatus.GetType().FullName;

                if (actualStatus.ToString() == searchedStatusName)
                {
                    result.TlUsers.Add(user);
                    result.Users.Add(_userService.CreateCustomUserModel(user));
                }
            }

            return result;
        }
        public async Task<UserSearchResult> GetUsers(TelegramClient client)
        {
            UserSearchResult searchResult = new UserSearchResult() { TlUsers = new List<TLUser>(), Users = new List<UserModel>() };

            try
            {
                dynamic dialogs;
                try
                {
                    dialogs = (TLDialogs)await client.GetUserDialogsAsync();
                }
                catch (Exception ex)
                {
                    dialogs = (TLDialogsSlice)await client.GetUserDialogsAsync();
                    Logger.Error(ex);
                }

                foreach (TLAbsChat element in dialogs.Chats)
                {
                    if (element is TLChat)
                    {
                        TLChat chat = element as TLChat;
                        var request = new TLRequestGetFullChat() { ChatId = chat.Id };

                        TeleSharp.TL.Messages.TLChatFull chatFull = await client.SendRequestAsync<TeleSharp.TL.Messages.TLChatFull>(request);

                        foreach (var absUser in chatFull.Users)
                        {
                            TLUser user = absUser as TLUser;

                            if (!user.Bot)
                            {
                                searchResult.TlUsers.Add(user);

                                var customeUser = _userService.CreateCustomUserModel(user);
                                searchResult.Users.Add(customeUser);
                            }
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                Logger.Error(ex);
            }

            return searchResult;
        }
    }
}
