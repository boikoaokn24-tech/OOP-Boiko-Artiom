using System;

// 1. Клас згідно з Варіантом 2 (DatabaseConnection)
public class DatabaseConnection : IDisposable
{
    private bool _disposed = false;
    private string _connectionString;
    private bool _isConnected;

    public DatabaseConnection(string connectionString)
    {
        _connectionString = connectionString;
        _isConnected = true;
        Console.WriteLine($"[DatabaseConnection] Успішно підключено до бази даних за рядком: '{_connectionString}'.");
    }

    public void ExecuteQuery(string query)
    {
        if (_disposed)
        {
            throw new ObjectDisposedException(nameof(DatabaseConnection), "Неможливо виконати запит через закріплене/закрите з'єднання.");
        }

        if (_isConnected)
        {
            Console.WriteLine($"[DB QUERY EXECUTED]: {query}");
        }
        else
        {
            Console.WriteLine("[DatabaseConnection] Попередження: з'єднання з базою даних закрито.");
        }
    }

    // Захищений віртуальний метод Dispose відповідно до патерну Dispose
    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                // Звільнення керованих ресурсів (якщо вони є)
                Console.WriteLine($"[DatabaseConnection] Звільнення керованих ресурсів для з'єднання '{_connectionString}'.");
            }

            // Звільнення некерованих ресурсів (імітація закриття з'єднання з БД)
            if (_isConnected)
            {
                Console.WriteLine($"[DatabaseConnection] Закриття з'єднання з базою даних (некерований ресурс).");
                _isConnected = false;
            }

            _disposed = true;
        }
    }

    // Публічний метод Dispose для інтерфейсу IDisposable
    public void Dispose()
    {
        Dispose(true);
        // Запобігає виклику фіналізатора (деструктора), оскільки ресурси вже звільнені вручну
        GC.SuppressFinalize(this);
    }

    // Деструктор (фіналізатор)
    ~DatabaseConnection()
    {
        Dispose(false);
    }
}

class Program
{
    static void Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        // Сценарій 1: Використання оператора using
        Console.WriteLine("--- Сценарій 1: Використання оператора using ---");
        {
            using var db1 = new DatabaseConnection("Server=127.0.0.1;Database=users_db;Trusted_Connection=True;");
            db1.ExecuteQuery("SELECT * FROM Users;");
        } // Тут автоматично викликається Dispose() для db1
        Console.WriteLine();

        // Сценарій 2: Явний виклик Dispose()
        Console.WriteLine("--- Сценарій 2: Явний виклик Dispose() ---");
        {
            DatabaseConnection db2 = new DatabaseConnection("Server=127.0.0.1;Database=orders_db;Trusted_Connection=True;");
            db2.ExecuteQuery("INSERT INTO Orders (Item, Amount) VALUES ('Laptop', 1);");
            
            // Явне звільнення ресурсів до завершення блоку
            db2.Dispose();
            
            // Повторний виклик Dispose() є безпечним завдяки перевірці _disposed
            db2.Dispose();
        }
        Console.WriteLine();

        // Сценарій 3: Робота через деструктор (GC.Collect)
        Console.WriteLine("--- Сценарій 3: Демонстрація деструктора через GC.Collect() ---");
        RunGarbageCollectionTest();

        // Примусовий запуск збирання сміття та очікування фіналізаторів
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect(); // Фіналізатор відпрацює тут

        Console.WriteLine("\nКінець програми.");
    }

    // Допоміжний метод для створення об'єкта в області видимості, що завершується
    static void RunGarbageCollectionTest()
    {
        DatabaseConnection db3 = new DatabaseConnection("Server=127.0.0.1;Database=logs_db;Trusted_Connection=True;");
        db3.ExecuteQuery("SELECT * FROM SystemLogs;");
        // Посилання на db3 зникає після виходу з методу, 
        // об'єкт стає кандидатом на збирання сміття.
    }
}