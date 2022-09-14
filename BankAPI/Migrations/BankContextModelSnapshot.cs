﻿// <auto-generated />
using System;
using BankAPI.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace BankAPI.Migrations
{
    [DbContext(typeof(BankContext))]
    partial class BankContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "6.0.8");

            modelBuilder.Entity("BankAPI.Models.Bank", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("FullName")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("ShortName")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Banks");
                });

            modelBuilder.Entity("BankAPI.Models.City", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("LastUpdate")
                        .HasColumnType("TEXT");

                    b.Property<int?>("LocationId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<double?>("Radius")
                        .HasColumnType("REAL");

                    b.HasKey("Id");

                    b.HasIndex("LocationId");

                    b.ToTable("Cities");
                });

            modelBuilder.Entity("BankAPI.Models.Currencies.Currency", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int?>("BankId")
                        .HasColumnType("INTEGER");

                    b.Property<decimal>("Buy")
                        .HasColumnType("TEXT");

                    b.Property<int?>("DepartmentId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<decimal>("Sell")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("BankId");

                    b.HasIndex("DepartmentId");

                    b.ToTable("Currencies");
                });

            modelBuilder.Entity("BankAPI.Models.Department", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int?>("BankId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("CityId")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("LocationId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Street")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("BankId");

                    b.HasIndex("CityId");

                    b.HasIndex("LocationId");

                    b.ToTable("Departments");
                });

            modelBuilder.Entity("BankAPI.Models.Location", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<double>("Latitude")
                        .HasColumnType("REAL");

                    b.Property<double>("Longitude")
                        .HasColumnType("REAL");

                    b.HasKey("Id");

                    b.ToTable("Location");
                });

            modelBuilder.Entity("BankAPI.Models.City", b =>
                {
                    b.HasOne("BankAPI.Models.Location", "Location")
                        .WithMany()
                        .HasForeignKey("LocationId");

                    b.Navigation("Location");
                });

            modelBuilder.Entity("BankAPI.Models.Currencies.Currency", b =>
                {
                    b.HasOne("BankAPI.Models.Bank", null)
                        .WithMany("BestCurrencies")
                        .HasForeignKey("BankId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("BankAPI.Models.Department", null)
                        .WithMany("Currencies")
                        .HasForeignKey("DepartmentId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("BankAPI.Models.Department", b =>
                {
                    b.HasOne("BankAPI.Models.Bank", null)
                        .WithMany("Departments")
                        .HasForeignKey("BankId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("BankAPI.Models.City", "City")
                        .WithMany()
                        .HasForeignKey("CityId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("BankAPI.Models.Location", "Location")
                        .WithMany()
                        .HasForeignKey("LocationId");

                    b.Navigation("City");

                    b.Navigation("Location");
                });

            modelBuilder.Entity("BankAPI.Models.Bank", b =>
                {
                    b.Navigation("BestCurrencies");

                    b.Navigation("Departments");
                });

            modelBuilder.Entity("BankAPI.Models.Department", b =>
                {
                    b.Navigation("Currencies");
                });
#pragma warning restore 612, 618
        }
    }
}
