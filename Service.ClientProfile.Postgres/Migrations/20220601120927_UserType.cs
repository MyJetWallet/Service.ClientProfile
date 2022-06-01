using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Service.ClientProfile.Postgres.Migrations
{
    public partial class UserType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UserType",
                schema: "clientprofiles",
                table: "profiles",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserType",
                schema: "clientprofiles",
                table: "profiles");
        }
    }
}
