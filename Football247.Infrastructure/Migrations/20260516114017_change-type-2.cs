using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Football247.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class changetype2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "IsApproved",
                table: "Articles",
                type: "int",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.UpdateData(
                table: "Articles",
                keyColumn: "Id",
                keyValue: new Guid("b9b0db97-6e0d-4428-b826-249aafda76e6"),
                column: "IsApproved",
                value: 1);

            migrationBuilder.UpdateData(
                table: "Articles",
                keyColumn: "Id",
                keyValue: new Guid("b9b0db97-6e0d-4428-b826-249aafda76e7"),
                column: "IsApproved",
                value: 1);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "IsApproved",
                table: "Articles",
                type: "bit",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.UpdateData(
                table: "Articles",
                keyColumn: "Id",
                keyValue: new Guid("b9b0db97-6e0d-4428-b826-249aafda76e6"),
                column: "IsApproved",
                value: false);

            migrationBuilder.UpdateData(
                table: "Articles",
                keyColumn: "Id",
                keyValue: new Guid("b9b0db97-6e0d-4428-b826-249aafda76e7"),
                column: "IsApproved",
                value: false);
        }
    }
}
