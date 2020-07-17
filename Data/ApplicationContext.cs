using DisgraceDiscordBot.Entities;
using Microsoft.EntityFrameworkCore;

namespace DisgraceDiscordBot.Data
{
    public class ApplicationContext : DbContext
    {
        public DbSet<Country> Countries { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite("Data Source=bot.db");
    }
}