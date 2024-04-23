using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Migrations.Migrations.Fido
{
    /// <inheritdoc />
    public partial class Fido : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FidoKeys",
                columns: table => new
                {
                    CredentialId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    UserHandle = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DisplayFriendlyName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AttestationType = table.Column<int>(type: "int", nullable: false),
                    AuthenticatorId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AuthenticatorIdType = table.Column<int>(type: "int", nullable: true),
                    Counter = table.Column<int>(type: "int", nullable: false),
                    KeyType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Algorithm = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CredentialAsJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastUsed = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FidoKeys", x => x.CredentialId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FidoKeys_UserId",
                table: "FidoKeys",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FidoKeys");
        }
    }
}
