﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TelegramBot.Infrastructure.Persistence;

#nullable disable

namespace TelegramBot.Infrastructure.Migrations
{
    [DbContext(typeof(TelegramContext))]
    partial class TelegramContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.10")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("Bank.Domain.Entities.City", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<DateTime>("LastUpdate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<double?>("Radius")
                        .HasColumnType("float");

                    b.HasKey("Id");

                    b.ToTable("Cities");
                });

            modelBuilder.Entity("Bank.Domain.Entities.Currency", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<decimal>("Buy")
                        .HasPrecision(8, 5)
                        .HasColumnType("decimal(8,5)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal>("Sell")
                        .HasPrecision(8, 5)
                        .HasColumnType("decimal(8,5)");

                    b.HasKey("Id");

                    b.ToTable("Currencies");
                });

            modelBuilder.Entity("Base.Domain.Entities.Location", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<int?>("CityId")
                        .HasColumnType("int");

                    b.Property<double>("Latitude")
                        .HasColumnType("float");

                    b.Property<double>("Longitude")
                        .HasColumnType("float");

                    b.Property<long?>("UserId")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("CityId")
                        .IsUnique()
                        .HasFilter("[CityId] IS NOT NULL");

                    b.HasIndex("UserId")
                        .IsUnique()
                        .HasFilter("[UserId] IS NOT NULL");

                    b.ToTable("Locations");
                });

            modelBuilder.Entity("TelegramBot.Domain.Entities.User", b =>
                {
                    b.Property<long>("Id")
                        .HasColumnType("bigint");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsActive")
                        .HasColumnType("bit");

                    b.Property<bool>("IsBot")
                        .HasColumnType("bit");

                    b.Property<bool?>("IsBuyOperation")
                        .HasColumnType("bit");

                    b.Property<string>("LanguageCode")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("LastName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("NearCityId")
                        .HasColumnType("int");

                    b.Property<int?>("SelectedCurrencyId")
                        .HasColumnType("int");

                    b.Property<string>("UserName")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("NearCityId");

                    b.HasIndex("SelectedCurrencyId");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("Base.Domain.Entities.Location", b =>
                {
                    b.HasOne("Bank.Domain.Entities.City", null)
                        .WithOne("Location")
                        .HasForeignKey("Base.Domain.Entities.Location", "CityId");

                    b.HasOne("TelegramBot.Domain.Entities.User", null)
                        .WithOne("Location")
                        .HasForeignKey("Base.Domain.Entities.Location", "UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("TelegramBot.Domain.Entities.User", b =>
                {
                    b.HasOne("Bank.Domain.Entities.City", "NearCity")
                        .WithMany()
                        .HasForeignKey("NearCityId");

                    b.HasOne("Bank.Domain.Entities.Currency", "SelectedCurrency")
                        .WithMany()
                        .HasForeignKey("SelectedCurrencyId")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.Navigation("NearCity");

                    b.Navigation("SelectedCurrency");
                });

            modelBuilder.Entity("Bank.Domain.Entities.City", b =>
                {
                    b.Navigation("Location");
                });

            modelBuilder.Entity("TelegramBot.Domain.Entities.User", b =>
                {
                    b.Navigation("Location");
                });
#pragma warning restore 612, 618
        }
    }
}