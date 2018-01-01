using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Chronos.Persistence.Migrations.Read
{
    public partial class BagHistory2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BagHistories",
                columns: table => new
                {
                    Key = table.Column<Guid>(type: "BLOB", nullable: false),
                    Timeline = table.Column<Guid>(type: "BLOB", nullable: false),
                    Values = table.Column<string>(type: "TEXT", nullable: true),
                    Version = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BagHistories", x => x.Key);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BagHistories");
        }
    }
}
