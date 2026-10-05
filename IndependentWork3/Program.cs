using System;

namespace IndependentWork3
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Console.WriteLine("==================================================");
            Console.WriteLine("  САМОСТІЙНА РОБОТА №3");
            Console.WriteLine("  Тема: Аналіз інкапсуляції в open-source проєктах");
            Console.WriteLine("==================================================\n");

            Console.WriteLine("Обраний проєкт: AutoMapper");
            Console.WriteLine("Репозиторій: https://github.com/AutoMapper/AutoMapper\n");

            Console.WriteLine("Проаналізовані класи:");
            Console.WriteLine("1. TypePair (строга інкапсуляція, read-only автовластивості)");
            Console.WriteLine("2. ResolutionContext (інкапсуляція стану, приватні поля)");
            Console.WriteLine("3. Profile (валідація входів, захист внутрішніх колекцій)\n");

            Console.WriteLine("==================================================");
            Console.WriteLine("Детальний звіт та аналіз коду дивіться у файлі README.md");
            Console.WriteLine("==================================================");
        }
    }
}