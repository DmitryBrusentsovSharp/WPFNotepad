using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
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

namespace WpfApp2
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            //this.Title = "Hello to All";
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
        }

        private void exitProgram_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void saveNewFile_Click(object sender, RoutedEventArgs e)
        {
            saveFile();
        }

        private void saveFile()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            bool? res = saveFileDialog.ShowDialog();

            if (res != false) { 
            using (Stream s = File.Open(saveFileDialog.FileName, FileMode.OpenOrCreate))
                {
                    using (StreamWriter sw = new StreamWriter(s))
                    {
                        sw.Write(textBox.Text);
                    }
                }
            }
        }

        private void openNewFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            bool? res = openFileDialog.ShowDialog();

            if (res != false)
            {
                Stream myStream;
                if ((myStream = openFileDialog.OpenFile()) != null) {
                    string file_name = openFileDialog.FileName;
                    string file_text = File.ReadAllText(file_name);
                    textBox.Text = file_text;
                }
            }
        }

        private void timesNewRomanFont_Click(object sender, RoutedEventArgs e)
        {
            textBox.FontFamily = new FontFamily("Times New Roman");
            verdanaFont.IsChecked = false;
        }

        private void verdanaFont_Click(object sender, RoutedEventArgs e)
        {
            textBox.FontFamily = new FontFamily("Verdana");
            timesNewRomanFont.IsChecked = false;
        }

        private void selectFontSize_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string fontSize = selectFontSize.SelectedItem.ToString();
            fontSize = fontSize.Substring(fontSize.Length - 2);
            
            switch (fontSize)
            {
                case "10":
                    textBox.FontSize = 10;
                    break;
                case "14":
                    textBox.FontSize = 14;
                    break;
                case "16":
                    textBox.FontSize = 16;
                    break;
                case "20":
                    textBox.FontSize = 20;
                    break;
                case "24":
                    textBox.FontSize = 24;
                    break;
                case "48":
                    textBox.FontSize = 48;
                    break;
            }
        }

        private void createNewFile_Click(object sender, RoutedEventArgs e)
        {
            if (textBox.Text != "") {
                saveFile();
            }
            textBox.Text = "";
        }

        private void regButton_Click(object sender, RoutedEventArgs e)
        {
            string connectionString = ConfigurationManager.AppSettings["connectionString"];

            SqlConnection sql = new SqlConnection(connectionString);

            try
            {
                if (sql.State == System.Data.ConnectionState.Closed)
                    sql.Open();

                string querry = "SELECT COUNT(1) FROM Users WHERE login=@login AND password=@pass";
                SqlCommand sqlCom = new SqlCommand(querry, sql);
                sqlCom.CommandType = System.Data.CommandType.Text;
                sqlCom.Parameters.Add("@login", loginField.Text);
                sqlCom.Parameters.Add("@pass", passField.Password);

                int countUser = Convert.ToInt32(sqlCom.ExecuteScalar());
                if (countUser == 0)
                {
                    querry = "INSERT INTO Users (login, password) VALUES(@login, @pass)";
                    SqlCommand com = new SqlCommand(querry, sql);
                    com.CommandType = System.Data.CommandType.Text;
                    com.Parameters.Add("@login", loginField.Text);
                    com.Parameters.Add("@pass", passField.Password);

                    com.ExecuteNonQuery();
                    MessageBox.Show("Мы добавили вас в Базу Данных");
                }
                else 
                {
                    MessageBox.Show("Вы успешно авторизовались");
                    AuthPage auth = new AuthPage();
                    auth.Show();

                    MainWindow mw = new MainWindow();
                    mw.Hide();

                    App.Current.MainWindow.Hide();
                }

            } catch (Exception ex) 
            {
                MessageBox.Show(ex.Message);
            } finally 
            {
                sql.Close();
            }
        }
    }
}
