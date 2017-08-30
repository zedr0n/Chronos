using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Chronos.Persistence;

namespace Chronos.Persistence.Migrations
{
    [DbContext(typeof(ReadContext))]
    partial class ReadContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.2");

            modelBuilder.Entity("Chronos.Core.Accounts.Projections.AccountInfo", b =>
                {
                    b.Property<Guid>("Key")
                        .ValueGeneratedOnAdd();

                    b.Property<double>("Balance");

                    b.Property<DateTime>("CreatedAtUtc");

                    b.Property<string>("Currency");

                    b.Property<string>("Name");

                    b.HasKey("Key");

                    b.ToTable("Accounts");
                });

            modelBuilder.Entity("Chronos.Core.Accounts.Projections.TotalMovement", b =>
                {
                    b.Property<Guid>("Key")
                        .ValueGeneratedOnAdd();

                    b.Property<double>("Value");

                    b.HasKey("Key");

                    b.ToTable("Movements");
                });
        }
    }
}
