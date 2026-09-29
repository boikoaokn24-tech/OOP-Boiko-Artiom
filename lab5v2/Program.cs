using System;
using System.Collections.Generic;
using System.Linq;

// 1. Клас StringList (Список рядків) відповідно до Варіанту 2
public class StringList : IEquatable<StringList>
{
    private List<string> _items;

    // Конструктор за замовчуванням
    public StringList()
    {
        _items = new List<string>();
    }

    // Конструктор на основі існуючої колекції
    public StringList(IEnumerable<string> items)
    {
        _items = new List<string>(items);
    }

    // Властивість для отримання кількості елементів
    public int Count => _items.Count;

    // Індексатор для доступу до елементів за індексом
    public string this[int index]
    {
        get
        {
            if (index < 0 || index >= _items.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(index), "Індекс виходить за межі списку.");
            }
            return _items[index];
        }
        set
        {
            if (index < 0 || index >= _items.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(index), "Індекс виходить за межі списку.");
            }
            _items[index] = value;
        }
    }

    // Додаткові методи
    public void Add(string item)
    {
        _items.Add(item);
    }

    public void Remove(string item)
    {
        _items.Remove(item);
    }

    // Перевантажений оператор + (об’єднання двох списків рядків)
    public static StringList operator +(StringList left, StringList right)
    {
        if (left is null) throw new ArgumentNullException(nameof(left));
        if (right is null) throw new ArgumentNullException(nameof(right));

        var combinedItems = new List<string>(left._items);
        combinedItems.AddRange(right._items);
        return new StringList(combinedItems);
    }

    // Перевантажені оператори == та !=
    public static bool operator ==(StringList left, StringList right)
    {
        if (ReferenceEquals(left, right)) return true;
        if (left is null || right is null) return false;
        return left._items.SequenceEqual(right._items);
    }

    public static bool operator !=(StringList left, StringList right)
    {
        return !(left == right);
    }

    // Перевизначення Equals та GetHashCode
    public override bool Equals(object obj)
    {
        return obj is StringList list && Equals(list);
    }

    public bool Equals(StringList other)
    {
        return other is not null && _items.SequenceEqual(other._items);
    }

    public override int GetHashCode()
    {
        HashCode hash = new HashCode();
        foreach (var item in _items)
        {
            hash.Add(item);
        }
        return hash.ToHashCode();
    }

    // Перевизначення ToString для зручного виведення
    public override string ToString()
    {
        return $"[{string.Join(", ", _items)}]";
    }
}

class Program
{
    static void Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        Console.WriteLine("=== Демонстрація роботи класу StringList (Варіант 2) ===\n");

        // 1. Створення об'єктів та додавання елементів
        StringList list1 = new StringList();
        list1.Add("Apple");
        list1.Add("Banana");

        StringList list2 = new StringList();
        list2.Add("Cherry");
        list2.Add("Date");

        Console.WriteLine($"Список 1: {list1} (Кількість: {list1.Count})");
        Console.WriteLine($"Список 2: {list2} (Кількість: {list2.Count})\n");

        // 2. Демонстрація роботи індексатора (читання та запис)
        Console.WriteLine("--- Робота з індексатором ---");
        Console.WriteLine($"Елемент з індексом 0 у списку 1: {list1[0]}");
        
        // Зміна елемента за допомогою індексатора (запис)
        list1[0] = "Apricot";
        Console.WriteLine($"Список 1 після зміни елемента [0] на 'Apricot': {list1}\n");

        // 3. Перевантажений оператор + (об'єднання списків)
        Console.WriteLine("--- Перевантаження оператора + (Об'єднання) ---");
        StringList combinedList = list1 + list2;
        Console.WriteLine($"Об'єднаний список (list1 + list2): {combinedList}\n");

        // 4. Перевантажені оператори == та !=, а також Equals
        Console.WriteLine("--- Перевантаження операторів порівняння та Equals ---");
        StringList list3 = new StringList();
        list3.Add("Apricot");
        list3.Add("Banana");

        Console.WriteLine($"Список 1: {list1}");
        Console.WriteLine($"Список 3: {list3}");
        Console.WriteLine($"Чи list1 == list3? {list1 == list3}");
        Console.WriteLine($"Чи list1 != list2? {list1 != list2}");
        Console.WriteLine($"Використання методу list1.Equals(list3): {list1.Equals(list3)}\n");

        // 5. Демонстрація додаткового методу Remove
        Console.WriteLine("--- Використання методу Remove ---");
        combinedList.Remove("Banana");
        Console.WriteLine($"Об'єднаний список після видалення 'Banana': {combinedList}");
    }
}