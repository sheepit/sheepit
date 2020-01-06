using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SheepIt.Api.Model.DeploymentProcesses
{
    public class DeploymentProcessMap : IEntityTypeConfiguration<DeploymentProcess>
    {
        public void Configure(EntityTypeBuilder<DeploymentProcess> builder)
        {
            builder.ToTable("DeploymentProcess");
            
            builder.HasKey(deploymentProcess => deploymentProcess.Id);

            builder.Property(deploymentProcess => deploymentProcess.ProjectId)
                .IsRequired();
        }
    }
}