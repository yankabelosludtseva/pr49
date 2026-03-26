using Microsoft.EntityFrameworkCore;
using pr_49.Model;
using System.Collections.Generic;

namespace pr_49.Context
{
    public class MenuContext : DbContext
    {
        public DbSet<Menu> Menus { get; set; }

        public MenuContext()
        {
            Database.EnsureCreated();
            Menus.Load();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) =>
            optionsBuilder.UseMySql("server=127.0.0.1;" +
                                  "uid=root;" +
                                  "pwd=;" +
                                  "database=FoodAPI",
                new MySqlServerVersion(new Version(8, 0, 11)));
    }
}
