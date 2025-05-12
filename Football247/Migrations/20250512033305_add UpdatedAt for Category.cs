using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Football247.Migrations
{
    /// <inheritdoc />
    public partial class addUpdatedAtforCategory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Categories",
                type: "datetime2",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("091a6e9c-b9d6-4854-9eb5-8239f1501a9f"),
                column: "UpdatedAt",
                value: null);

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("811d5c7d-30ba-4dd9-b479-505a65a217cf"),
                column: "UpdatedAt",
                value: null);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Categories");
        }
    }
}
