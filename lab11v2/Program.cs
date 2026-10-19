using System;
using System.Collections.Generic;

namespace lab11v2
{
    // Абстрактний базовий клас, що реалізує патерн Template Method
    public abstract class DataImporter
    {
        // Властивості стану обробки
        public string SourcePath { get; set; } = string.Empty;
        public int ImportedRecordsCount { get; protected set; }

        // Шаблонний метод (Template Method) — визначає скелет алгоритму
        public void Import()
        {
            Console.WriteLine($"=== Початок процесу імпорту з джерела: {SourcePath} ===");
            Connect();
            ReadData();
            Validate();
            Transform();
            SaveToDatabase();
            Disconnect();
            Console.WriteLine($"Імпорт завершено успішно. Опрацьовано записів: {ImportedRecordsCount}\n");
        }

        // Крок 1: Підключення (абстрактний метод)
        protected abstract void Connect();

        // Крок 2: Читання даних (абстрактний метод)
        protected abstract void ReadData();

        // Крок 3: Валідація (віртуальний метод з базовою реалізацією, за бажанням можна перевизначити)
        protected virtual void Validate()
        {
            Console.WriteLine("[Базова валідація] Перевірка структури та цілісності даних...");
        }

        // Крок 4: Трансформація (абстрактний метод)
        protected abstract void Transform();

        // Крок 5: Збереження в базу даних (віртуальний метод з базовою логікою)
        protected virtual void SaveToDatabase()
        {
            Console.WriteLine($"[БД] Збереження {ImportedRecordsCount} записів у реляційну базу даних...");
        }

        // Крок 6: Відключення (віртуальний метод)
        protected virtual void Disconnected()
        {
            Console.WriteLine("Відключення від джерела даних.");
        }

        // Залишаємо сумісність з назвою із завдання (Disconnect)
        protected virtual void Disconnect()
        {
            Disconnected();
        }
    }

    // Похідний клас 1: Імпорт з CSV
    public class CSVImporter : DataImporter
    {
        public char Delimiter { get; set; } = ',';

        protected override void Connect()
        {
            Console.WriteLine($"[CSV] Відкриття файлу за шляхом: {SourcePath} (розділювач: '{Delimiter}')");
        }

        protected override void ReadData()
        {
            Console.WriteLine("[CSV] Читання рядків файлу CSV...");
            ImportedRecordsCount = 150; // Умовно прочитано 150 рядків
        }

        protected override void Transform()
        {
            Console.WriteLine("[CSV] Перетворення CSV-рядків у внутрішні об'єкти сутностей...");
        }
    }

    // Похідний клас 2: Імпорт з JSON
    public class JSONImporter : DataImporter
    {
        public bool ValidateSchema { get; set; } = true;

        protected override void Connect()
        {
            Console.WriteLine($"[JSON] Підключення до JSON-потоку або читання файлу: {SourcePath}");
        }

        protected override void ReadData()
        {
            Console.WriteLine("[JSON] Десеріалізація JSON-документа...");
            ImportedRecordsCount = 320; // Умовно 320 об'єктів
        }

        protected override void Validate()
        {
            base.Validate();
            if (ValidateSchema)
            {
                Console.WriteLine("[JSON] Додаткова перевірка за JSON Schema успішна.");
            }
        }

        protected override void Transform()
        {
            Console.WriteLine("[JSON] Мапінг JSON полів у моделі даних...");
        }
    }

    // Похідний клас 3: Імпорт з XML
    public class XMLImporter : DataImporter
    {
        public string RootNodeName { get; set; } = "Root";

        protected override void Connect()
        {
            Console.WriteLine($"[XML] Завантаження XML-документа з кореневим вузлом <{RootNodeName}>: {SourcePath}");
        }

        protected override void ReadData()
        {
            Console.WriteLine("[XML] Парсинг XML-вузлів за допомогою XmlReader...");
            ImportedRecordsCount = 85; // Умовно 85 елементів
        }

        protected override void Transform()
        {
            Console.WriteLine("[XML] Конвертація XML-елементів у формат бази даних...");
        }

        // Перевизначаємо збереження під специфіку XML
        protected override void SaveToDatabase()
        {
            Console.WriteLine($"[БД - XML] Спеціальне збереження {ImportedRecordsCount} XML-вузлів через збережену процедуру.");
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            // Налаштування кодування для коректного відображення української мови в консолі
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            // Створення колекції об'єктів різних похідних класів типу базового класу (DataImporter)
            List<DataImporter> importers = new List<DataImporter>
            {
                new CSVImporter { SourcePath = "/data/users.csv", Delimiter = ';' },
                new JSONImporter { SourcePath = "https://api.example.com/products.json", ValidateSchema = true },
                new XMLImporter { SourcePath = "C:\\Configs\\settings.xml", RootNodeName = "Configuration" }
            };

            // Демонстрація поліморфного виклику шаблонного методу для кожного об'єкта
            foreach (var importer in importers)
            {
                importer.Import();
            }

            Console.WriteLine("Усі процеси імпорту даних успішно завершено.");
        }
    }
}