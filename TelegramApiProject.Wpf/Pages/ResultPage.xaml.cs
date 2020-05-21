using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using TelegramApiProject.Search;

namespace TelegramApiProject.Wpf.Pages
{
    public partial class ResultPage : Page
    {
        private readonly UserSearchResult _userResult;
        private readonly UserContext _context;
        private readonly DbServise _dbServise;

        public ResultPage(UserSearchResult userResult)
        {
            _userResult = userResult;
            _context = new UserContext();
           _dbServise = new DbServise(_context);

            InitializeComponent();
            Loaded += ResultPage_Loaded;
        }

        private void ResultPage_Loaded(object sender, RoutedEventArgs e)
        {
            _dbServise.AddDefinedUsers(_userResult);
            UsersDataGrid.ItemsSource = _context.SearchResult.Local.ToObservableCollection();
            //column with checkboxes IsPhoto is only for read
            UsersDataGrid.Columns.ElementAtOrDefault(UsersDataGrid.Columns.Count - 1).IsReadOnly = true;
        }

        private void Button_Click_ToSendPage(object sender, RoutedEventArgs e)
        {
            if (_userResult.TlUsers.Count > 0)
            {
                SendPage sendPage = new SendPage(_userResult);
                NavigationService.Navigate(sendPage);
            }
            else
            {
                MessageBox.Show("Некому отправлять сообщения", MessageBoxConstants.Information, MessageBoxButton.OK, 
                    MessageBoxImage.Information);
            }
        }
    }
}
