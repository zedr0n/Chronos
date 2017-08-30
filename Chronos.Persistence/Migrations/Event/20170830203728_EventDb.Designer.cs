using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Chronos.Persistence;

namespace Chronos.Persistence.Migrations.Event
{
    [DbContext(typeof(EventContext))]
    [Migration("20170830203728_EventDb")]
    partial class EventDb
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.2");

            modelBuilder.Entity("Chronos.Persistence.Types.Event", b =>
                {
                    b.Property<int>("EventNumber")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Payload");

                    b.Property<int?>("StreamHashId");

                    b.Property<DateTime>("TimestampUtc");

                    b.HasKey("EventNumber");

                    b.HasIndex("StreamHashId");

                    b.ToTable("Events");
                });

            modelBuilder.Entity("Chronos.Persistence.Types.Stream", b =>
                {
                    b.Property<int>("HashId")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid>("Key");

                    b.Property<string>("Name");

                    b.Property<string>("SourceType");

                    b.Property<int>("Version");

                    b.HasKey("HashId");

                    b.ToTable("Streams");
                });

            modelBuilder.Entity("Chronos.Persistence.Types.Event", b =>
                {
                    b.HasOne("Chronos.Persistence.Types.Stream")
                        .WithMany("Events")
                        .HasForeignKey("StreamHashId");
                });
        }
    }
}
