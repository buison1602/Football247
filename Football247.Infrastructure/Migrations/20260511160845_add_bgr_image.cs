using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Football247.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class add_bgr_image : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BgrImage",
                table: "Articles",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "Articles",
                keyColumn: "Id",
                keyValue: new Guid("b9b0db97-6e0d-4428-b826-249aafda76e6"),
                column: "BgrImage",
                value: "");

            migrationBuilder.UpdateData(
                table: "Articles",
                keyColumn: "Id",
                keyValue: new Guid("b9b0db97-6e0d-4428-b826-249aafda76e7"),
                column: "BgrImage",
                value: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BgrImage",
                table: "Articles");
        }
    }
}
