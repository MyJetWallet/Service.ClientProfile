﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using Service.ClientProfile.Postgres;

#nullable disable

namespace Service.ClientProfile.Postgres.Migrations
{
    [DbContext(typeof(DatabaseContext))]
    partial class DatabaseContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasDefaultSchema("clientprofiles")
                .HasAnnotation("ProductVersion", "6.0.1")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Service.ClientProfile.Domain.Models.Blocker", b =>
                {
                    b.Property<int>("BlockerId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("BlockerId"));

                    b.Property<int>("BlockedOperationType")
                        .HasColumnType("integer");

                    b.Property<string>("ClientProfileClientId")
                        .HasColumnType("character varying(128)");

                    b.Property<DateTime>("ExpiryTime")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime>("LastTs")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp with time zone")
                        .HasDefaultValue(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

                    b.Property<string>("Reason")
                        .HasColumnType("text");

                    b.HasKey("BlockerId");

                    b.HasIndex("ClientProfileClientId");

                    b.HasIndex("LastTs");

                    b.ToTable("blockers", "clientprofiles");
                });

            modelBuilder.Entity("Service.ClientProfile.Domain.Models.ClientProfile", b =>
                {
                    b.Property<string>("ClientId")
                        .HasMaxLength(128)
                        .HasColumnType("character varying(128)");

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("boolean");

                    b.Property<bool>("KYCPassed")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("boolean")
                        .HasDefaultValue(false);

                    b.Property<DateTime>("LastTs")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp with time zone")
                        .HasDefaultValue(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

                    b.Property<bool>("PhoneConfirmed")
                        .HasColumnType("boolean");

                    b.Property<string>("ReferralCode")
                        .HasMaxLength(128)
                        .HasColumnType("character varying(128)");

                    b.Property<string>("ReferrerClientId")
                        .HasMaxLength(128)
                        .HasColumnType("character varying(128)");

                    b.Property<int>("Status2FA")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasDefaultValue(0);

                    b.HasKey("ClientId");

                    b.HasIndex("ClientId");

                    b.HasIndex("LastTs");

                    b.HasIndex("ReferralCode");

                    b.HasIndex("ReferrerClientId");

                    b.ToTable("profiles", "clientprofiles");
                });

            modelBuilder.Entity("Service.ClientProfile.Domain.Models.Blocker", b =>
                {
                    b.HasOne("Service.ClientProfile.Domain.Models.ClientProfile", null)
                        .WithMany("Blockers")
                        .HasForeignKey("ClientProfileClientId");
                });

            modelBuilder.Entity("Service.ClientProfile.Domain.Models.ClientProfile", b =>
                {
                    b.Navigation("Blockers");
                });
#pragma warning restore 612, 618
        }
    }
}
