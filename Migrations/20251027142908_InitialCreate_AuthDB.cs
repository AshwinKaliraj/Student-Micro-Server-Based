using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AuthService.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate_AuthDB : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2025, 10, 27, 14, 29, 6, 764, DateTimeKind.Utc).AddTicks(4197), "$2a$11$SrTCqdHa.LJOiU3D/7AsjuOaduUKamksp19.pqDBGrdqvTY9ZVqPC" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2025, 10, 27, 14, 29, 7, 11, DateTimeKind.Utc).AddTicks(1903), "$2a$11$o.5w8lxXESNRF2/rs7mlUOF.9oeWXeytwD7jWBERLhrZ8Pq/YWAj6" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2025, 10, 27, 8, 54, 41, 614, DateTimeKind.Utc).AddTicks(7674), "$2a$11$aa4rH3cbQQ8kBvFOii.xrejT0.qFS2LP4UTWY3CoNaHnJG0DOA9KG" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2025, 10, 27, 8, 54, 41, 878, DateTimeKind.Utc).AddTicks(3991), "$2a$11$IM5z3r30YdCLwLZFeuLnee9zdXqi4HQRGDTRwsjNjtansbT37qaLq" });
        }
    }
}
