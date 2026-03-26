using Microsoft.EntityFrameworkCore;
using pr_49.Model;

namespace pr_49.Context
{
    public class OrderContext : DbContext
    {
        public DbSet<Order> Orders { get; set; }

        public OrderContext()
        {
            Database.EnsureCreated();
            Orders.Load();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) =>
            optionsBuilder.UseMySql("server=127.0.0.1;" +
                                  "uid=root;" +
                                  "pwd=;" +
                                  "database=FoodAPI",
                new MySqlServerVersion(new Version(8, 0, 11)));
    }
}
