using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Service.ClientProfile.Postgres.Migrations
{
    public partial class version_8 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "LastChangeTimestamp",
                schema: "clientprofiles",
                table: "profiles",
                type: "timestamp without time zone",
                nullable: false,
                defaultValueSql: "current_timestamp",
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone",
                oldDefaultValue: new DateTime(2021, 11, 2, 11, 34, 52, 180, DateTimeKind.Utc).AddTicks(8660));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "LastChangeTimestamp",
                schema: "clientprofiles",
                table: "profiles",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(2021, 11, 2, 11, 34, 52, 180, DateTimeKind.Utc).AddTicks(8660),
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone",
                oldDefaultValueSql: "current_timestamp");
        }
    }
}
