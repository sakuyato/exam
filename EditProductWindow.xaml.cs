using System;
using System.Data.SQLite;
using System.Windows;

namespace WpfApp7
{
    public partial class EditProductWindow : Window
    {
        private Product _product;

        public EditProductWindow(Product product)
        {
            InitializeComponent();
            _product = product;
            LoadProductData();
        }

        private void LoadProductData()
        {
            // Загружаем данные продукта в поля
            NameTextBox.Text = _product.Name;
            PriceTextBox.Text = _product.Price.ToString();
            DiscountTextBox.Text = _product.Discount.ToString();
        }

        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            string newName = NameTextBox.Text;
            if (double.TryParse(PriceTextBox.Text, out double newPrice) &&
                double.TryParse(DiscountTextBox.Text, out double newDiscount))
            {
                if (newPrice < 0)
                {
                    MessageBox.Show("Неправильная цена.");
                    return;
                }

                if (newDiscount < 0 || newDiscount > 100)
                {
                    MessageBox.Show("Неправильная скидка.");
                    return;
                }

                using (var connection = new SQLiteConnection("Data Source=products.db"))
                {
                    connection.Open();
                    string query = "UPDATE Products SET Name=@name, Price=@price, Discount=@discount WHERE Id=@id";
                    using (var cmd = new SQLiteCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@name", newName);
                        cmd.Parameters.AddWithValue("@price", newPrice);
                        cmd.Parameters.AddWithValue("@discount", newDiscount);
                        cmd.Parameters.AddWithValue("@id", _product.Id);  // Обновляем по ID
                        cmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Продукт успешно изменен!");
                Close();
            }
            else
            {
                MessageBox.Show("Ошибка! Введите коректную информацию.");
            }
        }
    }
}
