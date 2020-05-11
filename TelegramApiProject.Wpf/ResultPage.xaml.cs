using System;
using System.Collections.Generic;
using System.Data;
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
    /// Interaction logic for Result.xaml
    /// </summary>
    public partial class ResultPage : Page
    {
        public ResultPage()
        {
            InitializeComponent();
            CreateDataTable();
        }

        private void CreateDataTable()
        {
            System.Data.DataTable dt = new DataTable("MyTable");
            dt.Columns.Add("MyColumn", typeof(string));
            dt.Rows.Add("row of data");
        }
    }
}
