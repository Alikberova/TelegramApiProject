using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TelegramApiProject.Wpf
{
    /// <summary>
    /// Interaction logic for Home.xaml
    /// </summary>
    public partial class HomePage : Page
    {
        public HomePage()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // View Expense Report
            //ResultPage result = new ResultPage();
            //this.NavigationService.Navigate(result);
            Test result = new Test();
            this.NavigationService.Navigate(result);
        }

        private void Button_ClickTest2(object sender, RoutedEventArgs e)
        {
            Test2 result = new Test2();
            this.NavigationService.Navigate(result);
        }
    }
}
