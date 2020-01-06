using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace SheepIt.Api.Model.Deployments
{
    public class DeploymentMap : IEntityTypeConfiguration<Deployment>
    {
        public void Configure(EntityTypeBuilder<Deployment> builder)
        {
            builder.ToTable("Deployment");

            builder.HasKey(deployment => deployment.Id);
            
            builder.Property(deployment => deployment.ProjectId)
                .IsRequired();
            
            builder.Property(deployment => deployment.ProcessOutput)
                .HasColumnType("jsonb");

            builder.Property(deployment => deployment.Status)
                .HasConversion(new EnumToStringConverter<DeploymentStatus>());
        }
    }
}