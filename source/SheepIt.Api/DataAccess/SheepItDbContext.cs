﻿using Microsoft.EntityFrameworkCore;
using SheepIt.Api.Core.DeploymentProcesses;
using SheepIt.Api.Core.Deployments;
using SheepIt.Api.Core.Environments;
using SheepIt.Api.Core.Packages;
using SheepIt.Api.Core.Projects;
using SheepIt.Api.DataAccess.Sequencing;
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
        public DbSet<Deployment> Deployments { get; private set; }
        public DbSet<DeploymentProcess> DeploymentProcesses { get; private set; }
        
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