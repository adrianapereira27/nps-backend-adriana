using Microsoft.EntityFrameworkCore;
using nps_backend_adriana.Models.Entities;
using System.Reflection;

namespace nps_backend_adriana.Services
{
    public class NpsDbContext : DbContext
    {
        public NpsDbContext(DbContextOptions<NpsDbContext> options) : base(options)
        {
        }

        public DbSet<NpsLog> NpsLog { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        }
    }
}
