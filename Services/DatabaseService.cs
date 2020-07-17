using DisgraceDiscordBot.Data;
using DisgraceDiscordBot.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DisgraceDiscordBot.Services
{
    public class DatabaseService
    {
        // SQLite3 (EF6)
        // Databse file: bot.db
        // Database name: DisgraceDiscordBot
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
                .OrderBy(b => b.Id)
                .ToArray();

            return countries;
        }

        public Country GetCountryByName(string name)
        {
            var country = db.Countries
                .OrderBy(b => b.Name == name)
                .First();

            return country;
        }

        public Country GetCountryById(int id)
        {
            var country = db.Countries
                .OrderBy(b => b.Id == id)
                .First();

            return country;
        }

        public bool SetCountry(Country country)
        {
            db.Countries.Add(country);

            db.SaveChanges();

            return true;
        }

        public bool RemoveCountry(Country country)
        {
            db.Remove(country);

            var result = db.SaveChanges();
            Console.WriteLine("DELETE>>>"+result);

            return true;
        }

        public bool UpdateCountry(int id, Country newCountry)
        {
            return false;
        }
    }
}