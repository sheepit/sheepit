using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SheepIt.Api.Model.Components
{
    public class ComponentMap : IEntityTypeConfiguration<Component>
    {
        public void Configure(EntityTypeBuilder<Component> builder)
        {
            builder.ToTable("Component");

            builder.HasKey(component => component.Id);

            builder.Property(component => component.ProjectId)
                .IsRequired();
        }
    }
}