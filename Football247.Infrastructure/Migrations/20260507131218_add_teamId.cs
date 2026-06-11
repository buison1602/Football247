using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Football247.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class add_teamId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Slug",
                table: "Teams",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "TeamId",
                table: "Articles",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Articles",
                keyColumn: "Id",
                keyValue: new Guid("b9b0db97-6e0d-4428-b826-249aafda76e6"),
                column: "TeamId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Articles",
                keyColumn: "Id",
                keyValue: new Guid("b9b0db97-6e0d-4428-b826-249aafda76e7"),
                column: "TeamId",
                value: null);

            migrationBuilder.CreateIndex(
                name: "IX_Articles_TeamId",
                table: "Articles",
                column: "TeamId");

            migrationBuilder.AddForeignKey(
                name: "FK_Articles_Teams_TeamId",
                table: "Articles",
                column: "TeamId",
                principalTable: "Teams",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Articles_Teams_TeamId",
                table: "Articles");

            migrationBuilder.DropIndex(
                name: "IX_Articles_TeamId",
                table: "Articles");

            migrationBuilder.DropColumn(
                name: "Slug",
                table: "Teams");

            migrationBuilder.DropColumn(
                name: "TeamId",
                table: "Articles");
        }
    }
}
