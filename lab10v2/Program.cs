using System;
using System.Collections.Generic;

namespace lab10v2
{
    // 1. Інтерфейс ISortable
    public interface ISortable
    {
        void Sort(int[] array);
    }

    // Реалізація інтерфейсу: Сортування бульбашкою
    public class BubbleSort : ISortable
    {
        public void Sort(int[] array)
        {
            Console.WriteLine("[BubbleSort] Виконується сортування бульбашкою...");
            int n = array.Length;
            for (int i = 0; i < n - 1; i++)
            {
                for (int j = 0; j < n - i - 1; j++)
                {
                    if (array[j] > array[j + 1])
                    {
                        int temp = array[j];
                        array[j] = array[j + 1];
                        array[j + 1] = temp;
                    }
                }
            }
        }
    }

    // Реалізація інтерфейсу: Швидке сортування (QuickSort)
    public class QuickSort : ISortable
    {
        public void Sort(int[] array)
        {
            Console.WriteLine("[QuickSort] Виконується швидке сортування...");
            QuickSortRecursive(array, 0, array.Length - 1);
        }

        private void QuickSortRecursive(int[] array, int low, int high)
        {
            if (low < high)
            {
                int pivotIndex = Partition(array, low, high);
                QuickSortRecursive(array, low, pivotIndex - 1);
                QuickSortRecursive(array, pivotIndex + 1, high);
            }
        }

        private int Partition(int[] array, int low, int high)
        {
            int pivot = array[high];
            int i = (low - 1);
            for (int j = low; j < high; j++)
            {
                if (array[j] <= pivot)
                {
                    i++;
                    int temp = array[i];
                    array[i] = array[j];
                    array[j] = temp;
                }
            }
            int temp1 = array[i + 1];
            array[i + 1] = array[high];
            array[high] = temp1;
            return i + 1;
        }
    }

    // 2. Абстрактний клас LoggerBase
    public abstract class LoggerBase
    {
        // Абстрактний метод, який реалізують похідні класи
        public abstract void WriteLog(string message);

        // Конкретний метод, що додає часову мітку та викликає WriteLog
        public void LogTimestamp(string message)
        {
            string timestampedMessage = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {message}";
            WriteLog(timestampedMessage);
        }
    }

    // Похідний клас: Консольний логер
    public class ConsoleLogger : LoggerBase
    {
        public override void WriteLog(string message)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"[ConsoleLogger] {message}");
            Console.ResetColor();
        }
    }

    // Похідний клас: Дебаг-логер
    public class DebugLogger : LoggerBase
    {
        public override void WriteLog(string message)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"[DebugLogger] [DEBUG] {message}");
            Console.ResetColor();
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Console.WriteLine("=== ДЕМОНСТРАЦІЯ РОБОТИ ІНТЕРФЕЙСУ ISortable ===");
            List<ISortable> sorters = new List<ISortable>
            {
                new BubbleSort(),
                new QuickSort()
            };

            int[] data1 = { 64, 34, 25, 12, 22, 11, 90 };
            int[] data2 = { 5, 2, 9, 1, 5, 6 };

            // Поліморфний виклик через колекцію інтерфейсу
            for (int i = 0; i < sorters.Count; i++)
            {
                int[] sampleData = (i == 0) ? (int[])data1.Clone() : (int[])data2.Clone();
                Console.WriteLine($"\nПочатковий масив: {string.Join(", ", sampleData)}");
                sorters[i].Sort(sampleData);
                Console.WriteLine($"Відсортований масив: {string.Join(", ", sampleData)}");
                Console.WriteLine(new string('-', 40));
            }

            Console.WriteLine("\n=== ДЕМОНСТРАЦІЯ РОБОТИ АБСТРАКТНОГО КЛАСУ LoggerBase ===");
            List<LoggerBase> loggers = new List<LoggerBase>
            {
                new ConsoleLogger(),
                new DebugLogger()
            };

            // Поліморфний виклик через колекцію абстрактного класу
            foreach (var logger in loggers)
            {
                logger.LogTimestamp("Система успішно ініціалізована та готова до роботи.");
            }

            Console.WriteLine("\nРоботу програми завершено успішно.");
        }
    }
}