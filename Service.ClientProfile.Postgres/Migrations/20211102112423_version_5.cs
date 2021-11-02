using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Service.ClientProfile.Postgres.Migrations
{
    public partial class version_5 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "LastChangeTimestamp",
                schema: "clientprofiles",
                table: "profiles",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "ReferrerClientId",
                schema: "clientprofiles",
                table: "profiles",
                type: "character varying(128)",
                maxLength: 128,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_profiles_ReferralCode",
                schema: "clientprofiles",
                table: "profiles",
                column: "ReferralCode");

            migrationBuilder.CreateIndex(
                name: "IX_profiles_ReferrerClientId",
                schema: "clientprofiles",
                table: "profiles",
                column: "ReferrerClientId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_profiles_ReferralCode",
                schema: "clientprofiles",
                table: "profiles");

            migrationBuilder.DropIndex(
                name: "IX_profiles_ReferrerClientId",
                schema: "clientprofiles",
                table: "profiles");

            migrationBuilder.DropColumn(
                name: "LastChangeTimestamp",
                schema: "clientprofiles",
                table: "profiles");

            migrationBuilder.DropColumn(
                name: "ReferrerClientId",
                schema: "clientprofiles",
                table: "profiles");
        }
    }
}
