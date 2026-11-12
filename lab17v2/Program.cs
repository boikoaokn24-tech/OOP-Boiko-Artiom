using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading;

namespace lab17v2
{
    public static class RetryHelper
    {
        public static T ExecuteWithRetry<T>(
            Func<T> operation,
            int retryCount = 3,
            TimeSpan initialDelay = default,
            Func<Exception, bool>? shouldRetry = null)
        {
            if (operation == null)
                throw new ArgumentNullException(nameof(operation));

            if (initialDelay == default)
                initialDelay = TimeSpan.FromMilliseconds(500);

            int attempt = 0;

            while (true)
            {
                try
                {
                    attempt++;
                    Console.WriteLine($"[RetryHelper] Спроба #{attempt}...");
                    T result = operation();
                    Console.WriteLine($"[RetryHelper] Спроба #{attempt} успішна!");
                    return result;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[RetryHelper] Помилка на спробі #{attempt}: {ex.GetType().Name} — {ex.Message}");

                    bool canRetry = shouldRetry == null || shouldRetry(ex);

                    if (!canRetry)
                    {
                        Console.WriteLine($"[RetryHelper] Виняток {ex.GetType().Name} не підлягає повторній спробі (shouldRetry повернув false).");
                        throw;
                    }

                    if (attempt > retryCount)
                    {
                        Console.WriteLine($"[RetryHelper] Вичерпано максимальну кількість повторних спроб ({retryCount}).");
                        throw;
                    }

                    // Експоненційна затримка: initialDelay * 2^(attempt - 1)
                    double multiplier = Math.Pow(2, attempt - 1);
                    TimeSpan delay = TimeSpan.FromMilliseconds(initialDelay.TotalMilliseconds * multiplier);

                    Console.WriteLine($"[RetryHelper] Очікування {delay.TotalMilliseconds} мс перед наступною спробою...\n");
                    Thread.Sleep(delay);
                }
            }
        }
    }

    public class FileProcessor
    {
        private int _attemptCount = 0;

        public void WriteToFile(string path, string content)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentException("Шлях до файлу не може бути порожнім.", nameof(path));

            _attemptCount++;

            // Імітувати IOException перші 3 рази, потім успіх
            if (_attemptCount <= 3)
            {
                throw new IOException($"Файл '{path}' заблокований іншим процесом. Неможливо виконати запис.");
            }

            Console.WriteLine($"[FileProcessor] Дані \"{content}\" успішно записано у файл '{path}'.");
        }
    }

    public class NetworkClient
    {
        private int _attemptCount = 0;

        public bool UploadFile(string url, string filePath)
        {
            _attemptCount++;

            // Імітувати HttpRequestException перші 2 рази, потім успіх
            if (_attemptCount <= 2)
            {
                throw new HttpRequestException($"Тимчасова помилка мережі при завантаженні '{filePath}' на {url} (503 Service Unavailable).");
            }

            Console.WriteLine($"[NetworkClient] Файл '{filePath}' успішно завантажено на сервер {url}.");
            return true;
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;

            // Повторювати лише для IOException та HttpRequestException (згідно з Варіантом 2)
            Func<Exception, bool> retryPolicy = ex =>
                ex is IOException || ex is HttpRequestException;

            Console.WriteLine("=== Сценарій 1: Запис у файл (3 невдачі -> успіх) ===");
            var fileProcessor = new FileProcessor();
            try
            {
                bool writeSuccess = RetryHelper.ExecuteWithRetry(
                    operation: () =>
                    {
                        fileProcessor.WriteToFile("reports/report2026.txt", "Звіт за лабораторну №17");
                        return true;
                    },
                    retryCount: 4,
                    initialDelay: TimeSpan.FromMilliseconds(300),
                    shouldRetry: retryPolicy
                );
                Console.WriteLine($"РЕЗУЛЬТАТ: Запис завершено ({writeSuccess})\n");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"КРИТИЧНИЙ ЗБІЙ: {ex.Message}\n");
            }

            Console.WriteLine("=== Сценарій 2: Завантаження файлу на сервер (2 невдачі -> успіх) ===");
            var networkClient = new NetworkClient();
            try
            {
                bool uploadSuccess = RetryHelper.ExecuteWithRetry(
                    operation: () => networkClient.UploadFile("https://storage.example.com/upload", "reports/report2026.txt"),
                    retryCount: 3,
                    initialDelay: TimeSpan.FromMilliseconds(300),
                    shouldRetry: retryPolicy
                );
                Console.WriteLine($"РЕЗУЛЬТАТ: Завантаження на сервер = {uploadSuccess}\n");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"КРИТИЧНИЙ ЗБІЙ: {ex.Message}\n");
            }

            Console.WriteLine("=== Сценарій 3: Перевірка shouldRetry (нетимчасовий виняток) ===");
            try
            {
                RetryHelper.ExecuteWithRetry(
                    operation: () =>
                    {
                        fileProcessor.WriteToFile("", "Тест");
                        return true;
                    },
                    retryCount: 3,
                    initialDelay: TimeSpan.FromMilliseconds(300),
                    shouldRetry: retryPolicy
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Операцію перервано одразу без повторів: {ex.GetType().Name}\n");
            }
        }
    }
}