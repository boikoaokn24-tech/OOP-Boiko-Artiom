using System;

// 1. Клас Fraction (Дріб) відповідно до Варіанту 2
public class Fraction : IEquatable<Fraction>
{
    private int _numerator;
    private int _denominator;

    // Публічні властивості
    public int Numerator
    {
        get => _numerator;
        set => _numerator = value;
    }

    public int Denominator
    {
        get => _denominator;
        set
        {
            // Валідація: знаменник не може бути 0
            if (value == 0)
            {
                throw new ArgumentException("Знаменник дробу не може дорівнювати нулю.", nameof(value));
            }

            // Забезпечуємо, щоб знак містився в чисельнику, а знаменник був додатним
            if (value < 0)
            {
                _numerator = -_numerator;
                _denominator = -value;
            }
            else
            {
                _denominator = value;
            }
            Simplify();
        }
    }

    // Статичний член (властивість, що повертає одиничний дріб 1/1)
    public static Fraction One => new Fraction(1, 1);

    // Конструктор
    public Fraction(int numerator, int denominator)
    {
        if (denominator == 0)
        {
            throw new ArgumentException("Знаменник дробу не може дорівнювати нулю.", nameof(denominator));
        }

        if (denominator < 0)
        {
            _numerator = -numerator;
            _denominator = -denominator;
        }
        else
        {
            _numerator = numerator;
            _denominator = denominator;
        }
        Simplify();
    }

    // Індексатор: 0 - чисельник, 1 - знаменник
    public int this[int index]
    {
        get
        {
            return index switch
            {
                0 => _numerator,
                1 => _denominator,
                _ => throw new IndexOutOfRangeException("Індекс може бути лише 0 (чисельник) або 1 (знаменник).")
            };
        }
        set
        {
            switch (index)
            {
                case 0:
                    _numerator = value;
                    break;
                case 1:
                    Denominator = value; // Використовуємо сеттер для валідації знаменника
                    break;
                default:
                    throw new IndexOutOfRangeException("Індекс може бути лише 0 або 1.");
            }
            Simplify();
        }
    }

    // Допоміжний метод для скорочення дробу
    private void Simplify()
    {
        int gcd = GCD(Math.Abs(_numerator), _denominator);
        _numerator /= gcd;
        _denominator /= gcd;
    }

    private static int GCD(int a, int b)
    {
        while (b != 0)
        {
            int temp = b;
            b = a % b;
            a = temp;
        }
        return Math.Max(1, a);
    }

    // Перевантажені оператори
    public static Fraction operator +(Fraction a, Fraction b)
    {
        int num = a._numerator * b._denominator + b._numerator * a._denominator;
        int den = a._denominator * b._denominator;
        return new Fraction(num, den);
    }

    public static Fraction operator *(Fraction a, Fraction b)
    {
        return new Fraction(a._numerator * b._numerator, a._denominator * b._denominator);
    }

    public static bool operator ==(Fraction a, Fraction b)
    {
        if (ReferenceEquals(a, b)) return true;
        if (a is null || b is null) return false;
        return a._numerator == b._numerator && a._denominator == b._denominator;
    }

    public static bool operator !=(Fraction a, Fraction b)
    {
        return !(a == b);
    }

    // Перевизначення методів порівняння та хешування
    public override bool Equals(object obj)
    {
        return obj is Fraction fraction && Equals(fraction);
    }

    public bool Equals(Fraction other)
    {
        return other is not null &&
               _numerator == other._numerator &&
               _denominator == other._denominator;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(_numerator, _denominator);
    }

    public override string ToString()
    {
        return _denominator == 1 ? $"{_numerator}" : $"{_numerator}/{_denominator}";
    }
}

class Program
{
    static void Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        Console.WriteLine("=== Демонстрація роботи класу Fraction (Варіант 2) ===\n");

        // 1. Створення об'єктів та використання статичного члена
        Fraction f1 = new Fraction(2, 4); // Автоматично скоротиться до 1/2
        Fraction f2 = new Fraction(3, 4);
        Fraction unitFraction = Fraction.One; // Статичний член

        Console.WriteLine($"Дріб 1: {f1}");
        Console.WriteLine($"Дріб 2: {f2}");
        Console.WriteLine($"Статичний дріб (One): {unitFraction}\n");

        // 2. Демонстрація валідації властивостей
        Console.WriteLine("--- Перевірка валідації властивостей ---");
        try
        {
            Console.WriteLine("Спроба встановити знаменник = 0...");
            f1.Denominator = 0;
        }
        catch (ArgumentException ex)
        {
            Console.WriteLine($"[Помилку перехоплено]: {ex.Message}\n");
        }

        // 3. Використання індексатора
        Console.WriteLine("--- Використання індексатора ---");
        Console.WriteLine($"f2 до змін через індексатор: {f2}");
        Console.WriteLine($"Чисельник (індекс 0): {f2[0]}");
        Console.WriteLine($"Знаменник (індекс 1): {f2[1]}");
        
        f2[0] = 5; // Змінюємо чисельник через індексатор
        Console.WriteLine($"f2 після зміни чисельника на 5: {f2}\n");

        // 4. Перевантажені оператори (+, *, ==, !=)
        Console.WriteLine("--- Перевантаження операторів ---");
        Fraction sum = f1 + f2;
        Fraction product = f1 * f2;

        Console.WriteLine($"{f1} + {f2} = {sum}");
        Console.WriteLine($"{f1} * {f2} = {product}");

        Fraction f3 = new Fraction(1, 2);
        Console.WriteLine($"Чи f1 ({f1}) дорівнює f3 ({f3})? {f1 == f3}");
        Console.WriteLine($"Чи f1 ({f1}) не дорівнює f2 ({f2})? {f1 != f2}\n");

        // 5. Перевизначені методи Equals та ToString
        Console.WriteLine("--- Методи Equals та ToString ---");
        Console.WriteLine($"f1.Equals(f3): {f1.Equals(f3)}");
        Console.WriteLine($"Хеш-код f1: {f1.GetHashCode()}");
        Console.WriteLine($"Хеш-код f3: {f3.GetHashCode()}");
    }
}