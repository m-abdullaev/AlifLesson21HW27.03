using Microsoft.EntityFrameworkCore;
using WebApplication1.Models;

namespace WebApplication1.Context
{
    public class PersonContext : DbContext
    {
        public PersonContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Person> People { get; set; }
    }
}
