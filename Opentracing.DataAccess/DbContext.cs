using System;
using Microsoft.EntityFrameworkCore;
namespace Opentracing.DataAccess
{
    public class TracingDbContext:DbContext
    {
        public DbSet<Person> Persons { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder builder){
            builder.UseInMemoryDatabase(databaseName: "inmemeory");
        }
    }

    public class Person{
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
