using Microsoft.EntityFrameworkCore;
using pr_49.Model;
using System.Collections.Generic;

namespace pr_49.Context
{
    public class UserContext : DbContext
    {
        /// <summary>
        /// Данные из базы данных
        /// </summary>
        public DbSet<User> Users { get; set; }

        /// <summary>
        /// Конструктор контекста
        /// </summary>
        public UserContext()
        {
            Database.EnsureCreated();
            Users.Load();
        }

        /// <summary>
        /// Переопределяем метод конфигурации
        /// </summary>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) =>
            optionsBuilder.UseMySql("server=127.0.0.1;" +
                                  "uid=root;" +
                                  "pwd=;" +
                                  "database=FoodAPI",
                new MySqlServerVersion(new Version(8, 0, 11)));
    }
}
