using System;
using System.Collections.Generic;

namespace lab13v2
{
    // Кастомний клас для тестування
    public class Product
    {
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public double Rating { get; set; }

        public override string ToString() => $"Товар: {Name}, Ціна: {Price:C}, Рейтинг: {Rating}";
    }

    // Клас з Generic-методами
    public static class FunctionalUtils
    {
        // 1. Map<T, TResult>: Перетворює список List<T> в List<TResult>
        public static List<TResult> Map<T, TResult>(List<T> list, Func<T, TResult> mapper)
        {
            var result = new List<TResult>();
            foreach (var item in list)
            {
                result.Add(mapper(item));
            }
            return result;
        }

        // 2. Filter: Повертає елементи, що задовольняють умові Func<T, bool>
        public static List<T> Filter<T>(List<T> list, Func<T, bool> predicate)
        {
            var result = new List<T>();
            foreach (var item in list)
            {
                if (predicate(item))
                {
                    result.Add(item);
                }
            }
            return result;
        }

        // 3. MaxBy: Повертає елемент з максимальним значенням ключа
        public static T MaxBy<T, TKey>(List<T> list, Func<T, TKey> keySelector) 
            where TKey : IComparable<TKey>
        {
            if (list == null || list.Count == 0) 
                throw new ArgumentException("Список порожній");
            
            T maxItem = list[0];
            TKey maxKey = keySelector(maxItem);
            
            foreach (var item in list)
            {
                TKey key = keySelector(item);
                if (key.CompareTo(maxKey) > 0)
                {
                    maxKey = key;
                    maxItem = item;
                }
            }
            return maxItem;
        }

        // 4. MinBy: Повертає елемент з мінімальним значенням ключа
        public static T MinBy<T, TKey>(List<T> list, Func<T, TKey> keySelector) 
            where TKey : IComparable<TKey>
        {
            if (list == null || list.Count == 0) 
                throw new ArgumentException("Список порожній");
            
            T minItem = list[0];
            TKey minKey = keySelector(minItem);
            
            foreach (var item in list)
            {
                TKey key = keySelector(item);
                if (key.CompareTo(minKey) < 0)
                {
                    minKey = key;
                    minItem = item;
                }
            }
            return minItem;
        }

        // 5. FirstOrDefault: Повертає перший елемент або default(T)
        public static T? FirstOrDefault<T>(List<T> list, Func<T, bool> predicate)
        {
            foreach (var item in list)
            {
                if (predicate(item))
                {
                    return item;
                }
            }
            return default;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            // --- ТЕСТОВІ ДАНІ ---
            var numbers = new List<int> { 10, 25, 3, 42, 17, 8 };
            var words = new List<string> { "Яблуко", "Кіт", "Університет", "Дім", "Програмування" };
            var products = new List<Product>
            {
                new Product { Name = "Ноутбук", Price = 1500m, Rating = 4.8 },
                new Product { Name = "Мишка", Price = 25m, Rating = 4.2 },
                new Product { Name = "Монітор", Price = 300m, Rating = 4.9 },
                new Product { Name = "Клавіатура", Price = 50m, Rating = 4.0 }
            };

            // --- ДЕМОНСТРАЦІЯ Map ---
            Console.WriteLine("=== 1. Map ===");
            var stringNumbers = FunctionalUtils.Map(numbers, n => $"Число {n}");
            Console.WriteLine("Map (int -> string): " + string.Join(", ", stringNumbers));

            // --- ДЕМОНСТРАЦІЯ Filter ---
            Console.WriteLine("\n=== 2. Filter ===");
            var evenNumbers = FunctionalUtils.Filter(numbers, n => n % 2 == 0);
            Console.WriteLine("Filter (парні числа): " + string.Join(", ", evenNumbers));

            // --- ДЕМОНСТРАЦІЯ MaxBy ---
            Console.WriteLine("\n=== 3. MaxBy ===");
            var longestWord = FunctionalUtils.MaxBy(words, w => w.Length);
            Console.WriteLine($"MaxBy (найдовше слово): {longestWord}");

            // --- ДЕМОНСТРАЦІЯ MinBy ---
            Console.WriteLine("\n=== 4. MinBy ===");
            var cheapestProduct = FunctionalUtils.MinBy(products, p => p.Price);
            Console.WriteLine($"MinBy (найдешевший товар): {cheapestProduct.Name} ({cheapestProduct.Price:C})");

            // --- ДЕМОНСТРАЦІЯ FirstOrDefault ---
            Console.WriteLine("\n=== 5. FirstOrDefault ===");
            var firstWordStartingWithU = FunctionalUtils.FirstOrDefault(words, w => w.StartsWith("У"));
            Console.WriteLine($"FirstOrDefault (слово на 'У'): {firstWordStartingWithU}");
            
            var missingProduct = FunctionalUtils.FirstOrDefault(products, p => p.Price < 5);
            Console.WriteLine($"FirstOrDefault (товар < $5): {(missingProduct != null ? missingProduct.Name : "Не знайдено")}");
            
            Console.ReadLine();
        }
    }
}