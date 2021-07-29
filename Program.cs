using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using LinqKit;
using Microsoft.EntityFrameworkCore;

namespace LinqKitReprex
{
    class Program
    {
        static async Task Main(string[] args)
        {

            var predicate = PredicateBuilder.New<Person>();

            var targets = new[] {
                new { Age = 24, Pets = 2 },
                new { Age = 25, Pets = 6 },
                new { Age = 26, Pets = 10 },
                new { Age = 27, Pets = 1 },
                new { Age = 28, Pets = 0 }
            };

            foreach (var target in targets)
            {
                predicate.Or(person => person.Age == target.Age && person.Pets.Count == target.Pets);
            }

            var context = new Context();
            await context.Database.MigrateAsync();
            await context.People
                .AsExpandable()
                .Where(predicate)
                .DeleteFromQueryAsync();
        }
    }

    class Person
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string Name { get; set; }

        public int Age { get; set; }

        public ICollection<Pet> Pets { get; set; }
    }

    class Pet
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
    }

    class Context : DbContext
    {
        public DbSet<Person> People { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlite("Filename=:memory:");
        }
    }
}
