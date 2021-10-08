using Microsoft.EntityFrameworkCore.Migrations;

namespace Service.ClientProfile.Postgres.Migrations
{
    public partial class version_3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "KYCPassed",
                schema: "clientprofiles",
                table: "profiles",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "KYCPassed",
                schema: "clientprofiles",
                table: "profiles");
        }
    }
}
