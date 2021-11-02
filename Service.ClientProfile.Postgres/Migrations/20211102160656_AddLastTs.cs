using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Service.ClientProfile.Postgres.Migrations
{
    public partial class AddLastTs : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastChangeTimestamp",
                schema: "clientprofiles",
                table: "profiles");

            migrationBuilder.AddColumn<DateTime>(
                name: "LastTs",
                schema: "clientprofiles",
                table: "profiles",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "LastTs",
                schema: "clientprofiles",
                table: "blockers",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastTs",
                schema: "clientprofiles",
                table: "profiles");

            migrationBuilder.DropColumn(
                name: "LastTs",
                schema: "clientprofiles",
                table: "blockers");

            migrationBuilder.AddColumn<DateTime>(
                name: "LastChangeTimestamp",
                schema: "clientprofiles",
                table: "profiles",
                type: "timestamp without time zone",
                nullable: false,
                defaultValueSql: "current_timestamp");
        }
    }
}
