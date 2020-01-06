using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SheepIt.Api.Core.Packages
{
    public class PackageMap : IEntityTypeConfiguration<Package>
    {
        public void Configure(EntityTypeBuilder<Package> builder)
        {
            builder.ToTable("Package");

            builder.HasKey(package => package.Id);

            builder.Property(package => package.Variables)
                .HasColumnType("jsonb");
        }
    }
}