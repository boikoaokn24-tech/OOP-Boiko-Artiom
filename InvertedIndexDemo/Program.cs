using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace InvertedIndexDemo
{
    // 1. Модель документа
    public class Document
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;

        public override string ToString() => $"[{Id}] {Title}";
    }

    // 2. Утиліти для нормалізації та токенізації тексту
    public static class TextProcessor
    {
        // Шаблон зберігає слова (включно з C# / C++) та прибирає пунктуацію
        private static readonly Regex WordRegex = new Regex(@"[a-zа-яіїєґ0-9]+(?:#|\+\+)?", RegexOptions.Compiled);

        private static readonly HashSet<string> StopWords = new HashSet<string>
        {
            "a", "an", "the", "is", "are", "was", "were",
            "in", "on", "at", "to", "for", "of", "and", "or"
        };

        /// <summary>
        /// Розбиває текст на слова, видаляє пунктуацію, переводить у нижній регістр
        /// </summary>
        public static IEnumerable<string> Tokenize(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return Enumerable.Empty<string>();

            string lower = text.ToLowerInvariant();
            return WordRegex.Matches(lower).Select(m => m.Value);
        }

        /// <summary>
        /// Видаляє стоп-слова (a, an, the, is, are, etc.)
        /// </summary>
        public static IEnumerable<string> RemoveStopWords(IEnumerable<string> words)
        {
            return words.Where(w => !StopWords.Contains(w));
        }

        /// <summary>
        /// Повна нормалізація для індексу та лінійного пошуку
        /// </summary>
        public static IEnumerable<string> Normalize(string text)
        {
            return RemoveStopWords(Tokenize(text));
        }
    }

    // 3. Реалізація інвертованого індексу
    public class InvertedIndex
    {
        private readonly Dictionary<string, HashSet<int>> _index;
        private readonly Dictionary<int, Document> _documents;

        public InvertedIndex()
        {
            _index = new Dictionary<string, HashSet<int>>();
            _documents = new Dictionary<int, Document>();
        }

        /// <summary>
        /// Додає документ до індексу
        /// </summary>
        public void AddDocument(Document doc)
        {
            if (doc == null) throw new ArgumentNullException(nameof(doc));

            _documents[doc.Id] = doc;

            var words = TextProcessor.Normalize(doc.Content);
            foreach (var word in words)
            {
                if (!_index.TryGetValue(word, out var docIds))
                {
                    docIds = new HashSet<int>();
                    _index[word] = docIds;
                }
                docIds.Add(doc.Id);
            }
        }

        /// <summary>
        /// Шукає документи, що містять одне слово
        /// </summary>
        public List<Document> SearchSingleWord(string word)
        {
            var normalized = TextProcessor.Normalize(word).FirstOrDefault();
            if (string.IsNullOrEmpty(normalized) || !_index.TryGetValue(normalized, out var docIds))
                return new List<Document>();

            return docIds.Select(id => _documents[id]).ToList();
        }

        /// <summary>
        /// Шукає документи, що містять ВСІ слова (AND)
        /// </summary>
        public List<Document> SearchMultipleWords(params string[] words)
        {
            var normalizedWords = words
                .SelectMany(w => TextProcessor.Normalize(w))
                .Distinct()
                .ToList();

            if (normalizedWords.Count == 0)
                return new List<Document>();

            HashSet<int>? resultIds = null;

            foreach (var word in normalizedWords)
            {
                if (!_index.TryGetValue(word, out var currentIds))
                {
                    // Якщо хоча б одного слова немає — перетин порожній
                    return new List<Document>();
                }

                if (resultIds == null)
                {
                    resultIds = new HashSet<int>(currentIds);
                }
                else
                {
                    resultIds.IntersectWith(currentIds);
                }
            }

            return resultIds == null
                ? new List<Document>()
                : resultIds.Select(id => _documents[id]).ToList();
        }

        /// <summary>
        /// Шукає документи, що містять БУДЬ-ЯКЕ слово (OR)
        /// </summary>
        public List<Document> SearchAnyWord(params string[] words)
        {
            var normalizedWords = words
                .SelectMany(w => TextProcessor.Normalize(w))
                .Distinct()
                .ToList();

            var resultIds = new HashSet<int>();

            foreach (var word in normalizedWords)
            {
                if (_index.TryGetValue(word, out var currentIds))
                {
                    resultIds.UnionWith(currentIds);
                }
            }

            return resultIds.Select(id => _documents[id]).ToList();
        }

        public int DocumentCount => _documents.Count;
        public int UniqueWords => _index.Keys.Count;
    }

    // 4. Вимірювання продуктивності
    public static class PerformanceTest
    {
        public static List<Document> GenerateDocuments(int count)
        {
            var baseTemplates = GetSampleDocuments();
            var extraTopics = new[]
            {
                "databases sql nosql indexing caching distributed systems",
                "algorithms data structures hash tables trees graphs sorting",
                "web development rest api microservices docker kubernetes cloud",
                "machine learning neural networks data science statistics",
                "cybersecurity encryption authentication protocols networking"
            };

            var rand = new Random(42);
            var generated = new List<Document>(count);

            for (int i = 1; i <= count; i++)
            {
                var template = baseTemplates[(i - 1) % baseTemplates.Count];
                string extra = extraTopics[rand.Next(extraTopics.Length)];
                generated.Add(new Document
                {
                    Id = i,
                    Title = $"{template.Title} #{i}",
                    Content = $"{template.Content} Additional notes on {extra} document {i}."
                });
            }

            return generated;
        }

        public static void CompareSearchMethods(List<Document> documents, string searchWord)
        {
            var sw = new Stopwatch();

            // Підготовка індексу (побудова відбувається один раз заздалегідь)
            var index = new InvertedIndex();
            foreach (var doc in documents)
            {
                index.AddDocument(doc);
            }

            // Прогрів JIT-компілятора перед вимірюванням
            _ = documents.FirstOrDefault(d => TextProcessor.Normalize(d.Content).Contains("c#"));
            _ = index.SearchSingleWord("c#");

            // 1. Лінійний пошук (O(N * M))
            var normalizedWord = TextProcessor.Normalize(searchWord).First();
            sw.Restart();
            var linearResults = documents
                .Where(d => TextProcessor.Normalize(d.Content).Contains(normalizedWord))
                .ToList();
            sw.Stop();
            long linearTicks = sw.ElapsedTicks;
            double linearMs = sw.Elapsed.TotalMilliseconds;

            // 2. Пошук через Inverted Index (O(1))
            sw.Restart();
            var indexResults = index.SearchSingleWord(searchWord);
            sw.Stop();
            long indexTicks = sw.ElapsedTicks;
            double indexMs = sw.Elapsed.TotalMilliseconds;

            double speedup = indexTicks > 0 ? (double)linearTicks / indexTicks : 0;

            Console.WriteLine($"Документів: {documents.Count,-6} | Linear: {linearTicks,8} ticks ({linearMs,7:F3} ms) | Index: {indexTicks,5} ticks ({indexMs,6:F4} ms) | Прискорення: {speedup,6:F1}x | Знайдено: {linearResults.Count}/{indexResults.Count}");
        }

        public static List<Document> GetSampleDocuments()
        {
            return new List<Document>
            {
                new Document
                {
                    Id = 1,
                    Title = "Introduction to C#",
                    Content = "C# is a modern object-oriented programming language developed by Microsoft."
                },
                new Document
                {
                    Id = 2,
                    Title = "Java Basics",
                    Content = "Java is a popular programming language known for its portability and performance."
                },
                new Document
                {
                    Id = 3,
                    Title = "Python Overview",
                    Content = "Python is a high-level programming language with simple syntax."
                },
                new Document
                {
                    Id = 4,
                    Title = "C# Advanced Features",
                    Content = "C# supports LINQ, async/await, and many modern programming paradigms."
                },
                new Document
                {
                    Id = 5,
                    Title = "Comparing Languages",
                    Content = "C#, Java, and Python are among the most popular programming languages today."
                }
            };
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;

            // Частина 1: Тест продуктивності на 10, 100, 1000, 10000 документах
            Console.WriteLine("=== ЧАСТИНА 1: Порівняння продуктивності (слово пошуку: 'python') ===");
            int[] sizes = { 10, 100, 1000, 10000 };
            foreach (int size in sizes)
            {
                var docs = PerformanceTest.GenerateDocuments(size);
                PerformanceTest.CompareSearchMethods(docs, "python");
            }

            // Частина 2: Інтерактивна демонстрація на 5 базових документах
            Console.WriteLine("\n=== ЧАСТИНА 2: Інтерактивний повнотекстовий пошук ===");
            var index = new InvertedIndex();
            foreach (var doc in PerformanceTest.GetSampleDocuments())
            {
                index.AddDocument(doc);
            }

            Console.WriteLine($"Loaded {index.DocumentCount} documents");
            Console.WriteLine($"Index contains {index.UniqueWords} unique words\n");

            while (true)
            {
                Console.Write("Enter search query (or 'exit'): ");
                var query = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(query) || query.Trim().Equals("exit", StringComparison.OrdinalIgnoreCase))
                    break;

                var words = query.Split(' ', StringSplitOptions.RemoveEmptyEntries);

                Console.WriteLine("\n--- Search Results (AND) ---");
                var andResults = index.SearchMultipleWords(words);
                foreach (var doc in andResults)
                {
                    Console.WriteLine(doc);
                    Console.WriteLine($"  {doc.Content.Substring(0, Math.Min(100, doc.Content.Length))}...");
                }
                Console.WriteLine($"Found (AND): {andResults.Count} documents");

                if (words.Length > 1)
                {
                    Console.WriteLine("\n--- Search Results (OR) ---");
                    var orResults = index.SearchAnyWord(words);
                    foreach (var doc in orResults)
                    {
                        Console.WriteLine(doc);
                    }
                    Console.WriteLine($"Found (OR): {orResults.Count} documents");
                }
                Console.WriteLine();
            }
        }
    }
}