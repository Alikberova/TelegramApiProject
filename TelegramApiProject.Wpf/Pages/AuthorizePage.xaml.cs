using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using TelegramApiProject.User;

namespace TelegramApiProject.Wpf.Pages
{
    public partial class AuthorizePage : Page
    {
        private string _hash;

        public AuthorizePage()
        {
            InitializeComponent();
            //Loaded += AuthorizePage_Loaded;
            GridRowPassword.Height = new GridLength(0);
        }

        private void AuthorizePage_Loaded(object sender, RoutedEventArgs e)
        {
            GridRowPassword.Height = new GridLength(0);
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
                    GridRowPassword.Height = new GridLength(214); //GridResizeBehavior
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
            if (authorizedUserNumber == UserPhoneNumber.Text.TrimStart('+'))
            {
                HomePage home = new HomePage();
                NavigationService.Navigate(home);
            }
        }
    }
}
