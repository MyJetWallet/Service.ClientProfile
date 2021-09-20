using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Service.ClientProfile.Postgres.Migrations
{
    public partial class InitialCreation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "clientprofiles");

            migrationBuilder.CreateTable(
                name: "profiles",
                schema: "clientprofiles",
                columns: table => new
                {
                    ClientId = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    Status2FA = table.Column<int>(type: "integer", nullable: false, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_profiles", x => x.ClientId);
                });

            migrationBuilder.CreateTable(
                name: "blockers",
                schema: "clientprofiles",
                columns: table => new
                {
                    BlockerId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    BlockedOperationType = table.Column<int>(type: "integer", nullable: false),
                    ExpiryTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Reason = table.Column<string>(type: "text", nullable: true),
                    ClientProfileClientId = table.Column<string>(type: "character varying(128)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_blockers", x => x.BlockerId);
                    table.ForeignKey(
                        name: "FK_blockers_profiles_ClientProfileClientId",
                        column: x => x.ClientProfileClientId,
                        principalSchema: "clientprofiles",
                        principalTable: "profiles",
                        principalColumn: "ClientId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_blockers_ClientProfileClientId",
                schema: "clientprofiles",
                table: "blockers",
                column: "ClientProfileClientId");

            migrationBuilder.CreateIndex(
                name: "IX_profiles_ClientId",
                schema: "clientprofiles",
                table: "profiles",
                column: "ClientId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "blockers",
                schema: "clientprofiles");

            migrationBuilder.DropTable(
                name: "profiles",
                schema: "clientprofiles");
        }
    }
}
