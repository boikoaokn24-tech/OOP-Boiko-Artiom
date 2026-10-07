using System;

namespace Lab2V2
{
    public class Car
    {
        private string _brand = "Невідомо";
        private string _model = "Невідомо";
        private int _year;

        public string Brand
        {
            get => _brand;
            set => _brand = string.IsNullOrWhiteSpace(value) ? "Невідомо" : value;
        }

        public string Model
        {
            get => _model;
            set => _model = string.IsNullOrWhiteSpace(value) ? "Невідомо" : value;
        }

        public int Year
        {
            get => _year;
            set
            {
                int currentYear = DateTime.Now.Year;
                if (value > currentYear)
                {
                    Console.WriteLine($"[Валідація] Рік {value} не може бути в майбутньому! Встановлено поточний рік ({currentYear}).");
                    _year = currentYear;
                }
                else
                {
                    _year = value;
                }
            }
        }

        public Car() : this("Невідомо", "Невідомо", 2000)
        {
        }

        public Car(string brand, string model, int year)
        {
            Brand = brand;
            Model = model;
            Year = year;
        }

        public void StartEngine()
        {
            Console.WriteLine($"Двигун авто {Brand} {Model} ({Year} року) успішно запущено!");
        }

        ~Car()
        {
            Console.WriteLine($"[Деструктор] Об'єкт авто \"{Brand} {Model}\" вилучено з пам'яті.");
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            Console.InputEncoding = System.Text.Encoding.UTF8;
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Console.WriteLine("=== Creating objects ===");

            Car? car1 = new Car();
            Car? car2 = new Car("BMW", "M5 CS", 2021);
            Car? car3 = new Car("Tesla", "Cybertruck", 2030);

            Console.WriteLine("\n=== Testing Methods ===");
            car1.StartEngine();
            car2.StartEngine();
            car3.StartEngine();

            Console.WriteLine("\n=== Objects created ===");

            Console.WriteLine("\nEnd of Main, preparing for GC...");

            car1 = null;
            car2 = null;
            car3 = null;

            GC.Collect();
            GC.WaitForPendingFinalizers();

            Console.WriteLine("GC process completed.");
        }
    }
}