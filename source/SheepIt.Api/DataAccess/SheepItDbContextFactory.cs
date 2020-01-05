using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using SheepIt.Api.Infrastructure;

namespace SheepIt.Api.DataAccess
{
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