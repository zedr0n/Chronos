using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Chronos.Persistence.Migrations.Read
{
    public partial class Version : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "Timeline",
                table: "Stats",
                type: "BLOB",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<int>(
                name: "Version",
                table: "Stats",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "Timeline",
                table: "Movements",
                type: "BLOB",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<int>(
                name: "Version",
                table: "Movements",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "Timeline",
                table: "Coins",
                type: "BLOB",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<int>(
                name: "Version",
                table: "Coins",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "Timeline",
                table: "Accounts",
                type: "BLOB",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<int>(
                name: "Version",
                table: "Accounts",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "Timeline",
                table: "OrderStatuses",
                type: "BLOB",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<int>(
                name: "Version",
                table: "OrderStatuses",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "Timeline",
                table: "Orders",
                type: "BLOB",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<int>(
                name: "Version",
                table: "Orders",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Timeline",
                table: "Stats");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "Stats");

            migrationBuilder.DropColumn(
                name: "Timeline",
                table: "Movements");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "Movements");

            migrationBuilder.DropColumn(
                name: "Timeline",
                table: "Coins");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "Coins");

            migrationBuilder.DropColumn(
                name: "Timeline",
                table: "Accounts");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "Accounts");

            migrationBuilder.DropColumn(
                name: "Timeline",
                table: "OrderStatuses");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "OrderStatuses");

            migrationBuilder.DropColumn(
                name: "Timeline",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "Orders");
        }
    }
}
