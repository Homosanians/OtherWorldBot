using DisgraceDiscordBot.Data;
using DisgraceDiscordBot.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DisgraceDiscordBot.Services
{
    public class DatabaseService
    {
        // SQLite3 (EF6)
        // Databse file: bot.db
        // Collection name: Counties
        // Entry name: Country
       
        private ApplicationContext db;

        public DatabaseService()
        {
            db = new ApplicationContext();
        }

        public async Task<Country[]> GetAllCountries()
        {
            var countries = await db.Countries
                .ToArrayAsync();

            return countries;
        }

        public async Task<bool> IsCountryExist(string name)
        {
            return await db.Countries
                .AnyAsync(b => b.Name == name);
        }

        public async Task<Country> GetCountryByName(string name)
        {
            var country = await db.Countries
                .FirstOrDefaultAsync(b => b.Name == name);

            return country;
        }

        public async Task<bool> SetCountry(Country country)
        {
            country.LastUpdateTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

            db.Countries.Add(country);

            int result = await db.SaveChangesAsync();

            if (result == 1)
                return true;
            else
                return false;
        }

        public async Task<bool> RemoveCountry(Country country)
        {
            db.Remove(country);

            int result = await db.SaveChangesAsync();

            if (result == 1)
                return true;
            else
                return false;
        }

        public async Task<bool> UpdateCountry(Country newCountry)
        {
            newCountry.LastUpdateTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

            // Cannot be less than zero
            if (newCountry.DisgracePoints < 0)
                newCountry.DisgracePoints = 0;

            db.Update(newCountry);

            int result = await db.SaveChangesAsync();

            if (result == 1)
                return true;
            else
                return false;
        }
    }
}