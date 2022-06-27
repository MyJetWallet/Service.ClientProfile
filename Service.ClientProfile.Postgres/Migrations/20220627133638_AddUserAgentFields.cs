using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Service.ClientProfile.Postgres.Migrations
{
    public partial class AddUserAgentFields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DeviceOperationSystem",
                schema: "clientprofiles",
                table: "profiles",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsMobile",
                schema: "clientprofiles",
                table: "profiles",
                type: "boolean",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeviceOperationSystem",
                schema: "clientprofiles",
                table: "profiles");

            migrationBuilder.DropColumn(
                name: "IsMobile",
                schema: "clientprofiles",
                table: "profiles");
        }
    }
}
