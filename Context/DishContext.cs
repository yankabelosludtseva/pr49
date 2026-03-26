using Microsoft.EntityFrameworkCore;
using pr_49.Model;
using System.Collections.Generic;

namespace pr_49.Context
{
    public class DishContext : DbContext
    {
        public DbSet<Dish> Dishes { get; set; }

        public DishContext()
        {
            Database.EnsureCreated();
            Dishes.Load();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) =>
            optionsBuilder.UseMySql("server=127.0.0.1;" +
                                  "uid=root;" +
                                  "pwd=;" +
                                  "database=FoodAPI",
                new MySqlServerVersion(new Version(8, 0, 11)));
    }
}
