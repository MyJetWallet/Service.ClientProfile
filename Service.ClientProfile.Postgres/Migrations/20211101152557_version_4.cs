using Microsoft.EntityFrameworkCore.Migrations;

namespace Service.ClientProfile.Postgres.Migrations
{
    public partial class version_4 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ReferralCode",
                schema: "clientprofiles",
                table: "profiles",
                type: "character varying(128)",
                maxLength: 128,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReferralCode",
                schema: "clientprofiles",
                table: "profiles");
        }
    }
}
