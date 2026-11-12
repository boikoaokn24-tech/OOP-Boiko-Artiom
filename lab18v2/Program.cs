using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace lab18v2
{
    // 1. Власний клас винятку (Варіант 2)
    public class UserNotFoundException : Exception
    {
        public string Username { get; }

        public UserNotFoundException()
            : base("Користувача не знайдено.")
        {
            Username = string.Empty;
        }

        public UserNotFoundException(string username)
            : base($"Користувача з іменем '{username}' не знайдено в системі.")
        {
            Username = username;
        }

        public UserNotFoundException(string username, string message)
            : base(message)
        {
            Username = username;
        }

        public UserNotFoundException(string username, string message, Exception innerException)
            : base(message, innerException)
        {
            Username = username;
        }
    }

    // 2. Узагальнений клас патерну Result<T>
    public class Result<T>
    {
        public bool IsSuccess { get; }
        public T? Value { get; }
        public string Error { get; }

        private Result(T? value, bool isSuccess, string error)
        {
            Value = value;
            IsSuccess = isSuccess;
            Error = error;
        }

        public static Result<T> Success(T value)
        {
            return new Result<T>(value, true, string.Empty);
        }

        public static Result<T> Failure(string error)
        {
            return new Result<T>(default, false, error);
        }

        public static Result<T> Failure(Exception ex)
        {
            return new Result<T>(default, false, ex.Message);
        }
    }

    // Модель користувача
    public class User
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public DateTime RegisteredAt { get; set; }

        public User(string username, string password)
        {
            Username = username;
            Password = password;
            RegisteredAt = DateTime.Now;
        }

        public override string ToString()
        {
            return $"User: {Username} (зареєстрований: {RegisteredAt:dd.MM.yyyy HH:mm})";
        }
    }

    // 3. Сервіс управління користувачами (Варіант 2)
    public class UserService
    {
        private readonly List<User> _users = new List<User>
        {
            new User("admin", "Admin1234"),
            new User("artem_boiko", "Pass2026!")
        };

        // Метод, що викидає власний виняток, якщо користувача не знайдено
        public User GetUser(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                throw new UserNotFoundException(username ?? "null", "Ім'я користувача не може бути порожнім.");

            var user = _users.FirstOrDefault(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));

            if (user == null)
                throw new UserNotFoundException(username);

            return user;
        }

        // Метод, що повертає Result<bool> для очікуваних сценаріїв невдачі валідації
        public Result<bool> RegisterUser(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username) || username.Length < 3)
                return Result<bool>.Failure("Ім'я користувача повинно містити щонайменше 3 символи.");

            if (string.IsNullOrWhiteSpace(password) || password.Length < 6)
                return Result<bool>.Failure("Пароль занадто короткий (мінімум 6 символів).");

            if (_users.Any(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase)))
                return Result<bool>.Failure($"Користувач '{username}' вже існує в системі.");

            _users.Add(new User(username, password));
            return Result<bool>.Success(true);
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            var userService = new UserService();

            Console.WriteLine("=== 1. Демонстрація патерну Result<T> (RegisterUser) ===");

            // Спроба 1: Занадто короткий пароль
            var res1 = userService.RegisterUser("new_user", "123");
            PrintRegistrationResult("new_user", res1);

            // Спроба 2: Користувач вже існує
            var res2 = userService.RegisterUser("admin", "StrongPass99");
            PrintRegistrationResult("admin", res2);

            // Спроба 3: Успішна реєстрація
            var res3 = userService.RegisterUser("ivan_koval", "SecurePass2026");
            PrintRegistrationResult("ivan_koval", res3);

            Console.WriteLine("\n=== 2. Демонстрація власного винятку UserNotFoundException (GetUser) ===");

            // Спроба 1: Пошук існуючого користувача (щойно зареєстрованого)
            try
            {
                User foundUser = userService.GetUser("ivan_koval");
                Console.WriteLine($"[Успіх] Знайдено: {foundUser}");
            }
            catch (UserNotFoundException ex)
            {
                Console.WriteLine($"[Помилка] {ex.Message} (Запитуваний логін: '{ex.Username}')");
            }

            // Спроба 2: Пошук неіснуючого користувача (викине UserNotFoundException)
            try
            {
                Console.WriteLine("Пошук користувача 'ghost_user'...");
                User missingUser = userService.GetUser("ghost_user");
                Console.WriteLine($"[Успіх] Знайдено: {missingUser}");
            }
            catch (UserNotFoundException ex)
            {
                Console.WriteLine($"[Перехоплено UserNotFoundException] {ex.Message}");
                Console.WriteLine($"Деталі винятку -> Username: '{ex.Username}'");
            }
        }

        static void PrintRegistrationResult(string username, Result<bool> result)
        {
            if (result.IsSuccess)
            {
                Console.WriteLine($"[Result: SUCCESS] Користувача '{username}' успішно зареєстровано (Value = {result.Value}).");
            }
            else
            {
                Console.WriteLine($"[Result: FAILURE] Помилка реєстрації '{username}': {result.Error}");
            }
        }
    }
}