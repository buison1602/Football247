using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Football247.Migrations
{
    /// <inheritdoc />
    public partial class addIndexForEachTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Articles_CategoryId",
                table: "Articles");

            migrationBuilder.AlterColumn<string>(
                name: "Slug",
                table: "Tags",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Token",
                table: "RefreshTokens",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Slug",
                table: "Categories",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_Tags_Slug",
                table: "Tags",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_Slug",
                table: "RefreshTokens",
                column: "Token",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Categorys_Slug",
                table: "Categories",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Articles_CategoryId_IsApproved",
                table: "Articles",
                columns: new[] { "CategoryId", "IsApproved" });

            migrationBuilder.CreateIndex(
                name: "IX_Articles_CreatedAt",
                table: "Articles",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Articles_ViewCount",
                table: "Articles",
                column: "ViewCount");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Tags_Slug",
                table: "Tags");

            migrationBuilder.DropIndex(
                name: "IX_RefreshTokens_Slug",
                table: "RefreshTokens");

            migrationBuilder.DropIndex(
                name: "IX_Categorys_Slug",
                table: "Categories");

            migrationBuilder.DropIndex(
                name: "IX_Articles_CategoryId_IsApproved",
                table: "Articles");

            migrationBuilder.DropIndex(
                name: "IX_Articles_CreatedAt",
                table: "Articles");

            migrationBuilder.DropIndex(
                name: "IX_Articles_ViewCount",
                table: "Articles");

            migrationBuilder.AlterColumn<string>(
                name: "Slug",
                table: "Tags",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "Token",
                table: "RefreshTokens",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "Slug",
                table: "Categories",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.CreateIndex(
                name: "IX_Articles_CategoryId",
                table: "Articles",
                column: "CategoryId");
        }
    }
}
