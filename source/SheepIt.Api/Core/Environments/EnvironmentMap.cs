using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SheepIt.Api.Core.Environments;

namespace SheepIt.Api.DataAccess
{
    public class EnvironmentMap : IEntityTypeConfiguration<Environment>
    {
        public void Configure(EntityTypeBuilder<Environment> builder)
        {
            builder.ToTable("Environment");

            builder.HasKey(x => x.ObjectId);
        }
    }
}