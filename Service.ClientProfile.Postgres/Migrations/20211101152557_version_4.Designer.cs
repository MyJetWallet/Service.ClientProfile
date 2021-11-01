﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using Service.ClientProfile.Postgres;

namespace Service.ClientProfile.Postgres.Migrations
{
    [DbContext(typeof(DatabaseContext))]
    [Migration("20211101152557_version_4")]
    partial class version_4
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasDefaultSchema("clientprofiles")
                .HasAnnotation("Relational:MaxIdentifierLength", 63)
                .HasAnnotation("ProductVersion", "5.0.11")
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            modelBuilder.Entity("Service.ClientProfile.Domain.Models.Blocker", b =>
                {
                    b.Property<int>("BlockerId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<int>("BlockedOperationType")
                        .HasColumnType("integer");

                    b.Property<string>("ClientProfileClientId")
                        .HasColumnType("character varying(128)");

                    b.Property<DateTime>("ExpiryTime")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("Reason")
                        .HasColumnType("text");

                    b.HasKey("BlockerId");

                    b.HasIndex("ClientProfileClientId");

                    b.ToTable("blockers");
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

                    b.Property<bool>("PhoneConfirmed")
                        .HasColumnType("boolean");

                    b.Property<string>("ReferralCode")
                        .HasMaxLength(128)
                        .HasColumnType("character varying(128)");

                    b.Property<int>("Status2FA")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasDefaultValue(0);

                    b.HasKey("ClientId");

                    b.HasIndex("ClientId");

                    b.ToTable("profiles");
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
