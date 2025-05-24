using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Football247.Migrations
{
    /// <inheritdoc />
    public partial class ChangeArticlesTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Articles",
                keyColumn: "Id",
                keyValue: new Guid("b9b0db97-6e0d-4428-b826-249aafda76e6"),
                column: "BgrImg",
                value: "[\"HAHAHA_1.png\",\"HAHAHA_2.png\"]");

            migrationBuilder.UpdateData(
                table: "Articles",
                keyColumn: "Id",
                keyValue: new Guid("b9b0db97-6e0d-4428-b826-249aafda76e7"),
                column: "BgrImg",
                value: "[\"HAHAHA_1.png\",\"HAHAHA_2.png\"]");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Articles",
                keyColumn: "Id",
                keyValue: new Guid("b9b0db97-6e0d-4428-b826-249aafda76e6"),
                column: "BgrImg",
                value: "hahaha.jpg");

            migrationBuilder.UpdateData(
                table: "Articles",
                keyColumn: "Id",
                keyValue: new Guid("b9b0db97-6e0d-4428-b826-249aafda76e7"),
                column: "BgrImg",
                value: "hahaha.jpg");
        }
    }
}
