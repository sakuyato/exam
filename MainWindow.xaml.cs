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
using System;
using System.Data.SQLite;
using System.Collections.ObjectModel;
using Microsoft.EntityFrameworkCore;

namespace WpfApp7
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
        public double Discount { get; set; }

        public double DiscountedPrice
        {
            get
            {
                return Price - (Price * Discount / 100);
            }
        }
    }
    public partial class MainWindow : Window
    {
        private bool _isAdmin = false;
        private ObservableCollection<Product> _products = new ObservableCollection<Product>();

        public MainWindow()
        {
            InitializeComponent();
            EnsureDatabaseExists();
            LoadProducts();
            ProductListView.ItemsSource = _products;
            FilterComboBox.SelectedIndex = 4; // Пятый элемент в ComboBox ("Name (A-Z)")
            ReloadProductsWithFilter(); // Применяем фильтр сразу при запуске
        }
        private void EnsureDatabaseExists()
        {
            using (var connection = new SQLiteConnection("Data Source=products.db"))
            {
                connection.Open();
                string createTableQuery = @"
                CREATE TABLE IF NOT EXISTS Products (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Name TEXT NOT NULL,
                Price REAL NOT NULL,
                Discount REAL NOT NULL
                )";
                using (var cmd = new SQLiteCommand(createTableQuery, connection))
                {
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private void LoadProducts()
        {
            _products.Clear();  // Очищаем коллекцию перед загрузкой новых данных
            using (var connection = new SQLiteConnection("Data Source=products.db"))
            {
                connection.Open();
                string query = "SELECT * FROM Products";
                using (var cmd = new SQLiteCommand(query, connection))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            _products.Add(new Product
                            {
                                Id = reader.GetInt32(0),
                                Name = reader.GetString(1),
                                Price = reader.GetDouble(2),
                                Discount = reader.GetDouble(3)
                            });
                        }
                    }
                }
            }
        }

        private void AddProductButton_Click(object sender, RoutedEventArgs e)
        {
            if (!_isAdmin)
            {
                MessageBox.Show("Вы не имеете доступа к данному разделу.");
                return;
            }
            // Открытие окна добавления продукта
            AddProductWindow addProductWindow = new AddProductWindow();
            addProductWindow.ShowDialog();
            ReloadProductsWithFilter(); // Обновляем список продуктов

        }
        private void EditProductButton_Click(object sender, RoutedEventArgs e)
        {
            if (!_isAdmin)
            {
                MessageBox.Show("Вы не имеете доступа к данному разделу.");
                return;
            }
            if (ProductListView.SelectedItem is Product selectedProduct)
            {
                // Открытие окна редактирования с выбранным продуктом
                EditProductWindow editProductWindow = new EditProductWindow(selectedProduct);
                editProductWindow.ShowDialog();
                ReloadProductsWithFilter(); // Обновляем список продуктов после редактирования
            }
        }
        private void DeleteProductButton_Click(object sender, RoutedEventArgs e)
        {
            if (!_isAdmin)
            {
                MessageBox.Show("Вы не имеете доступа к данному разделу.");
                return;
            }
            if (ProductListView.SelectedItem is Product selectedProduct)
            {
                MessageBoxResult result = MessageBox.Show("Вы уверены что хотите удалить данный товар?", "Confirmation", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    using (var connection = new SQLiteConnection("Data Source=products.db"))
                    {
                        connection.Open();
                        string query = "DELETE FROM Products WHERE Id=@id";
                        using (var cmd = new SQLiteCommand(query, connection))
                        {
                            cmd.Parameters.AddWithValue("@id", selectedProduct.Id);  // Удаляем по ID
                            cmd.ExecuteNonQuery();
                        }
                    }
                    ReloadProductsWithFilter(); // Обновляем список продуктов после удаления
                }
            }
        }
        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            string searchTerm = SearchBox.Text.ToLower();
            var filteredProducts = new ObservableCollection<Product>();
            foreach (var product in _products)
            {
                if (product.Name.ToLower().Contains(searchTerm))
                {
                    filteredProducts.Add(product);
                }
            }
            ProductListView.ItemsSource = filteredProducts;
        }
        private void FilterProducts(string filterType)
        {
            IEnumerable<Product> filteredProducts = _products;

            switch (filterType)
            {
                case "DiscountDesc":
                    filteredProducts = _products.OrderByDescending(p => p.Discount);
                    break;
                case "DiscountAsc":
                    filteredProducts = _products.OrderBy(p => p.Discount);
                    break;
                case "PriceDesc":
                    filteredProducts = _products.OrderByDescending(p => p.Price);
                    break;
                case "PriceAsc":
                    filteredProducts = _products.OrderBy(p => p.Price);
                    break;
                case "NameAZ":
                    filteredProducts = _products.OrderBy(p => p.Name);
                    break;
                case "NameZA":
                    filteredProducts = _products.OrderByDescending(p => p.Name);
                    break;
                case "DiscountPriceDesc":
                    filteredProducts = _products.OrderByDescending(p => p.DiscountedPrice);
                    break;
                case "DiscountPriceAsc":
                    filteredProducts = _products.OrderBy(p => p.DiscountedPrice);
                    break;
            }

            ProductListView.ItemsSource = new ObservableCollection<Product>(filteredProducts);
        }
        private void FilterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (FilterComboBox.SelectedItem is ComboBoxItem selectedItem)
            {
                string filterType = selectedItem.Tag.ToString();
                FilterProducts(filterType);
            }
        }
        private void ReloadProductsWithFilter()
        {
            // Загружаем данные из базы заново
            LoadProducts();

            // Получаем текущий выбранный фильтр
            if (FilterComboBox.SelectedItem is ComboBoxItem selectedItem)
            {
                string filterType = selectedItem.Tag.ToString();
                FilterProducts(filterType); // Применяем фильтр к загруженным данным
            }
        }
        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            AuthenticateUser();
        }
        private void AuthenticateUser()
        {
            // Открываем окно авторизации
            LoginWindow loginWindow = new LoginWindow();
            if (loginWindow.ShowDialog() == true)
            {
                _isAdmin = loginWindow.IsAdmin; // Получаем информацию, является ли пользователь администратором
            }
        }
    }
}