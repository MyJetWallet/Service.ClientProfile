using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Service.ClientProfile.Postgres.Migrations
{
    public partial class ExternalId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ClientIdHash",
                schema: "clientprofiles",
                table: "profiles",
                newName: "ExternalClientId");

            migrationBuilder.CreateIndex(
                name: "IX_profiles_ExternalClientId",
                schema: "clientprofiles",
                table: "profiles",
                column: "ExternalClientId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_profiles_ExternalClientId",
                schema: "clientprofiles",
                table: "profiles");

            migrationBuilder.RenameColumn(
                name: "ExternalClientId",
                schema: "clientprofiles",
                table: "profiles",
                newName: "ClientIdHash");
        }
    }
}
