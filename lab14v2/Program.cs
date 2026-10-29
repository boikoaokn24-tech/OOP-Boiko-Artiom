using System;
using System.Collections.Generic;

namespace lab14v2
{
    // Модель лог-запису
    public class LogEntry
    {
        public int Id { get; set; }
        public string IpAddress { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string LogLevel { get; set; } = "INFO";
        public DateTime Timestamp { get; set; } = DateTime.Now;

        public override string ToString()
        {
            return $"[{Timestamp:HH:mm:ss}] [{LogLevel}] IP: {IpAddress} - {Message}";
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Console.WriteLine("==================================================");
            Console.WriteLine("    ЛАБОРАТОРНА РОБОТА №14 (ВАРІАНТ 2: ОБРОБКА ЛОГІВ)");
            Console.WriteLine("==================================================\n");

            // --- 1. HASHSET: Збереження унікальних IP-адрес ---
            Console.WriteLine("--- 1. ДЕМОНСТРАЦІЯ HASHSET<T> (Унікальні IP-адреси) ---");
            HashSet<string> uniqueIpAddresses = new HashSet<string>();

            string[] incomingIps = {
                "192.168.1.10", 
                "10.0.0.1", 
                "192.168.1.10", // Дублікат
                "172.16.0.5", 
                "10.0.0.1"      // Дублікат
            };

            foreach (var ip in incomingIps)
            {
                bool added = uniqueIpAddresses.Add(ip);
                Console.WriteLine($"Спроба додати IP {ip,-15} -> {(added ? "Додано" : "Пропущено (дублікат)")}");
            }

            Console.WriteLine($"\nЗагальна кількість унікальних IP: {uniqueIpAddresses.Count}");
            Console.WriteLine("Перевірка наявності (Contains 10.0.0.1): " + uniqueIpAddresses.Contains("10.0.0.1"));
            Console.WriteLine("Список унікальних IP: " + string.Join(", ", uniqueIpAddresses));


            // --- 2. QUEUE: Черга логів для обробки (FIFO) ---
            Console.WriteLine("\n--- 2. ДЕМОНСТРАЦІЯ QUEUE<T> (Черга логів / FIFO) ---");
            Queue<LogEntry> logQueue = new Queue<LogEntry>();

            logQueue.Enqueue(new LogEntry { Id = 1, IpAddress = "192.168.1.10", Message = "Успішне входу в систему", LogLevel = "INFO" });
            logQueue.Enqueue(new LogEntry { Id = 2, IpAddress = "10.0.0.1", Message = "Помилка підключення до БД", LogLevel = "ERROR" });
            logQueue.Enqueue(new LogEntry { Id = 3, IpAddress = "172.16.0.5", Message = "Оновлення профілю користувача", LogLevel = "WARNING" });

            Console.WriteLine($"Всього логів у черзі: {logQueue.Count}");
            Console.WriteLine($"Перегляд першого в черзі (Peek): {logQueue.Peek()}");

            Console.WriteLine("\nПослідовна обробка логів із черги (Dequeue):");
            Stack<LogEntry> logHistory = new Stack<LogEntry>(); // Для наступного кроку

            while (logQueue.Count > 0)
            {
                LogEntry currentLog = logQueue.Dequeue();
                Console.WriteLine($" [Оброблено]: {currentLog}");
                
                // Додаємо оброблений лог в історію
                logHistory.Push(currentLog);
            }

            Console.WriteLine($"Залишок у черзі після обробки: {logQueue.Count}");


            // --- 3. STACK: Історія переглянутих/оброблених логів (LIFO) ---
            Console.WriteLine("\n--- 3. ДЕМОНСТРАЦІЯ STACK<T> (Історія перегляду / LIFO) ---");
            Console.WriteLine($"Всього логів в історії: {logHistory.Count}");
            Console.WriteLine($"Останній оброблений лог на вершині стеку (Peek): {logHistory.Peek()}");

            Console.WriteLine("\nСкасування/Перегляд логів з кінця історії (Pop):");
            while (logHistory.Count > 0)
            {
                LogEntry poppedLog = logHistory.Pop();
                Console.WriteLine($" [Вилучено з історії]: {poppedLog}");
            }

            Console.WriteLine($"Залишок в історії: {logHistory.Count}");
            Console.ReadLine();
        }
    }
}