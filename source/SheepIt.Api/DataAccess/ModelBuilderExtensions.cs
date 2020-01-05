using Microsoft.EntityFrameworkCore;
using SheepIt.Api.DataAccess.Sequencing;

namespace SheepIt.Api.DataAccess
{
    public static class ModelBuilderExtensions
    {
        public static void ApplySequenceConfiguration(this ModelBuilder modelBuilder, IdSequence sequence)
        {
            modelBuilder.HasSequence<int>(sequence.Name)
                .StartsAt(1)
                .IncrementsBy(1);
        }
    }
}