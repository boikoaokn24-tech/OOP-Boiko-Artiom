using System;
using System.Collections.Generic;

namespace IndependentWork1
{
    // Клас 1: Банківський рахунок
    public class BankAccount
    {
        // Приватні поля
        private string _accountNumber;
        private string _ownerName;
        private decimal _balance;

        // Властивість тільки для читання (read-only)
        public string AccountNumber
        {
            get { return _accountNumber; }
        }

        // Публічна властивість з get і set
        public string OwnerName
        {
            get { return _ownerName; }
            set { _ownerName = value; }
        }

        // Властивість для перегляду балансу
        public decimal Balance
        {
            get { return _balance; }
        }

        // Конструктор
        public BankAccount(string accountNumber, string ownerName, decimal initialBalance)
        {
            _accountNumber = accountNumber;
            _ownerName = ownerName;
            _balance = initialBalance >= 0 ? initialBalance : 0;
        }

        // Метод для поповнення рахунку
        public void Deposit(decimal amount)
        {
            if (amount > 0)
            {
                _balance += amount;
                Console.WriteLine($"[BankAccount] Поповнено на {amount:F2} грн. Поточний баланс: {_balance:F2} грн.");
            }
        }

        // Метод з обчисленням та перевіркою стану (зняття коштів)
        public bool Withdraw(decimal amount)
        {
            if (amount <= 0)
            {
                Console.WriteLine("[BankAccount] Сума зняття має бути більшою за 0.");
                return false;
            }

            if (_balance >= amount)
            {
                _balance -= amount;
                Console.WriteLine($"[BankAccount] Успішно знято {amount:F2} грн. Залишок: {_balance:F2} грн.");
                return true;
            }
            else
            {
                Console.WriteLine($"[BankAccount] Помилка: Недостатньо коштів! Спроба зняти {amount:F2} грн при балансі {_balance:F2} грн.");
                return false;
            }
        }
    }

    // Клас 2: Навчальна група
    public class StudentGroup
    {
        // Приватні поля
        private string _groupName;
        private int _maxCapacity;
        private List<string> _students;

        // Властивість тільки для читання (read-only)
        public string GroupName
        {
            get { return _groupName; }
        }

        // Публічна властивість з get та set
        public int MaxCapacity
        {
            get { return _maxCapacity; }
            set { _maxCapacity = value > 0 ? value : 10; }
        }

        // Read-only властивість для поточної кількості студентів
        public int StudentCount
        {
            get { return _students.Count; }
        }

        // Конструктор
        public StudentGroup(string groupName, int maxCapacity)
        {
            _groupName = groupName;
            _maxCapacity = maxCapacity > 0 ? maxCapacity : 10;
            _students = new List<string>();
        }

        // Метод додавання студента з перевіркою стану
        public bool AddStudent(string studentName)
        {
            if (_students.Count < _maxCapacity)
            {
                _students.Add(studentName);
                Console.WriteLine($"[StudentGroup] Студента '{studentName}' додано до групи {GroupName}.");
                return true;
            }
            else
            {
                Console.WriteLine($"[StudentGroup] Помилка: Група {GroupName} заповнена! Неможливо додати '{studentName}'.");
                return false;
            }
        }

        // Метод виводу списку студентів
        public void PrintGroupInfo()
        {
            Console.WriteLine($"\n--- Інформація про групу {GroupName} ({StudentCount}/{MaxCapacity}) ---");
            if (_students.Count == 0)
            {
                Console.WriteLine("Група порожня.");
            }
            else
            {
                for (int i = 0; i < _students.Count; i++)
                {
                    Console.WriteLine($"{i + 1}. {_students[i]}");
                }
            }
        }
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Console.WriteLine("==================================================");
            Console.WriteLine("  САМОСТІЙНА РОБОТА №1");
            Console.WriteLine("  Тема: Базовий синтаксис C# і оголошення класів");
            Console.WriteLine("==================================================\n");

            // --- 1. Демонстрація роботи класу BankAccount ---
            Console.WriteLine("--- 1. Демонстрація класу BankAccount ---");
            BankAccount account = new BankAccount("UA1234567890", "Іван Петренко", 1000.00m);

            Console.WriteLine($"Номер рахунку (read-only): {account.AccountNumber}");
            Console.WriteLine($"Власник рахунку: {account.OwnerName}");
            Console.WriteLine($"Початковий баланс: {account.Balance:F2} грн\n");

            account.Deposit(500.00m);
            account.Withdraw(300.00m);
            account.Withdraw(1500.00m); // Спроба зняти більше, ніж є на балансі

            // --- 2. Демонстрація роботи класу StudentGroup ---
            Console.WriteLine("\n--- 2. Демонстрація класу StudentGroup ---");
            StudentGroup group = new StudentGroup("П-21", 2); // Максимум 2 студенти для перевірки обмеження

            Console.WriteLine($"Група: {group.GroupName}");
            Console.WriteLine($"Максимальна місткість: {group.MaxCapacity}");

            group.AddStudent("Олексій Коваль");
            group.AddStudent("Марія Бойко");
            group.AddStudent("Василь Сидоренко"); // Перевищення ліміту

            group.PrintGroupInfo();

            Console.WriteLine("\n==================================================");
            Console.WriteLine("Демонстрацію роботи класів завершено успішно.");
            Console.WriteLine("==================================================");
        }
    }
}