using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Service.ClientProfile.Postgres.Migrations
{
    public partial class InternalSimpleEmail : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "InternalSimpleEmail",
                schema: "clientprofiles",
                table: "profiles",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InternalSimpleEmail",
                schema: "clientprofiles",
                table: "profiles");
        }
    }
}
