﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TradeJournal.Data;

#nullable disable

namespace TradeJournal.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20240822082807_Journal")]
    partial class Journal
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.7")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("TradeJournal.Models.Journal", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Text")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("TradeId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("TradeId");

                    b.ToTable("Journals");
                });

            modelBuilder.Entity("TradeJournal.Models.Trade", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<float>("Comission")
                        .HasColumnType("real");

                    b.Property<float>("EntryPrice")
                        .HasColumnType("real");

                    b.Property<string>("PositionType")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<float>("PositionVolume")
                        .HasColumnType("real");

                    b.Property<float>("PriceChange")
                        .HasColumnType("real");

                    b.Property<float>("StopLoss")
                        .HasColumnType("real");

                    b.Property<float>("Swap")
                        .HasColumnType("real");

                    b.Property<string>("SymbolName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<float>("TakeProfit")
                        .HasColumnType("real");

                    b.Property<DateTime>("TradeAdded")
                        .HasColumnType("datetime2");

                    b.Property<float>("TradeOutcome")
                        .HasColumnType("real");

                    b.Property<DateTime>("TransactionCloseDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("TransactionOpenDate")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.ToTable("Trades");
                });

            modelBuilder.Entity("TradeJournal.Models.TradingAccount", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Login")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Server")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("StartingBalance")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("TradingAccounts");
                });

            modelBuilder.Entity("TradeJournal.Models.Journal", b =>
                {
                    b.HasOne("TradeJournal.Models.Trade", "Trade")
                        .WithMany("Journals")
                        .HasForeignKey("TradeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Trade");
                });

            modelBuilder.Entity("TradeJournal.Models.Trade", b =>
                {
                    b.Navigation("Journals");
                });
#pragma warning restore 612, 618
        }
    }
}
