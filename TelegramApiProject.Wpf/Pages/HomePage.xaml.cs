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
                LastSeen = UserLastSeenDate.SelectedDate
            };
        }

        private void Button_Click_Logout(object sender, RoutedEventArgs e)
        {
            _userService.DeleteUserSession();
            AuthorizePage authorizePage = new AuthorizePage();
            NavigationService.Navigate(authorizePage);
        }
    }
}
