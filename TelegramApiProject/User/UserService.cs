using System;
using System.IO;
using TeleSharp.TL;

namespace TelegramApiProject.User
{
    public class UserService
    {
        public TLAbsUserStatus GetUserStatusFromChoosenListItem(int itemCollectionCount, int selectedIndex)
        {
            if (selectedIndex == itemCollectionCount - 1) return null;

            try
            {
                var statuses = typeof(UserStatus).GetProperties();

                for (int i = 0; i < statuses.Length; i++)
                {
                    if (selectedIndex == i)
                    {
                        var statusType = statuses[i].PropertyType;
                        TLAbsUserStatus instance = (TLAbsUserStatus)Activator.CreateInstance(statusType);

                        return instance;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);

            }
            return null;
        }

        public UserModel CreateCustomUserModel(TLUser tlUser)
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
                            //assign TLUser's props to User
                            var tlUserPropValue = tlUser.GetType().GetProperty(tlUserProp.Name).GetValue(tlUser);
                            user.GetType().GetProperty(customUserProp.Name).SetValue(
                                user, tlUserPropValue);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }

            if (tlUser.Status != null)
            {
                user.UserStatus = tlUser.Status.ToString().Substring(25);

                if (tlUser.Status is TLUserStatusOffline)
                {
                    var offline = tlUser.Status as TLUserStatusOffline;
                    user.LastSeen = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc)
                        .AddSeconds(offline.WasOnline)
                        .ToLocalTime()
                        .ToString();
                }
            }

            user.WithPhoto = tlUser.Photo != null ? true : false;

            return user;
        }

        //usually uses for 3-counted list where 1) yes, 2) no, 3) null
        public bool? GetBoolFromChoosenListItem(int index)
        {
            if (index == 0)
            {
                return true;
            }
            else if (index == 1)
            {
                return false;
            }
            return null;
        }

        public void DeleteUserSession()
        {
            string sessionPath = new PathService().SessionPath() + ".dat";
            if (File.Exists(sessionPath))
            {
                File.Delete(sessionPath);
            }
        }
    }
}
