using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Lab15v2
{
    public class Course
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public int Credits { get; set; }
    }

    public class Student
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Age { get; set; }
        public int CourseId { get; set; }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.WriteLine("=== Лабораторна робота №15: LINQ в дії (Варіант 2: Управління студентами) ===\n");

            // Ініціалізація тестових даних (15-20 елементів)
            var courses = new List<Course>
            {
                new Course { Id = 1, Title = "Programming Basics", Credits = 5 },
                new Course { Id = 2, Title = "Data Structures", Credits = 6 },
                new Course { Id = 3, Title = "Web Development", Credits = 4 },
                new Course { Id = 4, Title = "Database Management", Credits = 5 }
            };

            var students = new List<Student>
            {
                new Student { Id = 1, Name = "Олександр Петренко", Age = 18, CourseId = 1 },
                new Student { Id = 2, Name = "Марія Шевченко", Age = 19, CourseId = 1 },
                new Student { Id = 3, Name = "Іван Ковальчук", Age = 21, CourseId = 2 },
                new Student { Id = 4, Name = "Софія Мельник", Age = 17, CourseId = 1 },
                new Student { Id = 5, Name = "Андрій Бондар", Age = 20, CourseId = 3 },
                new Student { Id = 6, Name = "Катерина Ткачук", Age = 19, CourseId = 2 },
                new Student { Id = 7, Name = "Максим Бойко", Age = 18, CourseId = 4 },
                new Student { Id = 8, Name = "Анна Павлюк", Age = 22, CourseId = 3 },
                new Student { Id = 9, Name = "Дмитро Романюк", Age = 19, CourseId = 1 },
                new Student { Id = 10, Name = "Вікторія Савчук", Age = 17, CourseId = 4 },
                new Student { Id = 11, Name = "Юрій Олійник", Age = 23, CourseId = 2 },
                new Student { Id = 12, Name = "Тетяна Шевчук", Age = 18, CourseId = 3 },
                new Student { Id = 13, Name = "Олег Кравчук", Age = 20, CourseId = 1 },
                new Student { Id = 14, Name = "Наталія Коваль", Age = 19, CourseId = 4 },
                new Student { Id = 15, Name = "Артем Поліщук", Age = 18, CourseId = 1 }
            };

            var stopwatch = new Stopwatch();

            // ==========================================
            // ЗАПИТ 1: Знайти всіх студентів віком до 20 років, які навчаються на курсі "Programming Basics".
            // ==========================================
            Console.WriteLine("--- Запит 1: Фільтрація (Вік < 20 та курс \"Programming Basics\") ---");

            stopwatch.Restart();
            var query1Methods = students
                .Where(s => s.Age < 20 && courses.Any(c => c.Id == s.CourseId && c.Title == "Programming Basics"))
                .ToList();
            stopwatch.Stop();
            long timeMethod1 = stopwatch.ElapsedTicks;

            stopwatch.Restart();
            var query1Syntax = (from s in students
                                join c in courses on s.CourseId equals c.Id
                                where s.Age < 20 && c.Title == "Programming Basics"
                                select s).ToList();
            stopwatch.Stop();
            long timeQuery1 = stopwatch.ElapsedTicks;

            Console.WriteLine($"[Method Syntax] Знайдено студентів: {query1Methods.Count}. Час: {timeMethod1} тіків");
            Console.WriteLine($"[Query Syntax]  Знайдено студентів: {query1Syntax.Count}. Час: {timeQuery1} тіків\n");


            // ==========================================
            // ЗАПИТ 2: Об’єднати студентів з їхніми курсами, вивести ім’я студента та назву курсу.
            // ==========================================
            Console.WriteLine("--- Запит 2: Об’єднання (Join) студентів та курсів ---");

            stopwatch.Restart();
            var query2Methods = students
                .Join(courses,
                      s => s.CourseId,
                      c => c.Id,
                      (s, c) => new { StudentName = s.Name, CourseTitle = c.Title })
                .ToList();
            stopwatch.Stop();
            long timeMethod2 = stopwatch.ElapsedTicks;

            stopwatch.Restart();
            var query2Syntax = (from s in students
                                join c in courses on s.CourseId equals c.Id
                                select new { StudentName = s.Name, CourseTitle = c.Title }).ToList();
            stopwatch.Stop();
            long timeQuery2 = stopwatch.ElapsedTicks;

            Console.WriteLine($"[Method Syntax] Елементів у результаті: {query2Methods.Count}. Час: {timeMethod2} тіків");
            Console.WriteLine($"[Query Syntax]  Елементів у результаті: {query2Syntax.Count}. Час: {timeQuery2} тіків");
            Console.WriteLine("Приклад результату: " + query2Methods.First().StudentName + " -> " + query2Methods.First().CourseTitle + "\n");


            // ==========================================
            // ЗАПИТ 3: Підрахувати кількість студентів на кожному курсі.
            // ==========================================
            Console.WriteLine("--- Запит 3: Агрегація та групування (Кількість студентів на курсі) ---");

            stopwatch.Restart();
            var query3Methods = students
                .GroupBy(s => s.CourseId)
                .Join(courses,
                      g => g.Key,
                      c => c.Id,
                      (g, c) => new { CourseTitle = c.Title, Count = g.Count() })
                .ToList();
            stopwatch.Stop();
            long timeMethod3 = stopwatch.ElapsedTicks;

            stopwatch.Restart();
            var query3Syntax = (from s in students
                                join c in courses on s.CourseId equals c.Id
                                group s by c.Title into courseGroup
                                select new { CourseTitle = courseGroup.Key, Count = courseGroup.Count() }).ToList();
            stopwatch.Stop();
            long timeQuery3 = stopwatch.ElapsedTicks;

            Console.WriteLine("[Method Syntax] Результати:");
            foreach (var item in query3Methods)
                Console.WriteLine($"  - {item.CourseTitle}: {item.Count} студентів");
            Console.WriteLine($"Час (Method): {timeMethod3} тіків | Час (Query): {timeQuery3} тіків\n");


            // ==========================================
            // ЗАПИТ 4: Знайти наймолодшого студента.
            // ==========================================
            Console.WriteLine("--- Запит 4: Агрегація (Наймолодший студент) ---");

            stopwatch.Restart();
            var youngestMethod = students.OrderBy(s => s.Age).FirstOrDefault();
            stopwatch.Stop();
            long timeMethod4 = stopwatch.ElapsedTicks;

            stopwatch.Restart();
            var youngestQuery = (from s in students
                                 orderby s.Age
                                 select s).FirstOrDefault();
            stopwatch.Stop();
            long timeQuery4 = stopwatch.ElapsedTicks;

            Console.WriteLine($"[Method Syntax] Наймолодший: {youngestMethod?.Name} ({youngestMethod?.Age} років). Час: {timeMethod4} тіків");
            Console.WriteLine($"[Query Syntax]  Наймолодший: {youngestQuery?.Name} ({youngestQuery?.Age} років). Час: {timeQuery4} тіків\n");


            // ==========================================
            // ВИСНОВКИ ПРО ПРОДУКТИВНІСТЬ
            // ==========================================
            Console.WriteLine("==========================================");
            Console.WriteLine("ВИСНОВКИ ЩОДО ПРОДУКТИВНОСТІ LINQ:");
            Console.WriteLine("1. Прості операції фільтрації (Where) та сортування (OrderBy) виконуються найшвидше.");
            Console.WriteLine("2. Операції Join та GroupBy вимагають додаткових витрат на побудову хеш-таблиць у пам'яті.");
            Console.WriteLine("3. Method Syntax та Query Syntax генерують однаковий IL-код, тому їхня продуктивність еквівалентна.");
        }
    }
}