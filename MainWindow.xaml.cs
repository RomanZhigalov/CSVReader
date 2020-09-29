using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using System.Data;

namespace CSVReader
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Load_Button_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Cursor Files|*.csv";
            dialog.Title = "Выберите файл в формате .csv";
            if(dialog.ShowDialog() == true)
            {
                MainDataGrid.ItemsSource = ReadFromCSV(dialog.FileName).DefaultView;
            }
            MessageBox.Show($"File \"{dialog.FileName}\" loaded successfully.");
        }

        private void Save_Button_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "Cursor Files|*.csv";
            if (dialog.ShowDialog() == true)
            {
                SaveToCSV(dialog.FileName);
            }
            MessageBox.Show($"File \"{dialog.FileName}\" saved successfully.");
        }

        private DataTable ReadFromCSV(string filePath)
        {
            DataTable dt = new DataTable();
            if (filePath != null)
            {
                using (StreamReader sr = new StreamReader(filePath))
                {
                    string[] headers = sr.ReadLine().Split(',');

                    foreach (string header in headers)
                    {
                        dt.Columns.Add(header);
                    }
                    while (!sr.EndOfStream)
                    {
                        string[] rows = sr.ReadLine().Split(',');
                        DataRow dr = dt.NewRow();
                        for (int i = 0; i < headers.Length; i++)
                        {
                            dr[i] = rows[i];
                        }
                        dt.Rows.Add(dr);
                    }
                }
            }
            return dt;
        }

        private void SaveToCSV(string fileName)
        {
            string temp = "";         
            DataView dv = new DataView();
            using (StreamWriter sw = new StreamWriter(fileName))
            {
                if (MainDataGrid.Columns.Count > 0)
                {
                    for (int i = 0; i <= MainDataGrid.Columns.Count - 1; i++)
                    {
                        if (i > 0)
                        {
                            sw.Write(',');
                        }
                        sw.Write(MainDataGrid.Columns[i].Header);
                    }
                    sw.WriteLine();
                    dv = (DataView)MainDataGrid.Items.SourceCollection;
                    for (int j = 0; j <= MainDataGrid.Columns.Count - 1; j++)
                    {
                        if (j > 0)
                        {
                            sw.WriteLine();
                        }
                        for (int i = 0; i <= MainDataGrid.Columns.Count - 1; i++)
                        {
                            if (i > 0)
                            {
                                sw.Write(",");
                            }
                            temp = dv.Table.Rows[j].ItemArray[i].ToString();
                            temp = temp.Replace(',', ' ');
                            temp = temp.Replace(Environment.NewLine, " ");
                            sw.Write(temp);
                        }
                    }
                }
            }            
        }
    }
}
