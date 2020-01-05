﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using SheepIt.Api.Core.Deployments;
using SheepIt.Api.Core.Packages;
using SheepIt.Api.DataAccess;

namespace SheepIt.Api.Migrations
{
    [DbContext(typeof(SheepItDbContext))]
    [Migration("20200105201507_table deployment")]
    partial class tabledeployment
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .HasAnnotation("ProductVersion", "3.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 63)
                .HasAnnotation("Relational:Sequence:.deployment", "'deployment', '', '1', '1', '', '', 'Int32', 'False'")
                .HasAnnotation("Relational:Sequence:.environment", "'environment', '', '1', '1', '', '', 'Int32', 'False'")
                .HasAnnotation("Relational:Sequence:.package", "'package', '', '1', '1', '', '', 'Int32', 'False'");

            modelBuilder.Entity("SheepIt.Api.Core.Deployments.Deployment", b =>
                {
                    b.Property<Guid>("ObjectId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<int>("EnvironmentId")
                        .HasColumnType("integer");

                    b.Property<int>("Id")
                        .HasColumnType("integer");

                    b.Property<int>("PackageId")
                        .HasColumnType("integer");

                    b.Property<ProcessOutput>("ProcessOutput")
                        .HasColumnType("jsonb");

                    b.Property<string>("ProjectId")
                        .HasColumnType("text");

                    b.Property<DateTime>("StartedAt")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("ObjectId");

                    b.ToTable("Deployment");
                });

            modelBuilder.Entity("SheepIt.Api.Core.Environments.Environment", b =>
                {
                    b.Property<Guid>("ObjectId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("DisplayName")
                        .HasColumnType("text");

                    b.Property<int>("Id")
                        .HasColumnType("integer");

                    b.Property<string>("ProjectId")
                        .HasColumnType("text");

                    b.Property<int>("Rank")
                        .HasColumnType("integer");

                    b.HasKey("ObjectId");

                    b.ToTable("Environment");
                });

            modelBuilder.Entity("SheepIt.Api.Core.Packages.Package", b =>
                {
                    b.Property<Guid>("ObjectId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp without time zone");

                    b.Property<int>("DeploymentProcessId")
                        .HasColumnType("integer");

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<int>("Id")
                        .HasColumnType("integer");

                    b.Property<string>("ProjectId")
                        .HasColumnType("text");

                    b.Property<VariableCollection>("Variables")
                        .HasColumnType("jsonb");

                    b.HasKey("ObjectId");

                    b.ToTable("Package");
                });

            modelBuilder.Entity("SheepIt.Api.Core.Projects.Project", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<Guid>("ObjectId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.ToTable("Project");
                });
#pragma warning restore 612, 618
        }
    }
}
