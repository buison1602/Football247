using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Football247.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class change_article_entity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Articles",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "Articles",
                keyColumn: "Id",
                keyValue: new Guid("b9b0db97-6e0d-4428-b826-249aafda76e6"),
                column: "Status",
                value: 0);

            migrationBuilder.UpdateData(
                table: "Articles",
                keyColumn: "Id",
                keyValue: new Guid("b9b0db97-6e0d-4428-b826-249aafda76e7"),
                column: "Status",
                value: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Articles");
        }
    }
}
