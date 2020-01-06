﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using SheepIt.Api.DataAccess;
using SheepIt.Api.Model.Deployments;
using SheepIt.Api.Model.Packages;

namespace SheepIt.Api.Migrations
{
    [DbContext(typeof(SheepItDbContext))]
    partial class SheepItDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .HasAnnotation("ProductVersion", "3.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 63)
                .HasAnnotation("Relational:Sequence:.deployment", "'deployment', '', '1', '1', '', '', 'Int32', 'False'")
                .HasAnnotation("Relational:Sequence:.deploymentprocess", "'deploymentprocess', '', '1', '1', '', '', 'Int32', 'False'")
                .HasAnnotation("Relational:Sequence:.environment", "'environment', '', '1', '1', '', '', 'Int32', 'False'")
                .HasAnnotation("Relational:Sequence:.package", "'package', '', '1', '1', '', '', 'Int32', 'False'");

            modelBuilder.Entity("SheepIt.Api.Core.DeploymentProcesses.DeploymentProcess", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<byte[]>("File")
                        .HasColumnType("bytea");

                    b.Property<string>("ProjectId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("ProjectId");

                    b.ToTable("DeploymentProcess");
                });

            modelBuilder.Entity("SheepIt.Api.Core.Deployments.Deployment", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<int>("EnvironmentId")
                        .HasColumnType("integer");

                    b.Property<int>("PackageId")
                        .HasColumnType("integer");

                    b.Property<ProcessOutput>("ProcessOutput")
                        .HasColumnType("jsonb");

                    b.Property<string>("ProjectId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("StartedAt")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("EnvironmentId");

                    b.HasIndex("PackageId");

                    b.HasIndex("ProjectId");

                    b.ToTable("Deployment");
                });

            modelBuilder.Entity("SheepIt.Api.Core.Environments.Environment", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("DisplayName")
                        .HasColumnType("text");

                    b.Property<string>("ProjectId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("Rank")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("ProjectId");

                    b.ToTable("Environment");
                });

            modelBuilder.Entity("SheepIt.Api.Core.Packages.Package", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp without time zone");

                    b.Property<int>("DeploymentProcessId")
                        .HasColumnType("integer");

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<string>("ProjectId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<VariableCollection>("Variables")
                        .HasColumnType("jsonb");

                    b.HasKey("Id");

                    b.HasIndex("DeploymentProcessId");

                    b.HasIndex("ProjectId");

                    b.ToTable("Package");
                });

            modelBuilder.Entity("SheepIt.Api.Core.Projects.Project", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Project");
                });

            modelBuilder.Entity("SheepIt.Api.Core.DeploymentProcesses.DeploymentProcess", b =>
                {
                    b.HasOne("SheepIt.Api.Core.Projects.Project", "Project")
                        .WithMany()
                        .HasForeignKey("ProjectId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("SheepIt.Api.Core.Deployments.Deployment", b =>
                {
                    b.HasOne("SheepIt.Api.Core.Environments.Environment", "Environment")
                        .WithMany("Deployments")
                        .HasForeignKey("EnvironmentId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SheepIt.Api.Core.Packages.Package", "Package")
                        .WithMany("Deployments")
                        .HasForeignKey("PackageId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SheepIt.Api.Core.Projects.Project", "Project")
                        .WithMany("Deployments")
                        .HasForeignKey("ProjectId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("SheepIt.Api.Core.Environments.Environment", b =>
                {
                    b.HasOne("SheepIt.Api.Core.Projects.Project", "Project")
                        .WithMany("Environments")
                        .HasForeignKey("ProjectId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("SheepIt.Api.Core.Packages.Package", b =>
                {
                    b.HasOne("SheepIt.Api.Core.DeploymentProcesses.DeploymentProcess", "DeploymentProcess")
                        .WithMany("Packages")
                        .HasForeignKey("DeploymentProcessId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SheepIt.Api.Core.Projects.Project", "Project")
                        .WithMany("Packages")
                        .HasForeignKey("ProjectId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
