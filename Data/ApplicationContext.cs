using OtherWorldBot.Entities;
using Microsoft.EntityFrameworkCore;

namespace OtherWorldBot.Data
{
    public class ApplicationContext : DbContext
    {
        public DbSet<Country> Countries { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite("Data Source=bot.db");
    }
}