using System;
using System.Collections.Generic;
using System.Text;

namespace lab9_self_v2
{
    // Статичний клас утиліт для роботи з колекціями через делегати
    public static class CollectionUtils
    {
        // Метод 1: Перетворення кожного елемента (аналог LINQ Select)
        public static IEnumerable<TResult> Map<TSource, TResult>(
            IEnumerable<TSource> collection,
            Func<TSource, TResult> mapper)
        {
            if (collection == null) throw new ArgumentNullException(nameof(collection));
            if (mapper == null) throw new ArgumentNullException(nameof(mapper));

            var result = new List<TResult>();
            foreach (var item in collection)
            {
                result.Add(mapper(item));
            }
            return result;
        }

        // Метод 2: Пошук елемента з максимальним значенням ключа (аналог LINQ MaxBy)
        public static T MaxBy<T, TKey>(
            IEnumerable<T> collection,
            Func<T, TKey> keySelector)
            where TKey : IComparable<TKey>
        {
            if (collection == null) throw new ArgumentNullException(nameof(collection));
            if (keySelector == null) throw new ArgumentNullException(nameof(keySelector));

            T maxItem = default!;
            TKey maxKey = default!;
            bool first = true;

            foreach (var item in collection)
            {
                TKey key = keySelector(item);
                if (first || key.CompareTo(maxKey) > 0)
                {
                    maxKey = key;
                    maxItem = item;
                    first = false;
                }
            }

            if (first)
                throw new InvalidOperationException("Колекція порожня, неможливо знайти максимальний елемент.");

            return maxItem;
        }

        // Метод 3: Фільтрація елементів за умовою (аналог LINQ Where)
        public static IEnumerable<T> Filter<T>(
            IEnumerable<T> collection,
            Predicate<T> predicate)
        {
            if (collection == null) throw new ArgumentNullException(nameof(collection));
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));

            var result = new List<T>();
            foreach (var item in collection)
            {
                if (predicate(item))
                {
                    result.Add(item);
                }
            }
            return result;
        }

        // Метод 4: Виконання дії для кожного елемента
        public static void ForEach<T>(
            IEnumerable<T> collection,
            Action<T> action)
        {
            if (collection == null) throw new ArgumentNullException(nameof(collection));
            if (action == null) throw new ArgumentNullException(nameof(action));

            foreach (var item in collection)
            {
                action(item);
            }
        }
    }

    // Користувацький клас для демонстрації (Колекція 1)
    public class Product
    {
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string Category { get; set; } = string.Empty;
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;

            // =========================================================================
            // КОЛЕКЦІЯ 1: List<Product> (Товари магазину)
            // =========================================================================
            Console.WriteLine("=== 1. Робота з колекцією List<Product> ===");
            var products = new List<Product>
            {
                new Product { Name = "Ноутбук", Price = 32000m, Category = "Електроніка" },
                new Product { Name = "Мишка", Price = 650m, Category = "Периферія" },
                new Product { Name = "Клавіатура", Price = 1800m, Category = "Периферія" },
                new Product { Name = "Монітор", Price = 11500m, Category = "Електроніка" }
            };

            // Map: проєкція об'єктів у відформатовані рядки
            var productLabels = CollectionUtils.Map(products, p => $"{p.Name} ({p.Price} грн)");
            Console.WriteLine("Усі товари (Map): " + string.Join(", ", productLabels));

            // MaxBy: найдорожчий товар
            var mostExpensive = CollectionUtils.MaxBy(products, p => p.Price);
            Console.WriteLine($"Найдорожчий товар (MaxBy): {mostExpensive.Name} — {mostExpensive.Price} грн");

            // Filter + ForEach: товари дорожчі за 1500 грн
            var expensiveProducts = CollectionUtils.Filter(products, p => p.Price > 1500m);
            Console.WriteLine("Товари дорожчі за 1500 грн (Filter + ForEach):");
            CollectionUtils.ForEach(expensiveProducts, p => Console.WriteLine($"  * {p.Name} [{p.Category}] — {p.Price} грн"));

            // =========================================================================
            // КОЛЕКЦІЯ 2: List<int> (Цілі числа)
            // =========================================================================
            Console.WriteLine("\n=== 2. Робота з колекцією List<int> ===");
            var numbers = new List<int> { 7, 14, 3, 22, 9, 30, 11 };

            // Filter: лише парні числа
            var evenNumbers = CollectionUtils.Filter(numbers, n => n % 2 == 0);
            Console.WriteLine("Парні числа (Filter): " + string.Join(", ", evenNumbers));

            // Map: піднесення до квадрату
            var squares = CollectionUtils.Map(evenNumbers, n => n * n);
            Console.WriteLine("Квадрати парних чисел (Map): " + string.Join(", ", squares));

            // MaxBy: число з найбільшою останньою цифрою (n % 10)
            int maxLastDigit = CollectionUtils.MaxBy(numbers, n => n % 10);
            Console.WriteLine($"Число з найбільшою останньою цифрою (MaxBy): {maxLastDigit}");

            // ForEach: підрахунок суми через замикання лямбда-виразу
            int sum = 0;
            CollectionUtils.ForEach(numbers, n => sum += n);
            Console.WriteLine($"Сума всіх чисел (ForEach із замиканням): {sum}");

            // =========================================================================
            // КОЛЕКЦІЯ 3: List<string> (Рядки / Імена студентів)
            // =========================================================================
            Console.WriteLine("\n=== 3. Робота з колекцією List<string> ===");
            var words = new List<string> { "Делегат", "Лямбда", "Інкапсуляція", "Поліморфізм", "Клас" };

            // MaxBy: найдовше слово за властивістю Length
            string longestWord = CollectionUtils.MaxBy(words, w => w.Length);
            Console.WriteLine($"Найдовше слово (MaxBy за Length): '{longestWord}' ({longestWord.Length} символів)");

            // Filter: слова довші за 6 символів
            var longWords = CollectionUtils.Filter(words, w => w.Length > 6);

            // Map + ForEach: переведення у верхній регістр та виведення
            var upperLongWords = CollectionUtils.Map(longWords, w => w.ToUpper());
            Console.Write("Довгі слова у верхньому регістрі (Filter -> Map -> ForEach): ");
            CollectionUtils.ForEach(upperLongWords, w => Console.Write($"[{w}] "));
            Console.WriteLine();
        }
    }
}