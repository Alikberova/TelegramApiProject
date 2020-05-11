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
    /// Interaction logic for Test.xaml
    /// </summary>
    public partial class Test : Page
    {
        public Test()
        {
            InitializeComponent();
            Create2();
        }

        private void Create2()
        {
            int cols = 2;
            int rows = 5;

            for (int i = 0; i < cols; i++)
            {
                myTable.Columns.Add(new TableColumn());
            }

            for (int i = 0; i < rows; i++)
            {
                TableRow row = new TableRow();

                for (int y = 0; y < cols; y++)
                {
                    //row.Cells.Add(new TableCell(new Paragraph(new Run("Some Text"))));
                    if (y % 2 != 0)
                    {
                        row.Cells.Add(new TableCell(new Paragraph(new Run("Some Text"))));
                    }
                    row.Cells.Add(new TableCell(new Paragraph(new Run("Some value"))));
                }

                TableRowGroup rowGroup = new TableRowGroup();
                rowGroup.Rows.Add(row);
                myTable.RowGroups.Add(rowGroup);
            }
        }

        private void Create()
        {
            int cols = 2;
            int rows = 10;

            for (int c = 0; c < cols; c++)
                myTable.Columns.Add(new TableColumn());

            for (int r = 0; r < rows; r++)
            {
                TableRow tr = new TableRow();

                for (int c = 0; c < cols; c++)
                    tr.Cells.Add(new TableCell(new Paragraph(new Run("Some Text"))));

                TableRowGroup trg = new TableRowGroup();
                trg.Rows.Add(tr);
                myTable.RowGroups.Add(trg);
            }
        }
    }
}
