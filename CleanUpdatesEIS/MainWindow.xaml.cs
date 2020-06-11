using System;
using System.IO;
using System.Windows;
using System.Windows.Data;
using Microsoft.Win32;
using MySql.Data.MySqlClient;

namespace CleanUpdatesEIS
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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string WinReg = "";
            if (System.Environment.Is64BitOperatingSystem == true) WinReg = "WOW6432Node\\";
            string InstallPath = (string)Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\" + WinReg + @"Microsoft\Windows\CurrentVersion\Uninstall\eNot_is1", "Inno Setup: App Path", null);
            if (InstallPath == null) InstallPath = @"C:\Triasoft\eNot";
            string Update = InstallPath + @"\Update";
            try
            {
                Directory.Delete(Update, true);
                MessageBox.Show("Папка с обновлениями ЕИС очищена.");
            }
            catch
            {
                MessageBox.Show("Не удалось очистить папку Updates. Закройте Енот / Adobe Reader / Express и попробуйте снова.");
            }

            string Database = InputDB.Text;
            if (Database == "") Database = "enotdb";
            string Source = InputServer.Text;
            if (Source == "") Source = "localhost";
            string User = InputName.Text;
            if (User == "") User = "enot";
            string Password = InputPassword.Text;
            string con = "Server=" + Source + ";Database=" + Database + ";Uid=" + User + ";Pwd=" + Password + ";SslMode=Preferred";

            if (Password != "")
            {
                try
                {
                    MySqlConnection connection = new MySqlConnection(con);
                    string query = "TRUNCATE TABLE " + "SoftwareUpdates";
                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    connection.Open();
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("База данных обновлений ЕИС очищена.");
                }
                catch
                {
                    MessageBox.Show("Не удалось подключиться к серверу MySql. Проверьте правильность введеных данных. \n \n");
                }
            }

        }
    }
    public class TextInputToVisibilityConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            // Always test MultiValueConverter inputs for non-null 
            // (to avoid crash bugs for views in the designer) 
            if (values[0] is bool && values[1] is bool)
            {
                bool hasText = !(bool)values[0];
                bool hasFocus = (bool)values[1];
                if (hasFocus || hasText)
                    return Visibility.Collapsed;
            }
            return Visibility.Visible;
        }
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
