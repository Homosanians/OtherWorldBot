using DisgraceDiscordBot.Data;
using DisgraceDiscordBot.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DisgraceDiscordBot.Services
{
    class DatabaseService
    {
        // SQLite3 (EF6)
        // Databse file: bot.db
        // Database name: DisgraceDiscordBot
        // Collection name: Counties
        // Entry name: Country

        public DatabaseService()
        {
            Console.WriteLine("алё");

            using (var db = new ApplicationContext())
            {
                // Create
                Console.WriteLine("Add New Employee: ");
                db.Countries.Add(new Country { FirstName = "John", LastName = "Doe", Age = 55 });
                db.SaveChanges();

                Console.WriteLine("Employee has been added sucessfully.");

                // Read
                Console.WriteLine("Querying table for that employee.");
                var employee = db.Employees
                    .OrderBy(b => b.Id)
                    .First();

                Console.WriteLine("The employee found: {0} {1} and is {2} years old.", employee.FirstName, employee.LastName, employee.Age);

                // Update
                Console.WriteLine("Updating the employee first name and age.");

                employee.FirstName = "Louis";
                employee.Age = 90;

                Console.WriteLine("Newly updated employee is: {0} {1} and is {2} years old.", employee.FirstName, employee.LastName, employee.Age);

                db.SaveChanges();

                // Delete
                Console.WriteLine("Delete the employee.");

                db.Remove(employee);
                db.SaveChanges();
            }
        }
    }
}
