using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Football247.Migrations
{
    /// <inheritdoc />
    public partial class AddupdateAtforTags : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Tags",
                type: "datetime2",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Tags",
                keyColumn: "Id",
                keyValue: new Guid("56ba2d79-f7bb-4be0-8d7b-12be4ad20335"),
                column: "UpdatedAt",
                value: null);

            migrationBuilder.UpdateData(
                table: "Tags",
                keyColumn: "Id",
                keyValue: new Guid("85fb19e4-2bda-4d34-b92c-f116e8a3166b"),
                column: "UpdatedAt",
                value: null);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Tags");
        }
    }
}
