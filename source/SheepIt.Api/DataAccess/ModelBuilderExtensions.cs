using Microsoft.EntityFrameworkCore;

namespace SheepIt.Api.DataAccess
{
    public static class ModelBuilderExtensions
    {
        public static void ApplySequenceConfiguration<TEntity>(this ModelBuilder modelBuilder)
        {
            modelBuilder.HasSequence<int>(typeof(TEntity).Name.ToLower())
                .StartsAt(1)
                .IncrementsBy(1);
        }
    }
}