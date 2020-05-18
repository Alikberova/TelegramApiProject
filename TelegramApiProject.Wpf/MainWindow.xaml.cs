using System.Diagnostics;
using System.Windows.Navigation;
using TelegramApiProject.Wpf.Pages;

namespace TelegramApiProject.Wpf
{
    public partial class MainWindow : NavigationWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            CheckIfAuthorized();
        }

        private void CheckIfAuthorized()
        {
            //need stay synchronous (!)
            var client = Client.GetClient().Result;
            HomePage home = new HomePage();

            if (client.Session != null && client.Session.TLUser != null)
            {
                NavigationService.Navigate(home);
            }
            else
            {
                AuthorizePage authorizePage = new AuthorizePage();
                NavigationService.Navigate(authorizePage);
            }
        }
    }
}
