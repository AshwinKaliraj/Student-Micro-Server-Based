using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AuthService.Migrations
{
    /// <inheritdoc />
    public partial class RemoveDesignationColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Designation",
                table: "Users");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2025, 10, 28, 7, 14, 38, 188, DateTimeKind.Utc).AddTicks(6075), "$2a$11$VplGERJPMsWBvrn825fNl.YF6PyQuksLS74.FdwM2UyhIys9gloSm" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2025, 10, 28, 7, 14, 38, 397, DateTimeKind.Utc).AddTicks(4319), "$2a$11$oSE2mQ7XVUeVIPnkpXf.uucJ97UhQI/2LFpA3IbdnZDfB0oseHJxy" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Designation",
                table: "Users",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "Designation", "PasswordHash" },
                values: new object[] { new DateTime(2025, 10, 27, 14, 29, 6, 764, DateTimeKind.Utc).AddTicks(4197), "Teacher", "$2a$11$SrTCqdHa.LJOiU3D/7AsjuOaduUKamksp19.pqDBGrdqvTY9ZVqPC" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "Designation", "PasswordHash" },
                values: new object[] { new DateTime(2025, 10, 27, 14, 29, 7, 11, DateTimeKind.Utc).AddTicks(1903), "Student", "$2a$11$o.5w8lxXESNRF2/rs7mlUOF.9oeWXeytwD7jWBERLhrZ8Pq/YWAj6" });
        }
    }
}
