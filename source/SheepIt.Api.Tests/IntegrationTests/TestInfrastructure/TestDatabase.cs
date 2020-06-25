using System.Data;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SheepIt.Api.DataAccess;

namespace SheepIt.Api.Tests.IntegrationTests.TestInfrastructure
{
    public static class TestDatabase
    {
        public static SheepItDbContext CreateSheepItDbContext(IConfiguration configuration)
        {
            var dbContextOptions = new DbContextOptionsBuilder<SheepItDbContext>()
                .UseNpgsql(configuration.GetConnectionString(SheepItDbContext.ConnectionStringName))
                .Options;

            return new SheepItDbContext(dbContextOptions);
        }

        public static async Task TruncateDatabase(SheepItDbContext dbContext)
        {
            await ExecuteNonQueryCommand(dbContext, @"
                DO
                $func$
                    DECLARE
                        statements CURSOR FOR
                            SELECT tablename FROM pg_tables
                            WHERE schemaname = 'public' and tablename <> '__EFMigrationsHistory';
                    BEGIN
                        FOR stmt IN statements LOOP
                            EXECUTE 'TRUNCATE TABLE ' || quote_ident(stmt.tablename) || ' CASCADE;';
                        END LOOP;
                    END;
                $func$;
            ");
        }

        public static async Task ResetSequences(SheepItDbContext dbContext)
        {
            await ExecuteNonQueryCommand(dbContext, @"
                SELECT SETVAL(c.oid, 1, false)
                FROM pg_class c JOIN pg_namespace n
                ON n.oid = c.relnamespace
                WHERE c.relkind = 'S' AND n.nspname = 'public';
            ");
        }

        private static async Task ExecuteNonQueryCommand(SheepItDbContext dbContext, string sqlCommand)
        {
            await using var command = dbContext.Database
                .GetDbConnection()
                .CreateCommand();

            command.CommandText = sqlCommand;
            command.CommandType = CommandType.Text;

            dbContext.Database.OpenConnection();

            await command.ExecuteNonQueryAsync();
        }
    }
}