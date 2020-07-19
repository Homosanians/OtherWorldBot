using OtherWorldBot.Data;
using OtherWorldBot.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OtherWorldBot.Services
{
    public class DatabaseService
    {
        // SQLite3 (EF6)
        // Databse file: bot.db
        // Collection name: Counties
        // Entry name: Country
       
        private ApplicationContext db;

        private SemaphoreSlim ss = new SemaphoreSlim(1, 1);

        public DatabaseService()
        {
            db = new ApplicationContext();
        }

        public async Task<Country[]> GetAllCountriesAsync()
        {
            await ss.WaitAsync();

            var countries = await db.Countries
                .ToArrayAsync();

            ss.Release();

            return countries;
        }

        public async Task<bool> IsCountryExistAsync(string name)
        {
            await ss.WaitAsync();

            var result = await db.Countries
                .AnyAsync(b => b.Name == name);

            ss.Release();

            return result;
        }

        public async Task<Country> GetCountryByNameAsync(string name)
        {
            await ss.WaitAsync();

            var country = await db.Countries
                .FirstOrDefaultAsync(b => b.Name == name);

            ss.Release();

            return country;
        }

        public async Task<bool> SetCountryAsync(Country country)
        {
            country.LastUpdateTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

            db.Countries.Add(country);

            await ss.WaitAsync();

            int result = await db.SaveChangesAsync();

            ss.Release();

            if (result == 1)
                return true;
            else
                return false;
        }

        public async Task<bool> RemoveCountryAsync(Country country)
        {
            db.Remove(country);

            await ss.WaitAsync();

            int result = await db.SaveChangesAsync();

            ss.Release();

            if (result == 1)
                return true;
            else
                return false;
        }

        public async Task<bool> UpdateCountryAsync(Country newCountry)
        {
            newCountry.LastUpdateTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

            // Cannot be less than zero
            if (newCountry.DisgracePoints < 0)
                newCountry.DisgracePoints = 0;

            db.Update(newCountry);

            await ss.WaitAsync();

            int result = await db.SaveChangesAsync();

            ss.Release();

            if (result == 1)
                return true;
            else
                return false;
        }
    }
}