using Microsoft.EntityFrameworkCore;

namespace SheepIt.Api.DataAccess.Sequencing
{
    public static class SequencingModelBuilderExtensions
    {
        public static void ApplySequenceConfiguration(this ModelBuilder modelBuilder, IdSequence sequence)
        {
            modelBuilder.HasSequence<int>(sequence.Name)
                .StartsAt(1)
                .IncrementsBy(1);
        }
    }
}