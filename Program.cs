using Dapper;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Net.NetworkInformation;
using System.Security.AccessControl;

namespace Homework21
{

    // тестовый коммеентари
    //ЫВ
    //ЫВА
    //ЫВА
    //ЫВА
    //ЫВА
    //ЫВАfsdsfdsfdsdffsd
    //А
    //ЫВfdsdffsdfsdfsdfsdsfd
    // й
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
    }
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        public DbSet<Product> Products { get; set; }

        protected override void OnModelCreating(ModelBuilder mb)
        {
            mb.Entity<Product>().ToTable("Products");
        }
    }

    public class Program
    {
        readonly static string connString = "Data Source=products.db";
        static void Main(string[] args)
        {
            Console.WriteLine("Старт программы...");
            CreateTable();
            Console.WriteLine("\n--- Примеры EF ---");
            AddProductByEF();
            ShowProductsByEf();
            Console.WriteLine("\n\n--- Примеры Dapper");
            ShowProductsWithSortByDapper();
        }
        static void CreateTable()
        {
            var conn = new SqliteConnection(connString);
            conn.Open();

            using (var createTableCmd = conn.CreateCommand())
            {
                createTableCmd.CommandText = """
                        CREATE TABLE IF NOT EXISTS Products (
                            Id INTEGER PRIMARY KEY,
                            Name VARCHAR,
                            Price DECIMAL
                        )
                    """;
                createTableCmd.ExecuteNonQuery();
            }
        }

        static void AddProductByEF()
        {
            Console.WriteLine("\n=== Добавление через EF ===");
            var options = new DbContextOptionsBuilder<AppDbContext>()
               .UseSqlite(connString)
               .Options;
            using var context = new AppDbContext(options);

            Console.Write("Введите название продукта: ");
            string name = Console.ReadLine();
            Console.Write("Введите цену этого продукта: ");
            decimal price = Convert.ToDecimal(Console.ReadLine());

            var product = new Product { Name = name, Price = price};

            context.Products.Add(product);
            context.SaveChanges();
            Console.WriteLine("Добавлен новый продукт!");
        }

        static void ShowProductsByEf()
        {
            Console.WriteLine("\n=== Просмотр всех продуктов ===");
            var options = new DbContextOptionsBuilder<AppDbContext>()
               .UseSqlite(connString)
               .Options;
            using var context = new AppDbContext(options);
            var productList = context.Products.ToList();
            foreach (var p in productList)
                Console.WriteLine($"ID: {p.Id} | {p.Name} - {p.Price} рублей");
        }

        static void ShowProductsWithSortByDapper()
        {
            Console.WriteLine("=== Поиск продуктов выше заданной цены ===");
            var conn = new SqliteConnection(connString);
            conn.Open();

            Console.Write("Введите минимальную цену: ");
            decimal minPriceInput = Convert.ToDecimal(Console.ReadLine());

            var showProductsSortCmd = "SELECT * FROM Products WHERE Price > @minPrice";
            var parameters = new { minPrice = minPriceInput}; // Параметры в Dapper изучил в документациях
            var products = conn.Query<Product>(showProductsSortCmd, parameters);
            

            foreach (var p in products)
                Console.WriteLine($"ID: {p.Id} | {p.Name} - {p.Price} рублей");
        }
    }
}
