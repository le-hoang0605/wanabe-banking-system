using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Authentications.Migrations
{
    /// <inheritdoc />
    public partial class InitialTransactionsDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "authentication-domain");

            migrationBuilder.CreateTable(
                name: "Credentials",
                schema: "authentication-domain",
                columns: table => new
                {
                    PartyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PasswordHashed = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Credentials", x => x.PartyId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Credentials",
                schema: "authentication-domain");
        }
    }
}
