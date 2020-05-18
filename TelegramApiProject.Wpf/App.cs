using System.Windows;

namespace TelegramApiProject.Wpf
{
    public class App : Application
    {
        public App()
        {
            MainWindow main = new MainWindow();
            main.Show();
        }

        public void InitializeComponent()
        {
        }
    }
}
