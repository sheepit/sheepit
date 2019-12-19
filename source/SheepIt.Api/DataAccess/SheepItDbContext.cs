using Microsoft.EntityFrameworkCore;
using SheepIt.Api.Core.Projects;

namespace SheepIt.Api.DataAccess
{
    public class SheepItDbContext : DbContext
    {
        public DbSet<Project> Projects { get; set; }
        
        public SheepItDbContext(DbContextOptions<SheepItDbContext> option)
            : base(option)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Project>().ToTable("Project");
        }
    }
}