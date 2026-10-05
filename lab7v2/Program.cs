using System;

namespace Lab7v2
{
    // Базовий клас
    public class Shape
    {
        public string Name { get; set; }

        public Shape(string name = "Generic Shape")
        {
            Name = name;
        }

        // Віртуальний метод для розрахунку площі
        public virtual double Area()
        {
            Console.WriteLine("--> [Shape.Area] Wyklykayetsya basovyy metod Area()");
            return 0.0;
        }
    }

    // Похідний клас A: перевизначення (override)
    public class Circle : Shape
    {
        public double Radius { get; set; }

        public Circle(double radius) : base("Circle")
        {
            Radius = radius;
        }

        // Перевизначення методу базового класу (динамічний поліморфізм)
        public override double Area()
        {
            Console.WriteLine("--> [Circle.Area] Wyklykayetsya perevyznachenyy metod Circle (override)");
            return Math.PI * Radius * Radius;
        }
    }

    // Похідний клас B: приховування (new)
    public class Triangle : Shape
    {
        public double BaseLength { get; set; }
        public double Height { get; set; }

        public Triangle(double baseLength, double height) : base("Triangle")
        {
            BaseLength = baseLength;
            Height = height;
        }

        // Приховування методу базового класу (статичне зв'язування)
        public new double Area()
        {
            Console.WriteLine("--> [Triangle.Area] Wyklykayetsya prykhovanyy metod Triangle (new)");
            return 0.5 * BaseLength * Height;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Console.WriteLine("==================================================");
            Console.WriteLine("  ЛАБОРАТОРНА РОБОТА №7 (Варіант 2)");
            Console.WriteLine("  Тема: Приховування методів (new) vs Перевизначення (override)");
            Console.WriteLine("==================================================\n");

            // Створення об'єктів
            Circle circle = new Circle(5.0);        // R = 5, Area = ~78.54
            Triangle triangle = new Triangle(4.0, 3.0); // Base = 4, Height = 3, Area = 6.0

            // Upcasting: збереження об'єктів у змінних типу базового класу Shape
            Shape shapeCircle = circle;
            Shape shapeTriangle = triangle;

            // 1. ДЕМОНСТРАЦІЯ ВИКЛИКУ ЧЕРЕЗ ПОСИЛАННЯ БАЗОВОГО КЛАСУ (Shape)
            Console.WriteLine("--- 1. Виклик через посилання базового класу (Shape) ---");
            
            Console.Write("shapeCircle.Area(): ");
            double area1 = shapeCircle.Area();
            Console.WriteLine($"   Результат: {area1:F2} (Працює поліморфізм -> викликано Circle.Area)\n");

            Console.Write("shapeTriangle.Area(): ");
            double area2 = shapeTriangle.Area();
            Console.WriteLine($"   Результат: {area2:F2} (Поліморфізм НЕ працює -> викликано Shape.Area)\n");

            // 2. ДЕМОНСТРАЦІЯ ВИКЛИКУ ЧЕРЕЗ ПОСИЛАННЯ ПОХІДНИХ КЛАСІВ (або явне приведення)
            Console.WriteLine("--- 2. Виклик через посилання похідного типу або явне приведення ---");

            Console.Write("((Circle)shapeCircle).Area(): ");
            double area3 = ((Circle)shapeCircle).Area();
            Console.WriteLine($"   Результат: {area3:F2}\n");

            Console.Write("((Triangle)shapeTriangle).Area(): ");
            double area4 = ((Triangle)shapeTriangle).Area();
            Console.WriteLine($"   Результат: {area4:F2} (Явне приведення відкриває прихований метод)\n");

            // 3. ПІДСУМОК ТА ПОЯСНЕННЯ
            Console.WriteLine("==================================================");
            Console.WriteLine("ПОЯСНЕННЯ:");
            Console.WriteLine("1. override (Circle): Метод обирається за РЕАЛЬНИМ типом об'єкта.");
            Console.WriteLine("   Тому Shape reference -> Circle instance все одно викликає Circle.Area().");
            Console.WriteLine("2. new (Triangle): Метод обирається за ТИПОМ ПОСИЛАННЯ.");
            Console.WriteLine("   Тому Shape reference -> Triangle instance викликає Shape.Area().");
            Console.WriteLine("==================================================");
        }
    }
}