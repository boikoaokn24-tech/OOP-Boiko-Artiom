using System;

// 1. Базовий клас Shape
public class Shape
{
    public string Color { get; set; }

    public Shape(string color)
    {
        Color = color;
    }

    // Віртуальний метод для перевизначення (поліморфізм)
    public virtual double GetArea()
    {
        return 0.0;
    }

    // Метод для демонстрації приховування через new (базова версія)
    public string GetShapeType()
    {
        return "Базова фігура (Shape)";
    }

    public virtual void DisplayInfo()
    {
        Console.WriteLine($"Фігура кольору: {Color}, Площа: {GetArea():F2}");
    }
}

// 2. Похідний клас Circle (Коло)
public class Circle : Shape
{
    public double Radius { get; set; }

    // Виклик конструктора базового класу через base(...)
    public Circle(string color, double radius) : base(color)
    {
        Radius = radius;
    }

    // Перевизначення віртуального методу (override)
    public override double GetArea()
    {
        return Math.PI * Radius * Radius;
    }

    public void Draw()
    {
        Console.WriteLine($"Малюємо коло радіусом {Radius} кольору {Color}.");
    }

    public override void DisplayInfo()
    {
        Console.WriteLine($"[Circle] Колір: {Color}, Радіус: {Radius}, Площа: {GetArea():F2}");
    }
}

// 3. Похідний клас Rectangle (Прямокутник)
public class Rectangle : Shape
{
    public double Width { get; set; }
    public double Height { get; set; }

    // Виклик конструктора базового класу через base(...)
    public Rectangle(string color, double width, double height) : base(color)
    {
        Width = width;
        Height = height;
    }

    // Перевизначення віртуального методу (override)
    public override double GetArea()
    {
        return Width * Height;
    }

    public void Draw()
    {
        Console.WriteLine($"Малюємо прямокутник зі сторонами {Width}x{Height} кольору {Color}.");
    }

    // Демонстрація new: приховування невіртуального методу базового класу
    public new string GetShapeType()
    {
        return "Прямокутник (Rectangle) - приховано через new";
    }

    public override void DisplayInfo()
    {
        Console.WriteLine($"[Rectangle] Колір: {Color}, Ширина: {Width}, Висота: {Height}, Площа: {GetArea():F2}");
    }
}

class Program
{
    static void Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        Console.WriteLine("=== Демонстрація ієрархії Shape -> Circle -> Rectangle (Варіант 2) ===\n");

        // 1. Створення об'єктів безпосередньо їхніх класів
        Circle circle = new Circle("Червоний", 5.0);
        Rectangle rectangle = new Rectangle("Синій", 4.0, 6.0);

        Console.WriteLine("--- Прямі виклики методів об'єктів ---");
        circle.DisplayInfo();
        circle.Draw();
        
        rectangle.DisplayInfo();
        rectangle.Draw();
        Console.WriteLine();

        // 2. Демонстрація поліморфної поведінки (virtual / override)
        Console.WriteLine("--- Демонстрація поліморфізму (посилання типу Shape) ---");
        Shape shape1 = new Circle("Зелений", 3.0);
        Shape shape2 = new Rectangle("Жовтий", 5.0, 2.0);

        // Викликається перевизначений метод завдяки override (динамічне зв'язування)
        shape1.DisplayInfo();
        shape2.DisplayInfo();
        Console.WriteLine();

        // 3. Демонстрація різниці між override та new
        Console.WriteLine("--- Порівняння override та new ---");
        
        // Для методу GetArea() використано override -> поліморфізм працює через посилання Shape
        Console.WriteLine($"Площа через посилання Shape (Circle): {shape1.GetArea():F2}"); // Викличе метод Circle
        Console.WriteLine($"Площа через посилання Shape (Rectangle): {shape2.GetArea():F2}"); // Викличе метод Rectangle
        Console.WriteLine();

        // Для методу GetShapeType() у Rectangle використано модифікатор new (приховування)
        // Якщо посилання має тип Rectangle:
        Rectangle rectRef = new Rectangle("Фіолетовий", 2.0, 2.0);
        Shape shapeRefAsShape = rectRef; // те ж саме об'єкт через базове посилання Shape

        Console.WriteLine($"Виклик GetShapeType() через посилання типу Rectangle: {rectRef.GetShapeType()}");
        Console.WriteLine($"Виклик GetShapeType() через посилання типу Shape: {shapeRefAsShape.GetShapeType()}");
        
        Console.WriteLine("\nПояснення: Метод з модифікатором 'new' викликається залежно від типу посилання (статичне зв'язування), " +
                          "тоді як метод з 'override' викликається залежно від реального типу об'єкта (динамічне зв'язування).");
    }
}