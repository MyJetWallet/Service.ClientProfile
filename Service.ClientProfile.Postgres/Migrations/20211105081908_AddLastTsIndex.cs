using Microsoft.EntityFrameworkCore.Migrations;

namespace Service.ClientProfile.Postgres.Migrations
{
    public partial class AddLastTsIndex : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_profiles_LastTs",
                schema: "clientprofiles",
                table: "profiles",
                column: "LastTs");

            migrationBuilder.CreateIndex(
                name: "IX_blockers_LastTs",
                schema: "clientprofiles",
                table: "blockers",
                column: "LastTs");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_profiles_LastTs",
                schema: "clientprofiles",
                table: "profiles");

            migrationBuilder.DropIndex(
                name: "IX_blockers_LastTs",
                schema: "clientprofiles",
                table: "blockers");
        }
    }
}
