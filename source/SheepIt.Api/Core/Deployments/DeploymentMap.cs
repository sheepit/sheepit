using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace SheepIt.Api.Core.Deployments
{
    public class DeploymentMap : IEntityTypeConfiguration<Deployment>
    {
        public void Configure(EntityTypeBuilder<Deployment> builder)
        {
            builder.ToTable("Deployment");

            builder.HasKey(deployment => deployment.Id);
            
            builder.Property(package => package.ProcessOutput)
                .HasColumnType("jsonb");

            builder.Property(package => package.Status)
                .HasConversion(new EnumToStringConverter<DeploymentStatus>());
        }
    }
}