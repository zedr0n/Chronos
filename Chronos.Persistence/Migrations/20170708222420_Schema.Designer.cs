using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Chronos.Persistence;

namespace Chronos.Persistence.Migrations
{
    [DbContext(typeof(EventContext))]
    [Migration("20170708222420_Schema")]
    partial class Schema
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.2");

            modelBuilder.Entity("Chronos.Persistence.Event", b =>
                {
                    b.Property<int>("EventNumber")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid>("Guid");

                    b.Property<string>("Payload");

                    b.Property<string>("StreamName");

                    b.Property<DateTime>("TimestampUtc");

                    b.HasKey("EventNumber");

                    b.HasIndex("StreamName");

                    b.ToTable("Events");
                });

            modelBuilder.Entity("Chronos.Persistence.Stream", b =>
                {
                    b.Property<string>("Name")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("Version");

                    b.HasKey("Name");

                    b.ToTable("Streams");
                });

            modelBuilder.Entity("Chronos.Persistence.Event", b =>
                {
                    b.HasOne("Chronos.Persistence.Stream")
                        .WithMany("Events")
                        .HasForeignKey("StreamName");
                });
        }
    }
}
