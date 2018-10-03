﻿// <auto-generated />
using MachineCalculator.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace MachineCalculator.Migrations
{
    [DbContext(typeof(ServerCapacityContext))]
    [Migration("20181003102827_Init")]
    partial class Init
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn)
                .HasAnnotation("ProductVersion", "2.1.3-rtm-32065")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            modelBuilder.Entity("MachineCalculator.Models.AppObj", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("flag");

                    b.Property<string>("name");

                    b.HasKey("Id");

                    b.ToTable("AppObjDbSet");
                });

            modelBuilder.Entity("MachineCalculator.Models.AppParameters", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("AppId");

                    b.Property<double>("load");

                    b.Property<string>("name");

                    b.HasKey("Id");

                    b.HasIndex("AppId");

                    b.ToTable("AppParameterDbSet");
                });

            modelBuilder.Entity("MachineCalculator.Models.Project", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.HasKey("Id");

                    b.ToTable("ProjectDbSet");
                });

            modelBuilder.Entity("MachineCalculator.Models.ProjectApp", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("appId");

                    b.Property<int>("projectId");

                    b.HasKey("Id");

                    b.HasIndex("appId");

                    b.HasIndex("projectId");

                    b.ToTable("ProjectAppDbSet");
                });

            modelBuilder.Entity("MachineCalculator.Models.AppParameters", b =>
                {
                    b.HasOne("MachineCalculator.Models.AppObj", "AppObj")
                        .WithMany("AppParameters")
                        .HasForeignKey("AppId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("MachineCalculator.Models.ProjectApp", b =>
                {
                    b.HasOne("MachineCalculator.Models.AppObj", "AppObj")
                        .WithMany("ProjectApps")
                        .HasForeignKey("appId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("MachineCalculator.Models.Project", "Project")
                        .WithMany("projectApps")
                        .HasForeignKey("projectId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
