using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Chronos.Persistence.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AggregateIds",
                columns: table => new
                {
                    Guid = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AggregateIds", x => x.Guid);
                });

            migrationBuilder.CreateTable(
                name: "Streams",
                columns: table => new
                {
                    Name = table.Column<string>(nullable: false),
                    Version = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Streams", x => x.Name);
                });

            migrationBuilder.CreateTable(
                name: "Events",
                columns: table => new
                {
                    EventNumber = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Guid = table.Column<Guid>(nullable: false),
                    Payload = table.Column<string>(nullable: true),
                    StreamName = table.Column<string>(nullable: true),
                    TimestampUtc = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Events", x => x.EventNumber);
                    table.ForeignKey(
                        name: "FK_Events_Streams_StreamName",
                        column: x => x.StreamName,
                        principalTable: "Streams",
                        principalColumn: "Name",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Events_StreamName",
                table: "Events",
                column: "StreamName");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AggregateIds");

            migrationBuilder.DropTable(
                name: "Events");

            migrationBuilder.DropTable(
                name: "Streams");
        }
    }
}
