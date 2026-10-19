using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace lab9v1
{
    // 1. Абстрактний базовий клас
    public abstract class Notification
    {
        public string Recipient { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;

        // Поліморфний метод відправки
        public abstract void Send();

        // Загальний метод валідації
        public abstract bool Validate();
    }

    // 2. Похідні класи

    public class EmailNotification : Notification
    {
        public override bool Validate()
        {
            string pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            return !string.IsNullOrEmpty(Recipient) && Regex.IsMatch(Recipient, pattern);
        }

        public override void Send()
        {
            if (!Validate())
            {
                throw new FormatException($"Помилка валідації: Некоректний формат email адреси -> '{Recipient}'.");
            }
            Console.WriteLine($"[Email] Відправка на {Recipient}: \"{Message}\"");
        }
    }

    public class SMSNotification : Notification
    {
        public override bool Validate()
        {
            string pattern = @"^\+\d{10,15}$";
            return !string.IsNullOrEmpty(Recipient) && Regex.IsMatch(Recipient, pattern);
        }

        public override void Send()
        {
            if (!Validate())
            {
                throw new FormatException($"Помилка валідації: Некоректний формат номера телефону -> '{Recipient}'.");
            }
            Console.WriteLine($"[SMS] Відправка на номер {Recipient}: \"{Message}\"");
        }
    }

    public class PushNotification : Notification
    {
        public override bool Validate()
        {
            return !string.IsNullOrEmpty(Recipient) && Recipient.Length >= 5;
        }

        public override void Send()
        {
            if (!Validate())
            {
                throw new ArgumentException($"Помилка валідації: Некоректний або занадто короткий Device ID -> '{Recipient}'.");
            }
            Console.WriteLine($"[Push] Відправка на пристрій (ID: {Recipient}): \"{Message}\"");
        }
    }

    // 3. Клас-сервіс для масової обробки сповіщень
    public class NotificationService
    {
        public void SendAll(List<Notification> notifications)
        {
            Console.WriteLine("=== Початок масової відправки сповіщень ===");
            int successCount = 0;
            int errorCount = 0;

            foreach (var notification in notifications)
            {
                try
                {
                    notification.Send();
                    successCount++;
                }
                catch (Exception ex)
                {
                    errorCount++;
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"[ПОМИЛКА] {ex.Message}");
                    Console.ResetColor();
                }
                Console.WriteLine(new string('-', 40));
            }

            Console.WriteLine($"=== Звіт: Успішно відправлено: {successCount}, Помилок: {errorCount} ===\n");
        }
    }

    // 4. Демонстрація роботи в Main
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            List<Notification> notifications = new List<Notification>
            {
                new EmailNotification 
                { 
                    Recipient = "student@colleg.rv.ua", 
                    Message = "Вашу лабораторну роботу прийнято!" 
                },
                new SMSNotification 
                { 
                    Recipient = "+380501234567", 
                    Message = "Код підтвердження: 9876" 
                },
                new PushNotification 
                { 
                    Recipient = "dev_token_xyz987", 
                    Message = "У вас нове повідомлення в системі." 
                },
                // Навмисно невалідні об'єкти для демонстрації обробки помилок
                new EmailNotification 
                { 
                    Recipient = "invalid-email-address", 
                    Message = "Тестове повідомлення з помилкою" 
                },
                new SMSNotification 
                { 
                    Recipient = "12345", 
                    Message = "Помилковий SMS" 
                }
            };

            NotificationService service = new NotificationService();
            service.SendAll(notifications);
        }
    }
}