﻿// <auto-generated />
using Chronos.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.Internal;
using System;

namespace Chronos.Persistence.Migrations.Read
{
    [DbContext(typeof(ReadContext))]
    [Migration("20171231173022_BagHistory")]
    partial class BagHistory
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.0.0-rtm-26452");

            modelBuilder.Entity("Chronos.Core.Accounts.Projections.AccountInfo", b =>
                {
                    b.Property<Guid>("Key")
                        .ValueGeneratedOnAdd();

                    b.Property<double>("Balance");

                    b.Property<DateTime>("CreatedAtUtc");

                    b.Property<string>("Currency");

                    b.Property<string>("Name");

                    b.Property<Guid>("Timeline");

                    b.Property<int>("Version");

                    b.HasKey("Key");

                    b.ToTable("Accounts");
                });

            modelBuilder.Entity("Chronos.Core.Accounts.Projections.TotalMovement", b =>
                {
                    b.Property<Guid>("Key")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid>("Timeline");

                    b.Property<double>("Value");

                    b.Property<int>("Version");

                    b.HasKey("Key");

                    b.ToTable("Movements");
                });

            modelBuilder.Entity("Chronos.Core.Assets.Projections.BagInfo", b =>
                {
                    b.Property<Guid>("Key")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name");

                    b.Property<Guid>("Timeline");

                    b.Property<double>("Value");

                    b.Property<int>("Version");

                    b.HasKey("Key");

                    b.ToTable("Bags");
                });

            modelBuilder.Entity("Chronos.Core.Assets.Projections.CoinInfo", b =>
                {
                    b.Property<Guid>("Key")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name");

                    b.Property<double>("Price");

                    b.Property<string>("Ticker");

                    b.Property<Guid>("Timeline");

                    b.Property<int>("Version");

                    b.HasKey("Key");

                    b.ToTable("Coins");
                });

            modelBuilder.Entity("Chronos.Core.Nicehash.Projections.OrderInfo", b =>
                {
                    b.Property<Guid>("Key")
                        .ValueGeneratedOnAdd();

                    b.Property<double>("MaxSpeed");

                    b.Property<int>("OrderNumber");

                    b.Property<Guid>("PriceAsset");

                    b.Property<Guid>("Timeline");

                    b.Property<int>("Version");

                    b.HasKey("Key");

                    b.ToTable("Orders");
                });

            modelBuilder.Entity("Chronos.Core.Nicehash.Projections.OrderStatus", b =>
                {
                    b.Property<Guid>("Key")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("OrderNumber");

                    b.Property<double>("Speed");

                    b.Property<double>("Spent");

                    b.Property<Guid>("Timeline");

                    b.Property<int>("Version");

                    b.HasKey("Key");

                    b.ToTable("OrderStatuses");
                });

            modelBuilder.Entity("Chronos.Core.Projections.Stats", b =>
                {
                    b.Property<string>("Key")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("NumberOfAccounts");

                    b.Property<int>("NumberOfAssets");

                    b.Property<Guid>("Timeline");

                    b.Property<int>("Version");

                    b.HasKey("Key");

                    b.ToTable("Stats");
                });
#pragma warning restore 612, 618
        }
    }
}
