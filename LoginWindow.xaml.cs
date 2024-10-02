using System;
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
using System.Windows.Shapes;

namespace WpfApp7
{
    /// <summary>
    /// Логика взаимодействия для LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        public bool IsAdmin { get; private set; }
        public LoginWindow()
        {
            InitializeComponent();
        }
        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameTextBox.Text;
            string password = PasswordBox.Password;

            // Проверяем, является ли пользователь администратором
            if (username == "admin" && password == "admin")
            {
                IsAdmin = true;
                MessageBox.Show("Вы успешно зашли как Администратор.");
                this.DialogResult = true; // Закрываем окно и возвращаем результат
            }
            else
            {
                IsAdmin = false;
                MessageBox.Show("Неправильный логин или пароль.");
            }
        }
    }
}
