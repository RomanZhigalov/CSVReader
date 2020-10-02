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
        private DataSet ds;

        public MainWindow()
        {
            InitializeComponent();
            comboBox.SelectedItem = comma;
            ds = new DataSet();
        }
        private void Load_Button_Click(object sender, RoutedEventArgs e)
        {
            string filePath = OpenCSVFileDialog();
            if(filePath != "")
            {
                LoadCSV(filePath, ds, MainDataGrid);
                FileName.Text = filePath;
            }
        }
        private void LoadCSV (string filePath, DataSet ds, DataGrid dg)
        {
            DataTable dt = new DataTable();
            ReadCSV(filePath, dt);
            if (dt.Columns.Count != 0)
            {
                dg.ItemsSource = dt.DefaultView;
            }
            ds.Clear();
            ds.Tables.Add(dt);
        }
        private string OpenCSVFileDialog()
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Cursor Files|*.csv";
            dialog.Title = "Выберите файл в формате .csv";
            dialog.ShowDialog();
            return dialog.FileName;
        }
        private string SaveCSVFileDialog()
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "Cursor Files|*.csv";
            dialog.ShowDialog();
            dialog.Title = "Выберите место сохранения файла";
            return dialog.FileName;
        }
        private void Save_Button_Click(object sender, RoutedEventArgs e)
        {
            if(ds.Tables[0].Columns.Count == 0)
            {
                MessageBox.Show("Нет данных.");
                return;
            }
            string filePath = SaveCSVFileDialog();
            if (filePath != "")
            {
                WriteCSV(filePath, ds.Tables[0]);
                FileName.Text = filePath;
                LoadCSV(filePath, ds, MainDataGrid);
            }
        }
        private void ReadCSV(string filePath, DataTable dt)
        {
            if (filePath.Length != 0)
            {
                using (StreamReader sr = new StreamReader(filePath, Encoding.UTF8))
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
        private void WriteCSV(string filePath, DataTable dt)
        {
            string temp = "";    
            using (StreamWriter sw = new StreamWriter(filePath))
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
        private void Clear_Button_Click(object sender, RoutedEventArgs e)
        {
            MainDataGrid.ItemsSource = null;
            FileName.Text = "";
        }
        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;
            TextBlock item = (TextBlock)comboBox.SelectedItem;
            if (item.Text == "Запятая")
            {
                splitter = ',';
            }
            else if(item.Text == "Точка с запятой")
            {
                splitter = ';';
            }  
        }
    }
}
