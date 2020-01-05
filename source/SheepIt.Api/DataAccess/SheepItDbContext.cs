using Microsoft.EntityFrameworkCore;
using SheepIt.Api.Core.Environments;
using SheepIt.Api.Core.Packages;
using SheepIt.Api.Core.Projects;
using Environment = SheepIt.Api.Core.Environments.Environment;

// ReSharper disable UnusedAutoPropertyAccessor.Local - db sets are created by EF

namespace SheepIt.Api.DataAccess
{
    public class SheepItDbContext : DbContext
    {
        public static string ConnectionStringName = "SheepItContext";
        
        public DbSet<Project> Projects { get; private set; }
        public DbSet<Environment> Environments { get; private set; }
        public DbSet<Package> Packages { get; private set; }
        
        public SheepItDbContext(DbContextOptions<SheepItDbContext> option)
            : base(option)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new ProjectMap());
            
            modelBuilder.ApplyConfiguration(new EnvironmentMap());
            modelBuilder.ApplySequenceConfiguration<Environment>();

            modelBuilder.ApplyConfiguration(new PackageMap());
            modelBuilder.ApplySequenceConfiguration<Package>();
        }
    }
}