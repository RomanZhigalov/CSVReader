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
using System.Configuration;
using System.Data.SqlClient;

namespace CSVReader
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private char splitter;
        private string connection;
        private SqlDataAdapter adapter;
        DataSet ds = new DataSet();

        public MainWindow()
        {
            InitializeComponent();
            splitter = ',';
            connection = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        }

        private void Load_Button_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Cursor Files|*.csv";
            dialog.Title = "Выберите файл в формате .csv";
            if(dialog.ShowDialog() == true)
            {
                DataTable dt = new DataTable();
                ds.Tables.Clear();
                ds.Tables.Add(dt);
                ReadFromCSV(dialog.FileName, dt);
                if (dt.Columns.Count != 0)
                {
                    MainDataGrid.ItemsSource = ds.Tables[0].DefaultView;
                    MessageBox.Show($"File \"{dialog.FileName}\" loaded successfully.");
                }
                if (dt.Columns.Count == 0)
                {
                    MessageBox.Show($"File \"{dialog.FileName}\" is empty.");
                }
            }


        }

        private void Save_Button_Click(object sender, RoutedEventArgs e)
        {
            if(MainDataGrid.Columns.Count <= 0)
            {
                MessageBox.Show("There is no data to export.");
                return;
            }
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "Cursor Files|*.csv";
            if (dialog.ShowDialog() == true)
            {
                WriteToCSV(dialog.FileName, ds.Tables[0]);
            }
            MessageBox.Show($"File \"{dialog.FileName}\" saved successfully.");
        }

        private void ReadFromCSV(string filePath, DataTable dt)
        {
            if (filePath.Length != 0)
            {
                using (StreamReader sr = new StreamReader(filePath, Encoding.Default))
                {
                    try
                    {
                        dt.Rows.Clear();
                        dt.Columns.Clear();
                        string[] headers = sr.ReadLine().Split(splitter);
                        foreach (string header in headers)
                        {
                            dt.Columns.Add(header);
                        }
                        while (!sr.EndOfStream)
                        {
                            string[] rows = sr.ReadLine().Split(splitter);
                            DataRow dr = dt.NewRow();
                            for (int i = 0; i < headers.Length; i++)
                            {
                                dr[i] = rows[i];
                            }
                            dt.Rows.Add(dr);
                        }
                    }
                    catch(Exception ex)
                    {
                        MessageBox.Show(ex.GetType().ToString());
                    }
                }
            }
        }

        private void WriteToCSV(string fileName, DataTable dt)
        {
            string temp = "";    
            //DataView dv = (DataView)MainDataGrid.Items.SourceCollection;
            using (StreamWriter sw = new StreamWriter(fileName))
            {
                if (dt.Columns.Count > 0)
                {
                    for (int column = 0; column <= dt.Columns.Count - 1; column++)
                    {
                        if (column > 0)
                        {
                            sw.Write(splitter);
                        }
                        sw.Write(dt.Columns[column].ColumnName);
                    }
                    sw.WriteLine();
                    for (int row = 0; row <= dt.Rows.Count - 1; row++)
                    {
                        if (row > 0)
                        {
                            sw.WriteLine();
                        }
                        for (int column = 0; column <= dt.Columns.Count - 1; column++)
                        {
                            if (column > 0)
                            {
                                sw.Write(splitter);
                            }
                            temp = dt.Rows[row].ItemArray[column].ToString();
                            temp = temp.Replace(splitter, ' ');
                            temp = temp.Replace(Environment.NewLine, " ");
                            sw.Write(temp);
                        }
                    }
                }
            }            
        }

        private void DataTableToDB()
        {

        }

        private void Clear_Button_Click(object sender, RoutedEventArgs e)
        {
            MainDataGrid.ItemsSource = null;
        }
    }
}
