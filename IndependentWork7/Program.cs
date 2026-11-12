using System;
using System.Collections.Generic;
using System.Text;

namespace IndependentWork7
{
    // 1. Узагальнений клас FIFO-кешу
    public class Cache<TKey, TValue> where TKey : notnull
    {
        private readonly int _capacity;
        private readonly Dictionary<TKey, TValue> _cache;
        private readonly Queue<TKey> _keysOrder;

        public Cache(int capacity)
        {
            if (capacity <= 0)
                throw new ArgumentOutOfRangeException(nameof(capacity), "Ємність кешу повинна бути більшою за нуль.");

            _capacity = capacity;
            _cache = new Dictionary<TKey, TValue>(capacity);
            _keysOrder = new Queue<TKey>(capacity);
        }

        public int Count => _cache.Count;

        public void Add(TKey key, TValue value)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            if (_cache.ContainsKey(key))
            {
                _cache[key] = value;
                return;
            }

            if (_cache.Count >= _capacity)
            {
                TKey oldestKey = _keysOrder.Dequeue();
                _cache.Remove(oldestKey);
            }

            _cache.Add(key, value);
            _keysOrder.Enqueue(key);
        }

        public TValue Get(TKey key)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            if (_cache.TryGetValue(key, out TValue? value))
            {
                return value!;
            }

            throw new KeyNotFoundException($"Ключ '{key}' не знайдено в кеші.");
        }

        public bool ContainsKey(TKey key)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            return _cache.ContainsKey(key);
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;

            // Частина 1: Демонстрація роботи кешу
            Console.WriteLine("=== 1. Демонстрація роботи Cache<string, string> (ємність = 3) ===");
            var cache = new Cache<string, string>(3);

            cache.Add("user:1", "Артем Бойко");
            cache.Add("user:2", "Іван Коваль");
            cache.Add("user:3", "Олена Мельник");

            Console.WriteLine($"Додано 3 елементи. Count = {cache.Count}");
            Console.WriteLine($"Отримано 'user:1': {cache.Get("user:1")}");

            Console.WriteLine("Додаємо 4-й елемент ('user:4') -> спрацьовує витіснення FIFO...");
            cache.Add("user:4", "Максим Шевченко");

            Console.WriteLine($"Count після переповнення = {cache.Count}");
            Console.WriteLine($"ContainsKey('user:1') = {cache.ContainsKey("user:1")} (витіснено)");
            Console.WriteLine($"ContainsKey('user:4') = {cache.ContainsKey("user:4")} (додано)\n");

            // Частина 2: Модульні тести (перевірка всіх сценаріїв)
            Console.WriteLine("=== 2. Запуск модульних тестів для Cache<TKey, TValue> ===");
            RunAllTests();
        }

        static void RunAllTests()
        {
            int passed = 0;

            void RunTest(string testName, Action testAction)
            {
                testAction();
                passed++;
                Console.WriteLine($"[PASS] {testName}");
            }

            // Тест 1: Додавання та отримання елементів
            RunTest("Add_And_Get_ReturnsCorrectValues", () =>
            {
                var c = new Cache<string, int>(2);
                c.Add("a", 10);
                c.Add("b", 20);
                if (c.Get("a") != 10 || c.Get("b") != 20)
                    throw new Exception("Отримано неправильне значення.");
            });

            // Тест 2: Властивість Count та оновлення існуючого ключа
            RunTest("Count_And_UpdateExistingKey_WorksCorrectly", () =>
            {
                var c = new Cache<int, string>(2);
                if (c.Count != 0) throw new Exception("Початковий Count має бути 0.");
                c.Add(1, "old");
                c.Add(1, "new");
                if (c.Count != 1 || c.Get(1) != "new")
                    throw new Exception("Оновлення ключа зламало Count або значення.");
            });

            // Тест 3: Перевірка ContainsKey
            RunTest("ContainsKey_ReturnsExpectedBoolean", () =>
            {
                var c = new Cache<string, string>(2);
                c.Add("exists", "yes");
                if (!c.ContainsKey("exists") || c.ContainsKey("missing"))
                    throw new Exception("ContainsKey повернув хибний результат.");
            });

            // Тест 4: Політика витіснення FIFO при перевищенні ємності
            RunTest("Add_ExceedingCapacity_EvictsOldestElement_FIFO", () =>
            {
                var c = new Cache<int, string>(3);
                c.Add(1, "First");
                c.Add(2, "Second");
                c.Add(3, "Third");

                _ = c.Get(1); // Звернення до 1-го не змінює чергу FIFO
                c.Add(4, "Fourth"); // Має витіснити ключ 1

                if (c.Count != 3 || c.ContainsKey(1) || !c.ContainsKey(2) || !c.ContainsKey(4))
                    throw new Exception("Порушено політику витіснення FIFO.");
            });

            // Тест 5: Викидання KeyNotFoundException для відсутнього ключа
            RunTest("Get_MissingKey_ThrowsKeyNotFoundException", () =>
            {
                var c = new Cache<string, string>(2);
                c.Add("k1", "v1");
                bool thrown = false;
                try
                {
                    c.Get("unknown");
                }
                catch (KeyNotFoundException)
                {
                    thrown = true;
                }
                if (!thrown) throw new Exception("Не було викинуто KeyNotFoundException.");
            });

            Console.WriteLine($"\nУсі тести успішно пройдено: {passed}/{passed}!");
        }
    }
}