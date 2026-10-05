using System;

namespace IndependentWork2
{
    public class Product
    {
        // Приватні поля
        private int _id;
        private string _name;
        private decimal _price;
        private string _category;
        private int _stockCount;

        // Публічні властивості (read-only)
        public int Id => _id;
        public string Name => _name;
        public decimal Price => _price;
        public string Category => _category;
        public int StockCount => _stockCount;

        // Конструктор 1 (основний)
        public Product(int id, string name, decimal price, string category, int stockCount)
        {
            _id = id;
            _name = name;
            _price = price;
            _category = category;
            _stockCount = stockCount;
        }

        // Конструктор 2 (для швидкого додавання товару через делегування : this(...))
        public Product(int id, string name, decimal price)
            : this(id, name, price, "Uncategorized", 0)
        {
        }

        // Конструктор 3 (конструктор копіювання через делегування : this(...))
        public Product(Product other)
            : this(other.Id, other.Name, other.Price, other.Category, other.StockCount)
        {
        }

        // Перевизначення методу ToString()
        public override string ToString()
        {
            return $"ID: {Id}, Name: {Name}, Price: {Price:C}, Category: {Category}, Stock: {StockCount}";
        }
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Console.WriteLine("==================================================");
            Console.WriteLine("  САМОСТІЙНА РОБОТА №2");
            Console.WriteLine("  Тема: Перевантаження конструкторів: приклади та сценарії");
            Console.WriteLine("==================================================\n");

            Console.WriteLine("Створення товарів:\n");

            // 1. Товар за допомогою основного конструктора
            Product product1 = new Product(101, "Laptop", 35000.00m, "Electronics", 15);
            Console.WriteLine($"Товар 1 (основний конструктор): {product1}");

            // 2. Товар за допомогою скороченого конструктора
            Product product2 = new Product(102, "Mouse", 800.00m);
            Console.WriteLine($"Товар 2 (скорочений конструктор): {product2}");

            // 3. Товар за допомогою конструктора копіювання
            Product product3 = new Product(product1);
            Console.WriteLine($"Товар 3 (конструктор копіювання): {product3}");

            Console.WriteLine("\n==================================================");
            Console.WriteLine("  Демонстрацію роботи конструкторів завершено");
            Console.WriteLine("==================================================");
        }
    }
}