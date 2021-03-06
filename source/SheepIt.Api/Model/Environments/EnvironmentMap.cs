﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SheepIt.Api.Model.Environments
{
    public class EnvironmentMap : IEntityTypeConfiguration<Environment>
    {
        public void Configure(EntityTypeBuilder<Environment> builder)
        {
            builder.ToTable("Environment");

            builder.HasKey(environment => environment.Id);
            
            builder.Property(environment => environment.ProjectId)
                .IsRequired();
        }
    }
}