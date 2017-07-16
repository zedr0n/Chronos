using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Chronos.Persistence;

namespace Chronos.Persistence.Migrations
{
    [DbContext(typeof(EventContext))]
    partial class EventContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.2");

            modelBuilder.Entity("Chronos.Persistence.Event", b =>
                {
                    b.Property<int>("EventNumber")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid>("Guid");

                    b.Property<string>("Payload");

                    b.Property<int?>("StreamHashId");

                    b.Property<DateTime>("TimestampUtc");

                    b.HasKey("EventNumber");

                    b.HasIndex("StreamHashId");

                    b.ToTable("Events");
                });

            modelBuilder.Entity("Chronos.Persistence.Stream", b =>
                {
                    b.Property<int>("HashId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name");

                    b.Property<int>("Version");

                    b.HasKey("HashId");

                    b.ToTable("Streams");
                });

            modelBuilder.Entity("Chronos.Persistence.Event", b =>
                {
                    b.HasOne("Chronos.Persistence.Stream")
                        .WithMany("Events")
                        .HasForeignKey("StreamHashId");
                });
        }
    }
}
