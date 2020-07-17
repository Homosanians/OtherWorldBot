﻿using DisgraceDiscordBot.Data;
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
            db.Update(newCountry);

            int result = db.SaveChanges();

            if (result == 1)
                return true;
            else
                return false;
        }
    }
}