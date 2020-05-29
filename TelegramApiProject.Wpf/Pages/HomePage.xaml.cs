using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using TelegramApiProject.Search;
using TelegramApiProject.User;

namespace TelegramApiProject.Wpf.Pages
{
    public partial class HomePage : Page
    {
        private readonly UserSearchService _userSearchService;
        private readonly UserService _userService;

        private UserSearchResult _searchResult;

        public HomePage()
        {
            _userService = new UserService();
            _userSearchService = new UserSearchService(_userService);

            InitializeComponent();

            _searchResult = new UserSearchResult();
        }

        private async void Button_Click_Results(object sender, RoutedEventArgs e)
        {
            UserSearchModel searchModel = GetSearchModelFromUserInput();

            var client = await Client.GetClient();

            _searchResult = await _userSearchService.Find(client, searchModel);

            ResultPage result = new ResultPage(_searchResult);

            NavigationService.Navigate(result);
        }

        private UserSearchModel GetSearchModelFromUserInput()
        {
            return new UserSearchModel()
            {
                UserStatus = _userService.GetUserStatusFromChoosenListItem(
                    UserStatusList.Items.Count, UserStatusList.SelectedIndex),
                IsNicknamePresent = _userService.GetBoolFromChoosenListItem(
                    UserNicknameIsAbsent.SelectedIndex),
                IsPhotoPresent = _userService.GetBoolFromChoosenListItem(
                    UserPhotoIsPresent.SelectedIndex),
                LastSeen = UserLastSeenDate.SelectedDate,
                GroupsList = GetChatsNames(GroupsList.Text)
                /* List groups for test:
                 Test acc Superman,Тест Группа,Test Group, Testgroup5*, TestGroup3, testgroupraid,Testgroup20181231-1, 
                1000FPS SANDBOX, React(Native)Sandbox, Песочница: чат, TestGroupBot, Babyblog front gitlab, TEST GROUP, something else,
                rnsandbox,
                testgroup_Nous
                 */
            };
        }

        private List<string> GetChatsNames(string names)
        {
            List<string> chatsNames = new List<string>();
            string[] namesArray = names.Split(',').Where(n => !string.IsNullOrWhiteSpace(n)).ToArray();

            for (int i = 0; i < namesArray.Length; i++)
            {
                //if (i == 10) break;

                string name = namesArray[i];
                chatsNames.Add(name.Trim());
            }

            return chatsNames;
        }

        private void Button_Click_Logout(object sender, RoutedEventArgs e)
        {
            _userService.DeleteUserSession();
            var client = Client.GetClient().Result;
            client.Session.TLUser = null;
            AuthorizePage authorizePage = new AuthorizePage();
            NavigationService.Navigate(authorizePage);
        }
    }
}
