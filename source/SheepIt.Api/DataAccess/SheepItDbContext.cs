using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using SheepIt.Api.Core.Projects;
using Environment = SheepIt.Api.Core.Environments.Environment;

namespace SheepIt.Api.DataAccess
{
    public class SheepItDbContext : DbContext
    {
        public DbSet<Project> Projects { get; set; }
        public DbSet<Environment> Environments { get; set; }
        
        public SheepItDbContext(DbContextOptions<SheepItDbContext> option)
            : base(option)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Project>().ToTable("Project");
            
            modelBuilder.ApplyConfiguration(new EnvironmentMap());

            modelBuilder.ApplySequenceConfiguration<Environment>();
        }
    }
}