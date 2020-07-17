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

        public Country[] GetAllCountries()
        {
            var countries = db.Countries
                .ToArray();

            return countries;
        }

        public bool IsCountryExist(string name)
        {
            return db.Countries
                .Any(b => b.Name == name);
        }

        public Country GetCountryByName(string name)
        {
            var country = db.Countries
                .FirstOrDefault(b => b.Name == name);

            return country;
        }

        public bool SetCountry(Country country)
        {
            country.LastUpdateTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

            db.Countries.Add(country);

            int result = db.SaveChanges();

            if (result == 1)
                return true;
            else
                return false;
        }

        public bool RemoveCountry(Country country)
        {
            db.Remove(country);

            int result = db.SaveChanges();

            if (result == 1)
                return true;
            else
                return false;
        }

        public bool UpdateCountry(Country newCountry)
        {
            newCountry.LastUpdateTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

            // Cannot be less than zero
            if (newCountry.DisgracePoints < 0)
                newCountry.DisgracePoints = 0;

            db.Update(newCountry);

            int result = db.SaveChanges();

            if (result == 1)
                return true;
            else
                return false;
        }
    }
}