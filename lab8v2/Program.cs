using System;
using System.Collections.Generic;
using System.Linq;

namespace Lab8v2
{
    // Базовий клас
    public class Vehicle
    {
        public double Speed { get; set; } // швидкість у км/год

        public Vehicle(double speed)
        {
            Speed = speed;
        }

        // Віртуальний метод
        public virtual void Move()
        {
            Console.WriteLine($"[Vehicle] Рухається зі швидкістю {Speed} км/год.");
        }
    }

    // Похідний клас 1: Автомобіль
    public class Car : Vehicle
    {
        public int NumWheels { get; set; }

        public Car(double speed, int numWheels) : base(speed)
        {
            NumWheels = numWheels;
        }

        public override void Move()
        {
            Console.WriteLine($"[Car] Їде по дорозі на {NumWheels} колесах зі швидкістю {Speed} км/год.");
        }
    }

    // Похідний клас 2: Велосипед
    public class Bicycle : Vehicle
    {
        public bool HasGears { get; set; }

        public Bicycle(double speed, bool hasGears) : base(speed)
        {
            HasGears = hasGears;
        }

        public override void Move()
        {
            string gearsInfo = HasGears ? "із передачами" : "без передач";
            Console.WriteLine($"[Bicycle] Крутить педалі {gearsInfo} зі швидкістю {Speed} км/год.");
        }
    }

    // Похідний клас 3: Човен
    public class Boat : Vehicle
    {
        public string EngineType { get; set; }

        public Boat(double speed, string engineType) : base(speed)
        {
            EngineType = engineType;
        }

        public override void Move()
        {
            Console.WriteLine($"[Boat] Пливе по воді (двигун: {EngineType}) зі швидкістю {Speed} км/год.");
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Console.WriteLine("==================================================");
            Console.WriteLine("  ЛАБОРАТОРНА РОБОТА №8 (Варіант 2)");
            Console.WriteLine("  Тема: Поліморфізм та динамічне зв'язування");
            Console.WriteLine("==================================================\n");

            // Створення колекції об'єктів базового типу List<Vehicle>
            List<Vehicle> fleet = new List<Vehicle>
            {
                new Car(120.0, 4),
                new Bicycle(22.5, true),
                new Boat(45.0, "Бензиновий"),
                new Car(90.0, 4),
                new Bicycle(15.0, false)
            };

            Console.WriteLine("--- Демонстрація поліморфного виклику Move() ---");
            foreach (var vehicle in fleet)
            {
                // Поліморфний виклик: для кожного об'єкта виконується його власний перевизначений Move()
                vehicle.Move();
            }

            // Агрегація: обчислення середньої швидкості всіх транспортних засобів
            double averageSpeed = fleet.Average(v => v.Speed);
            double maxSpeed = fleet.Max(v => v.Speed);

            Console.WriteLine("\n==================================================");
            Console.WriteLine("РЕЗУЛЬТАТИ АГРЕГАЦІЇ:");
            Console.WriteLine($"Кількість транспортних засобів у флоті: {fleet.Count}");
            Console.WriteLine($"Середня швидкість усіх ТЗ: {averageSpeed:F2} км/год");
            Console.WriteLine($"Максимальна швидкість серед усіх ТЗ: {maxSpeed:F2} км/год");
            Console.WriteLine("==================================================");
        }
    }
}