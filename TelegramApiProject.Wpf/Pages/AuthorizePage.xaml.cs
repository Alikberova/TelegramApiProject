using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace TelegramApiProject.Wpf.Pages
{
    public partial class AuthorizePage : Page
    {
        private string _hash;

        public AuthorizePage()
        {
            InitializeComponent();
        }

        private async void Button_Click_GetAuthorizeCode(object sender, RoutedEventArgs e)
        {
            try
            {
                _hash = await Client.GetSmsCode(UserPhoneNumber.Text);
            }
            catch (Exception ex)
            {
                _hash = await Client.GetSmsCode(UserPhoneNumber.Text);
                Logger.Error(ex);
            }
        }

        private async void Button_Click_AuthorizeWithSmsCode(object sender, RoutedEventArgs e)
        {
            await Client.AuthorizeWithSms(UserPhoneNumber.Text, _hash, UserSmsCode.Text);
            var authorizedUserNumber = Client.GetClient().Result.Session.TLUser.Phone;

            if (authorizedUserNumber == UserPhoneNumber.Text.TrimStart('+'))
            {
                HomePage home = new HomePage();
                NavigationService.Navigate(home);
            }
        }
    }
}
