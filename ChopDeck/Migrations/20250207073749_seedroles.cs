using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ChopDeck.Migrations
{
    /// <inheritdoc />
    public partial class seedroles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "5ada4059-2a36-42b3-84a5-5c0526c46c25", null, "Driver", "DRIVER" },
                    { "a687a1f9-f603-4d95-bdd6-5fa7b59b0621", null, "Customer", "CUSTOMER" },
                    { "f67e1e35-26cf-440d-85d6-bf7bd0bbcd7e", null, "Restaurant", "RESTAURANT" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "5ada4059-2a36-42b3-84a5-5c0526c46c25");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "a687a1f9-f603-4d95-bdd6-5fa7b59b0621");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "f67e1e35-26cf-440d-85d6-bf7bd0bbcd7e");
        }
    }
}
