using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Chronos.Persistence.Migrations
{
    public partial class Base : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Streams",
                columns: table => new
                {
                    HashId = table.Column<int>(type: "INTEGER", nullable: false),
                    TimelineId = table.Column<Guid>(type: "BLOB", nullable: false),
                    BranchVersion = table.Column<int>(type: "INTEGER", nullable: false),
                    Key = table.Column<Guid>(type: "BLOB", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    SourceType = table.Column<string>(type: "TEXT", nullable: true),
                    Version = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Streams", x => new { x.HashId, x.TimelineId });
                });

            migrationBuilder.CreateTable(
                name: "Events",
                columns: table => new
                {
                    EventNumber = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Payload = table.Column<string>(type: "TEXT", nullable: true),
                    StreamHashId = table.Column<int>(type: "INTEGER", nullable: true),
                    StreamTimelineId = table.Column<Guid>(type: "BLOB", nullable: true),
                    TimestampUtc = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Version = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Events", x => x.EventNumber);
                    table.ForeignKey(
                        name: "FK_Events_Streams_StreamHashId_StreamTimelineId",
                        columns: x => new { x.StreamHashId, x.StreamTimelineId },
                        principalTable: "Streams",
                        principalColumns: new[] { "HashId", "TimelineId" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Events_StreamHashId_StreamTimelineId",
                table: "Events",
                columns: new[] { "StreamHashId", "StreamTimelineId" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Events");

            migrationBuilder.DropTable(
                name: "Streams");
        }
    }
}
