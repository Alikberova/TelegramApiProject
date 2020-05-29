using System;
using System.Text.RegularExpressions;
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
            GridRowPassword.Height = new GridLength(0);
        }

        private async void Button_Click_GetAuthorizeCode(object sender, RoutedEventArgs e)
        {
            //string phoneNumber = Regex.Replace(UserPhoneNumber.Text, @"\s+", "");
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
            try
            {
                await Client.AuthorizeWithSms(UserPhoneNumber.Text, _hash, UserSmsCode.Text);
                NavigateHome();
            }
            catch (Exception ex)
            {
                if (ex.Message == "This Account has Cloud Password !")
                {
                    MessageBox.Show("Включена двухфакторная верификация. Введите пароль.", MessageBoxConstants.Information, 
                        MessageBoxButton.OK, MessageBoxImage.Question);
                    GridRowPassword.Height = new GridLength(214);
                }
                else
                {
                    Logger.Error(ex);
                }
            }
        }

        private async void Button_Click_AuthorizeWithUserPassword(object sender, RoutedEventArgs e)
        {
            try
            {
                await Client.AuthorizeWithPassword(UserPassword.Password);
                NavigateHome();
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }

        private void NavigateHome()
        {
            var authorizedUserNumber = Client.GetClient().Result.Session.TLUser.Phone;
            string uiPhoneNumber =  Regex.Replace(UserPhoneNumber.Text, @"\s+", "").TrimStart('+');

            if (authorizedUserNumber == uiPhoneNumber)
            {
                HomePage home = new HomePage();
                NavigationService.Navigate(home);
            }
        }
    }
}
