using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using SheepIt.Api.Core.Environments;
using SheepIt.Api.Core.Packages;
using SheepIt.Api.Core.Projects;
using SheepIt.Api.Infrastructure;
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
            modelBuilder.Entity<Project>().ToTable("Project");
            
            modelBuilder.ApplyConfiguration(new EnvironmentMap());
            modelBuilder.ApplySequenceConfiguration<Environment>();

            modelBuilder.ApplyConfiguration(new PackageMap());
            modelBuilder.ApplySequenceConfiguration<Package>();
        }
    }
    
    // ReSharper disable once UnusedMember.Global - this class is used by EF CLI tool, e.g. when creating migrations
    public class SheepItDbContextFactory : IDesignTimeDbContextFactory<SheepItDbContext>
    {
        public SheepItDbContext CreateDbContext(string[] args)
        {
            var configuration = ConfigurationFactory.CreateConfiguration(args);
            var connectionString = configuration.GetConnectionString(SheepItDbContext.ConnectionStringName);

            var optionsBuilder = new DbContextOptionsBuilder<SheepItDbContext>();
            
            optionsBuilder.UseNpgsql(connectionString, opts => opts
                .CommandTimeout((int)TimeSpan.FromMinutes(10).TotalSeconds)
            );
            
            return new SheepItDbContext(optionsBuilder.Options);
        }
    }
}