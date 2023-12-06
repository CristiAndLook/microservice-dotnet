using Microsoft.EntityFrameworkCore;
using micronet.Models;

namespace micronet.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        { }

        public DbSet<Adult> Adults { get; set; }
        public DbSet<Child> Children { get; set; }
    }
}