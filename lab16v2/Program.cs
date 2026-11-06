using System;
using System.Collections.Generic;
using System.Linq;

namespace Lab16v2
{
    // 2. Створення класів сутностей згідно з варіантом 2
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
    }

    public class OrderDetail
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public Product Product { get; set; } = null!;
    }

    public class Order
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public List<OrderDetail> OrderDetails { get; set; } = new();
    }

    // 3. Створення власного методу розширення (Custom Extension Method)
    public static class OrderExtensions
    {
        // Метод розширення TotalOrderAmount() для IEnumerable<OrderDetail>
        public static decimal TotalOrderAmount(this IEnumerable<OrderDetail> orderDetails)
        {
            return orderDetails.Sum(od => od.Quantity * (od.Product?.Price ?? 0));
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.WriteLine("=== Лабораторна робота №16: LINQ (SelectMany, пагінація, custom extensions) ===");
            Console.WriteLine("--- Варіант 2: Замовлення та деталі замовлень з продуктами ---\n");

            // Ініціалізація тестових даних (продукти)
            var p1 = new Product { Id = 1, Name = "Ігрова миша", Price = 1200m };
            var p2 = new Product { Id = 2, Name = "Механічна клавіатура", Price = 2500m };
            var p3 = new Product { Id = 3, Name = "Монітор 24\"", Price = 7500m };
            var p4 = new Product { Id = 4, Name = "Бездротові навушники", Price = 3100m };
            var p5 = new Product { Id = 5, Name = "Килимок для миші", Price = 450m };

            // Ініціалізація тестових даних (замовлення з вкладеними колекціями OrderDetail)
            var orders = new List<Order>
            {
                new Order
                {
                    Id = 101,
                    OrderDate = new DateTime(2026, 10, 1),
                    OrderDetails = new List<OrderDetail>
                    {
                        new OrderDetail { Id = 1, ProductId = 1, Quantity = 2, Product = p1 },
                        new OrderDetail { Id = 2, ProductId = 5, Quantity = 1, Product = p5 }
                    }
                },
                new Order
                {
                    Id = 102,
                    OrderDate = new DateTime(2026, 10, 3),
                    OrderDetails = new List<OrderDetail>
                    {
                        new OrderDetail { Id = 3, ProductId = 2, Quantity = 1, Product = p2 },
                        new OrderDetail { Id = 4, ProductId = 4, Quantity = 1, Product = p4 }
                    }
                },
                new Order
                {
                    Id = 103,
                    OrderDate = new DateTime(2026, 10, 5),
                    OrderDetails = new List<OrderDetail>
                    {
                        new OrderDetail { Id = 5, ProductId = 3, Quantity = 1, Product = p3 },
                        new OrderDetail { Id = 6, ProductId = 1, Quantity = 1, Product = p1 },
                        new OrderDetail { Id = 7, ProductId = 5, Quantity = 2, Product = p5 }
                    }
                },
                new Order
                {
                    Id = 104,
                    OrderDate = new DateTime(2026, 10, 10),
                    OrderDetails = new List<OrderDetail>
                    {
                        new OrderDetail { Id = 8, ProductId = 4, Quantity = 2, Product = p4 },
                        new OrderDetail { Id = 9, ProductId = 2, Quantity = 1, Product = p2 }
                    }
                },
                new Order
                {
                    Id = 105,
                    OrderDate = new DateTime(2026, 10, 12),
                    OrderDetails = new List<OrderDetail>
                    {
                        new OrderDetail { Id = 10, ProductId = 3, Quantity = 2, Product = p3 }
                    }
                }
            };

            // ==========================================
            // ЗАПИТ 1: SelectMany — отримати всі продукти, які були замовлені, в одному списку (без дублікатів)
            // ==========================================
            Console.WriteLine("--- 1. Демонстрація SelectMany (Унікальні замовлені продукти) ---");
            var orderedProducts = orders
                .SelectMany(o => o.OrderDetails)      // Розгортаємо вкладені колекції деталей замовлень
                .Select(od => od.Product)             // Проектуємо на продукт
                .GroupBy(p => p.Id)                   // Групуємо за ID для видалення дублікатів
                .Select(g => g.First())
                .ToList();

            foreach (var prod in orderedProducts)
            {
                Console.WriteLine($"  - [ID: {prod.Id}] {prod.Name} — {prod.Price} грн");
            }
            Console.WriteLine();

            // ==========================================
            // ЗАПИТ 2: Пагінація — вивести всі замовлення по 2 замовлення на сторінку
            // ==========================================
            Console.WriteLine("--- 2. Демонстрація пагінації (Skip / Take по 2 замовлення) ---");
            int pageSize = 2;
            int totalOrders = orders.Count;
            int totalPages = (int)Math.Ceiling(totalOrders / (double)pageSize);

            for (int page = 1; page <= totalPages; page++)
            {
                Console.WriteLine($"\n[Сторінка {page} з {totalPages}]:");
                var pageOrders = orders
                    .Skip((page - 1) * pageSize)      // Пропускаємо елементи попередніх сторінок
                    .Take(pageSize)                   // Беремо кількість елементів на сторінку
                    .ToList();

                foreach (var ord in pageOrders)
                {
                    // Використання нашого власного методу розширення всередині виводу
                    decimal totalAmount = ord.OrderDetails.TotalOrderAmount();
                    Console.WriteLine($"  Замовлення #{ord.Id} від {ord.OrderDate:yyyy-MM-dd}: Сума = {totalAmount} грн (Кількість позицій: {ord.OrderDetails.Count})");
                }
            }
            Console.WriteLine();

            // ==========================================
            // ЗАПИТ 3: Custom Extension Method — обчислення суми замовлення через TotalOrderAmount()
            // ==========================================
            Console.WriteLine("--- 3. Демонстрація Custom Extension Method (TotalOrderAmount) ---");
            foreach (var ord in orders)
            {
                decimal sum = ord.OrderDetails.TotalOrderAmount();
                Console.WriteLine($"  Замовлення #{ord.Id}: загальна сума = {sum} грн");
            }
        }
    }
}