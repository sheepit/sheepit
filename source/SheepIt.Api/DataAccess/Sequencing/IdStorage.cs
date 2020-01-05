using System.Data;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace SheepIt.Api.DataAccess.Sequencing
{
    public class IdStorage
    {
        private readonly SheepItDbContext _context;

        public IdStorage(SheepItDbContext context)
        {
            _context = context;
        }        

        public async Task<int> GetNext(IdSequence sequence)
        {
            await using var command = _context.Database
                .GetDbConnection()
                .CreateCommand();
            
            command.CommandText = $"select nextval('{sequence.Name}')";
            command.CommandType = CommandType.Text;

            _context.Database.OpenConnection();

            var result = await command.ExecuteScalarAsync();

            return int.Parse(result.ToString(), CultureInfo.InvariantCulture);
        }
    }
}