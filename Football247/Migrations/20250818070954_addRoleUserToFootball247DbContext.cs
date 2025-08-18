using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Football247.Migrations
{
    /// <inheritdoc />
    public partial class addRoleUserToFootball247DbContext : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "79620ca9-0980-410b-96ad-04e05a20e80e", "79620ca9-0980-410b-96ad-04e05a20e80e", "User", "USER" },
                    { "81470c42-0690-41b4-8b44-d6e388086964", "81470c42-0690-41b4-8b44-d6e388086964", "Admin", "ADMIN" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "79620ca9-0980-410b-96ad-04e05a20e80e");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "81470c42-0690-41b4-8b44-d6e388086964");
        }
    }
}
