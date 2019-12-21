using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
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
    public class SheepItDbContextFactory : IDesignTimeDbContextFactory<SheepItDbContext>
    {
        public SheepItDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<SheepItDbContext>();
            optionsBuilder.UseNpgsql(@"Host=localhost;Database=sheepit;Username=postgres;Password=postgres", opts => opts.CommandTimeout((int)TimeSpan.FromMinutes(10).TotalSeconds));
            return new SheepItDbContext(optionsBuilder.Options);
        }
    }
}