using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IndependentWork9
{
    // =========================================================
    // Сценарій 1: Кеш метаданих (Dictionary<string, FileMetadata>)
    // =========================================================
    public class FileMetadata
    {
        public string FileName { get; set; }
        public long Size { get; set; }
        public DateTime LastModified { get; set; }
        public string Hash { get; set; }

        public FileMetadata(string fileName, long size, DateTime lastModified, string hash)
        {
            FileName = fileName;
            Size = size;
            LastModified = lastModified;
            Hash = hash;
        }

        public override string ToString()
        {
            return $"{FileName} ({Size} байт, змінено: {LastModified:dd.MM.yyyy HH:mm}, SHA256: {Hash})";
        }
    }

    public class FileMetadataCache
    {
        // Використовуємо StringComparer.OrdinalIgnoreCase для коректного пошуку шляхів у Windows
        private readonly Dictionary<string, FileMetadata> _cache =
            new Dictionary<string, FileMetadata>(StringComparer.OrdinalIgnoreCase);

        public void Add(string filePath, FileMetadata metadata)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentException("Шлях до файлу не може бути порожнім.", nameof(filePath));

            _cache[filePath] = metadata ?? throw new ArgumentNullException(nameof(metadata));
        }

        public FileMetadata? Get(string filePath)
        {
            return _cache.TryGetValue(filePath, out var metadata) ? metadata : null;
        }

        public bool Remove(string filePath)
        {
            return _cache.Remove(filePath);
        }

        public bool Contains(string filePath)
        {
            return _cache.ContainsKey(filePath);
        }

        public int Count => _cache.Count;
    }

    // =========================================================
    // Сценарій 2: Унікальні теги (Dictionary + HashSet операції)
    // =========================================================
    public class TagManager
    {
        private readonly Dictionary<string, HashSet<string>> _articleTags =
            new Dictionary<string, HashSet<string>>(StringComparer.OrdinalIgnoreCase);

        public void AddTag(string articleId, string tag)
        {
            if (!_articleTags.TryGetValue(articleId, out var tags))
            {
                tags = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                _articleTags[articleId] = tags;
            }
            tags.Add(tag.Trim());
        }

        public bool RemoveTag(string articleId, string tag)
        {
            if (_articleTags.TryGetValue(articleId, out var tags))
            {
                return tags.Remove(tag.Trim());
            }
            return false;
        }

        // Об'єднання всіх тегів (UnionWith)
        public HashSet<string> GetAllUniqueTags()
        {
            var allTags = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var tagSet in _articleTags.Values)
            {
                allTags.UnionWith(tagSet);
            }
            return allTags;
        }

        // Перетин тегів двох статей (IntersectWith)
        public HashSet<string> FindCommonTags(string articleId1, string articleId2)
        {
            if (!_articleTags.TryGetValue(articleId1, out var tags1) ||
                !_articleTags.TryGetValue(articleId2, out var tags2))
            {
                return new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            }

            var common = new HashSet<string>(tags1, StringComparer.OrdinalIgnoreCase);
            common.IntersectWith(tags2);
            return common;
        }

        // Різниця тегів: які теги є у articleId1, але відсутні у articleId2 (ExceptWith)
        public HashSet<string> FindUniqueTagsForFirst(string articleId1, string articleId2)
        {
            if (!_articleTags.TryGetValue(articleId1, out var tags1))
                return new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            var uniqueToFirst = new HashSet<string>(tags1, StringComparer.OrdinalIgnoreCase);
            if (_articleTags.TryGetValue(articleId2, out var tags2))
            {
                uniqueToFirst.ExceptWith(tags2);
            }
            return uniqueToFirst;
        }
    }

    // =========================================================
    // Сценарій 3: Інвертований пошуковий індекс (Dictionary + HashSet)
    // =========================================================
    public class SearchIndex
    {
        // Слово -> Множина ID документів, де воно зустрічається
        private readonly Dictionary<string, HashSet<int>> _invertedIndex =
            new Dictionary<string, HashSet<int>>(StringComparer.OrdinalIgnoreCase);

        private static readonly char[] Separators =
            { ' ', ',', '.', '!', '?', ';', ':', '-', '\n', '\r', '\t', '"', '(', ')' };

        public void AddDocument(int docId, string content)
        {
            if (string.IsNullOrWhiteSpace(content)) return;

            string[] words = content.Split(Separators, StringSplitOptions.RemoveEmptyEntries);
            foreach (string word in words)
            {
                if (!_invertedIndex.TryGetValue(word, out var docIds))
                {
                    docIds = new HashSet<int>();
                    _invertedIndex[word] = docIds;
                }
                docIds.Add(docId);
            }
        }

        public HashSet<int> Search(string word)
        {
            if (_invertedIndex.TryGetValue(word, out var docIds))
            {
                return new HashSet<int>(docIds);
            }
            return new HashSet<int>();
        }

        public HashSet<int> SearchMultiple(params string[] words)
        {
            if (words == null || words.Length == 0)
                return new HashSet<int>();

            HashSet<int>? result = null;

            foreach (string word in words)
            {
                if (!_invertedIndex.TryGetValue(word, out var currentDocs))
                {
                    // Якщо хоча б одного слова немає в індексі, перетин порожній
                    return new HashSet<int>();
                }

                if (result == null)
                {
                    result = new HashSet<int>(currentDocs);
                }
                else
                {
                    result.IntersectWith(currentDocs);
                }
            }

            return result ?? new HashSet<int>();
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;

            // --- Демонстрація Сценарію 1 ---
            Console.WriteLine("=== Сценарій 1: Кеш метаданих файлів (Dictionary) ===");
            var fileCache = new FileMetadataCache();
            fileCache.Add(@"C:\Docs\report.pdf", new FileMetadata("report.pdf", 204800, DateTime.Now.AddHours(-2), "a1b2c3d4"));
            fileCache.Add(@"C:\Docs\data.json", new FileMetadata("data.json", 4096, DateTime.Now, "e5f6g7h8"));

            Console.WriteLine($"Кількість у кеші: {fileCache.Count}");
            Console.WriteLine($"Пошук без урахування регістру (c:\\docs\\REPORT.PDF): {fileCache.Contains(@"c:\docs\REPORT.PDF")}");
            Console.WriteLine($"Отримані дані: {fileCache.Get(@"C:\Docs\report.pdf")}");
            fileCache.Remove(@"C:\Docs\data.json");
            Console.WriteLine($"Кількість після видалення data.json: {fileCache.Count}\n");

            // --- Демонстрація Сценарію 2 ---
            Console.WriteLine("=== Сценарій 2: Управління тегами статей (HashSet) ===");
            var tagManager = new TagManager();
            tagManager.AddTag("art-1", "C#");
            tagManager.AddTag("art-1", ".NET");
            tagManager.AddTag("art-1", "Collections");
            tagManager.AddTag("art-1", "c#"); // Дублікат не додасться

            tagManager.AddTag("art-2", ".NET");
            tagManager.AddTag("art-2", "Performance");
            tagManager.AddTag("art-2", "Collections");

            Console.WriteLine($"Усі унікальні теги (UnionWith): [{string.Join(", ", tagManager.GetAllUniqueTags())}]");
            Console.WriteLine($"Спільні теги art-1 і art-2 (IntersectWith): [{string.Join(", ", tagManager.FindCommonTags("art-1", "art-2"))}]");
            Console.WriteLine($"Теги лише у art-1 (ExceptWith): [{string.Join(", ", tagManager.FindUniqueTagsForFirst("art-1", "art-2"))}]\n");

            // --- Демонстрація Сценарію 3 ---
            Console.WriteLine("=== Сценарій 3: Інвертований пошуковий індекс (Dictionary + HashSet) ===");
            var searchIndex = new SearchIndex();
            searchIndex.AddDocument(1, "C# та платформа .NET дозволяють швидко будувати веб-застосунки.");
            searchIndex.AddDocument(2, "Колекції Dictionary та HashSet забезпечують пошук за час O(1) у .NET.");
            searchIndex.AddDocument(3, "Мова C# використовує хеш-таблиці для реалізації Dictionary.");

            var singleSearch = searchIndex.Search("Dictionary");
            Console.WriteLine($"Документи зі словом 'Dictionary': [{string.Join(", ", singleSearch)}]");

            var multiSearch = searchIndex.SearchMultiple("C#", "Dictionary");
            Console.WriteLine($"Документи зі словами 'C#' ТА 'Dictionary': [{string.Join(", ", multiSearch)}]");

            var dotnetSearch = searchIndex.SearchMultiple(".NET", "C#");
            Console.WriteLine($"Документи зі словами '.NET' ТА 'C#': [{string.Join(", ", dotnetSearch)}]");
        }
    }
}