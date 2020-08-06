using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;
using OtherWorldBot.Data;
using OtherWorldBot.Entities;

namespace OtherWorldBot.Services
{
    public class DatabaseService : IDisposable
    {
        private readonly ApplicationContext db;
        private readonly SemaphoreSlim dbLock;

        public DatabaseService()
        {
            db = new ApplicationContext();
            dbLock = new SemaphoreSlim(1, 1);
        }

        public async Task<Country[]> GetAllCountriesAsync()
        {
            await dbLock.WaitAsync().ConfigureAwait(false);

            try
            {
                var countries = await db.Countries
                    .ToArrayAsync();

                return countries;
            }
            finally
            {
                dbLock.Release();
            }
        }

        public async Task<bool> IsCountryExistAsync(string name)
        {
            await dbLock.WaitAsync().ConfigureAwait(false);

            try
            {
                var result = await db.Countries
                    .AnyAsync(b => b.Name == name);

                return result;
            }
            finally
            {
                dbLock.Release();
            }
        }

        public async Task<Country> GetCountryByNameAsync(string name)
        {
            await dbLock.WaitAsync().ConfigureAwait(false);

            try
            {
                var country = await db.Countries
                    .FirstOrDefaultAsync(b => b.Name == name);

                return country;
            }
            finally
            {
                dbLock.Release();
            }
        }

        public async Task<bool> SetCountryAsync(Country country)
        {
            country.LastUpdateTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

            db.Countries.Add(country);

            await dbLock.WaitAsync().ConfigureAwait(false);

            try
            {
                int result = await db.SaveChangesAsync();

                if (result == 1)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            finally
            {
                dbLock.Release();
            }
        }

        public async Task<bool> RemoveCountryAsync(Country country)
        {
            db.Remove(country);

            await dbLock.WaitAsync().ConfigureAwait(false);

            try
            {
                int result = await db.SaveChangesAsync();

                if (result == 1)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            finally
            {
                dbLock.Release();
            }
        }

        public async Task<bool> UpdateCountryAsync(Country newCountry)
        {
            newCountry.LastUpdateTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

            // Cannot be less than zero
            if (newCountry.DisgracePoints < 0)
            {
                newCountry.DisgracePoints = 0;
            }

            db.Update(newCountry);

            await dbLock.WaitAsync().ConfigureAwait(false);

            try
            {
                int result = await db.SaveChangesAsync();

                if (result == 1)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            finally
            {
                dbLock.Release();
            }
        }

        void IDisposable.Dispose()
        {
            dbLock?.Dispose();
            db?.Dispose();
        }
    }
}