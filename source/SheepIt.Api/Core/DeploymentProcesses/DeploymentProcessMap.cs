using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SheepIt.Api.Core.DeploymentProcesses
{
    public class DeploymentProcessMap : IEntityTypeConfiguration<DeploymentProcess>
    {
        public void Configure(EntityTypeBuilder<DeploymentProcess> builder)
        {
            builder.ToTable("DeploymentProcess");
            
            builder.HasKey(deploymentProcess => deploymentProcess.Id);
        }
    }
}