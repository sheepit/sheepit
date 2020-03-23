using Microsoft.EntityFrameworkCore;
using SheepIt.Api.DataAccess.Sequencing;
using SheepIt.Api.Model.Components;
using SheepIt.Api.Model.DeploymentProcesses;
using SheepIt.Api.Model.Deployments;
using SheepIt.Api.Model.Environments;
using SheepIt.Api.Model.Packages;
using SheepIt.Api.Model.Projects;
using Environment = SheepIt.Api.Model.Environments.Environment;

// ReSharper disable UnusedAutoPropertyAccessor.Local - db sets are created by EF

namespace SheepIt.Api.DataAccess
{
    public class SheepItDbContext : DbContext
    {
        public static string ConnectionStringName = "SheepItContext";
        
        public DbSet<Project> Projects { get; private set; }
        public DbSet<Environment> Environments { get; private set; }
        public DbSet<Package> Packages { get; private set; }
        public DbSet<Deployment> Deployments { get; private set; }
        public DbSet<DeploymentProcess> DeploymentProcesses { get; private set; }
        public DbSet<Component> Components { get; set; }
        
        public SheepItDbContext(DbContextOptions<SheepItDbContext> option)
            : base(option)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new ProjectMap());
            
            modelBuilder.ApplyConfiguration(new EnvironmentMap());
            modelBuilder.ApplySequenceConfiguration(IdSequence.Environment);

            modelBuilder.ApplyConfiguration(new PackageMap());
            modelBuilder.ApplySequenceConfiguration(IdSequence.Package);

            modelBuilder.ApplyConfiguration(new DeploymentMap());
            modelBuilder.ApplySequenceConfiguration(IdSequence.Deployment);

            modelBuilder.ApplyConfiguration(new DeploymentProcessMap());
            modelBuilder.ApplySequenceConfiguration(IdSequence.DeploymentProcess);
        }
    }
}