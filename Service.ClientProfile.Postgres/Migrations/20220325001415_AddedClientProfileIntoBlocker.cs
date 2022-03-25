using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Service.ClientProfile.Postgres.Migrations
{
    public partial class AddedClientProfileIntoBlocker : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_blockers_profiles_ClientProfileClientId",
                schema: "clientprofiles",
                table: "blockers");

            migrationBuilder.RenameColumn(
                name: "ClientProfileClientId",
                schema: "clientprofiles",
                table: "blockers",
                newName: "ProfileClientId");

            migrationBuilder.RenameIndex(
                name: "IX_blockers_ClientProfileClientId",
                schema: "clientprofiles",
                table: "blockers",
                newName: "IX_blockers_ProfileClientId");

            migrationBuilder.AddForeignKey(
                name: "FK_blockers_profiles_ProfileClientId",
                schema: "clientprofiles",
                table: "blockers",
                column: "ProfileClientId",
                principalSchema: "clientprofiles",
                principalTable: "profiles",
                principalColumn: "ClientId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_blockers_profiles_ProfileClientId",
                schema: "clientprofiles",
                table: "blockers");

            migrationBuilder.RenameColumn(
                name: "ProfileClientId",
                schema: "clientprofiles",
                table: "blockers",
                newName: "ClientProfileClientId");

            migrationBuilder.RenameIndex(
                name: "IX_blockers_ProfileClientId",
                schema: "clientprofiles",
                table: "blockers",
                newName: "IX_blockers_ClientProfileClientId");

            migrationBuilder.AddForeignKey(
                name: "FK_blockers_profiles_ClientProfileClientId",
                schema: "clientprofiles",
                table: "blockers",
                column: "ClientProfileClientId",
                principalSchema: "clientprofiles",
                principalTable: "profiles",
                principalColumn: "ClientId");
        }
    }
}
