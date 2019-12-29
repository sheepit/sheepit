using System;
using System.Data;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace SheepIt.Api.DataAccess
{
    public class IdStorage
    {
        private readonly SheepItDbContext _context;

        public IdStorage(SheepItDbContext context)
        {
            _context = context;
        }        

        public async Task<int> GetNext(Type identifiableObjectType)
        {
            using (var command = _context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = $"select nextval('{identifiableObjectType.Name.ToLower()}')";
                command.CommandType = CommandType.Text;

                _context.Database.OpenConnection();

                var result = await command.ExecuteScalarAsync();

                return int.Parse(result.ToString());
            }
        }
    }
}