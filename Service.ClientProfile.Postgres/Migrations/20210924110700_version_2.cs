using Microsoft.EntityFrameworkCore.Migrations;

namespace Service.ClientProfile.Postgres.Migrations
{
    public partial class version_2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "EmailConfirmed",
                schema: "clientprofiles",
                table: "profiles",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "PhoneConfirmed",
                schema: "clientprofiles",
                table: "profiles",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EmailConfirmed",
                schema: "clientprofiles",
                table: "profiles");

            migrationBuilder.DropColumn(
                name: "PhoneConfirmed",
                schema: "clientprofiles",
                table: "profiles");
        }
    }
}
