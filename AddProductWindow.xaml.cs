using System;
using System.Data.SQLite;
using System.Windows;
using WpfApp7;

namespace WpfApp7
{
    public partial class AddProductWindow : Window
    {
        public AddProductWindow()
        {
            InitializeComponent();
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            string name = NameTextBox.Text;

            // Проверка на пустые поля
            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(DiscountTextBox.Text) || string.IsNullOrWhiteSpace(PriceTextBox.Text))
            {
                MessageBox.Show("Поля не должны быть пустыми.");
                return;
            }

            // Проверка на корректность числовых значений
            if (double.TryParse(PriceTextBox.Text, out double price) &&
                double.TryParse(DiscountTextBox.Text, out double discount))
            {
                if (price < 0)
                {
                    MessageBox.Show("Неправильная цена.");
                    return;
                }

                if (discount < 0 || discount > 100)
                {
                    MessageBox.Show("Неправильная скидка.");
                    return;
                }
                // Добавление продукта в базу данных
                using (var connection = new SQLiteConnection("Data Source=products.db"))
                {
                    connection.Open();
                    string query = "INSERT INTO Products (Name, Price, Discount) VALUES (@name, @price, @discount)";
                    using (var cmd = new SQLiteCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@name", name);
                        cmd.Parameters.AddWithValue("@price", price);
                        cmd.Parameters.AddWithValue("@discount", discount);
                        cmd.ExecuteNonQuery();
                    }
                }
                MessageBox.Show("Продукт успешно добавлен");
                Close();
            }
            else
            {
                MessageBox.Show("Ошибка! Введите коректную информация в полях.");
            }
        }
    }
}
