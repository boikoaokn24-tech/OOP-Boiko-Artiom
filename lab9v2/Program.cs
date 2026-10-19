using System;
using System.Collections.Generic;

namespace lab9v2
{
    // Абстрактний базовий клас
    abstract class Payment
    {
        public decimal Amount { get; set; }
        public abstract void Validate();
        public abstract void Process();
    }

    // Похідний клас: Оплата банківською карткою
    class CreditCardPayment : Payment
    {
        public string CardNumber { get; set; } = string.Empty;

        public override void Validate()
        {
            if (string.IsNullOrWhiteSpace(CardNumber) || CardNumber.Length != 16 || !long.TryParse(CardNumber, out _))
            {
                throw new ArgumentException($"Некоректний номер картки -> '{CardNumber}'. Має містити рівно 16 цифр.");
            }
        }

        public override void Process()
        {
            Validate();
            Console.WriteLine($"[CreditCard] Оброблено платіж на суму {Amount} грн з картки ****-****-****-{CardNumber.Substring(12)}");
        }
    }

    // Похідний клас: Оплата через PayPal
    class PayPalPayment : Payment
    {
        public string Email { get; set; } = string.Empty;

        public override void Validate()
        {
            if (string.IsNullOrWhiteSpace(Email) || !Email.Contains("@") || !Email.Contains("."))
            {
                throw new ArgumentException($"Некоректний email PayPal -> '{Email}'");
            }
        }

        public override void Process()
        {
            Validate();
            Console.WriteLine($"[PayPal] Оброблено платіж на суму {Amount} грн з акаунта {Email}");
        }
    }

    // Похідний клас: Криптоплатіж
    class CryptoPayment : Payment
    {
        public string WalletAddress { get; set; } = string.Empty;

        public override void Validate()
        {
            if (string.IsNullOrWhiteSpace(WalletAddress) || WalletAddress.Length < 10)
            {
                throw new ArgumentException($"Некоректна крипто-адреса -> '{WalletAddress}'");
            }
        }

        public override void Process()
        {
            Validate();
            Console.WriteLine($"[Crypto] Оброблено платіж на суму {Amount} грн на гаманець {WalletAddress}");
        }
    }

    // Сервіс для масової обробки платежів
    class PaymentService
    {
        public void ProcessAll(List<Payment> payments)
        {
            Console.WriteLine("=== Початок масової обробки платежів ===");
            int successCount = 0;
            int errorCount = 0;

            foreach (var payment in payments)
            {
                try
                {
                    payment.Process();
                    successCount++;
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"[ПОМИЛКА] Помилка валідації платежу: {ex.Message}");
                    Console.ResetColor();
                    errorCount++;
                }
                Console.WriteLine("----------------------------------------");
            }

            Console.WriteLine($"=== Звіт: Успішно опрацьовано: {successCount}, Помилок: {errorCount} ===");
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            List<Payment> payments = new List<Payment>
            {
                new CreditCardPayment { Amount = 1500.00m, CardNumber = "1234567812345678" },
                new PayPalPayment { Amount = 450.50m, Email = "student@colleg.rv.ua" },
                new CryptoPayment { Amount = 3200.00m, WalletAddress = "0x71C93A2b8f5e1234" },
                
                // Навмисно невалідні об'єкти для демонстрації обробки помилок
                new CreditCardPayment { Amount = 500.00m, CardNumber = "1234" },
                new PayPalPayment { Amount = 100.00m, Email = "invalid-paypal-email" }
            };

            PaymentService service = new PaymentService();
            service.ProcessAll(payments);
        }
    }
}