using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Football247.Migrations
{
    /// <inheritdoc />
    public partial class ChangeArtilceclassaddImageclassandseedingforImageclass2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BgrImg",
                table: "Articles");

            migrationBuilder.AlterColumn<bool>(
                name: "IsApproved",
                table: "Articles",
                type: "bit",
                nullable: false,
                oldClrType: typeof(byte),
                oldType: "tinyint");

            migrationBuilder.CreateTable(
                name: "Image",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Caption = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    UploadedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ArticleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Image", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Image_Articles_ArticleId",
                        column: x => x.ArticleId,
                        principalTable: "Articles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

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

            migrationBuilder.InsertData(
                table: "Image",
                columns: new[] { "Id", "ArticleId", "Caption", "DisplayOrder", "ImageUrl", "UploadedAt" },
                values: new object[,]
                {
                    { new Guid("5d968e39-4dd8-4e14-89c3-924b65d2dc05"), new Guid("8172ac98-d6fb-4bc3-e672-08dd9a226bbc"), "Phạm Đức Huy trong một pha tranh chấp", 2, "/images/duc-huy.jpg", new DateTime(2025, 5, 7, 16, 38, 39, 658, DateTimeKind.Utc) },
                    { new Guid("8b085068-d940-458b-87f8-fae4cb132139"), new Guid("8172ac98-d6fb-4bc3-e672-08dd9a226bbc"), "Mehdi Taremi trong màu áo Porto", 1, "/images/sao-inter.jpg", new DateTime(2025, 5, 7, 16, 38, 39, 658, DateTimeKind.Utc) }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Image_ArticleId",
                table: "Image",
                column: "ArticleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Image");

            migrationBuilder.AlterColumn<byte>(
                name: "IsApproved",
                table: "Articles",
                type: "tinyint",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AddColumn<string>(
                name: "BgrImg",
                table: "Articles",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "Articles",
                keyColumn: "Id",
                keyValue: new Guid("b9b0db97-6e0d-4428-b826-249aafda76e6"),
                columns: new[] { "BgrImg", "IsApproved" },
                values: new object[] { "[\"HAHAHA_1.png\",\"HAHAHA_2.png\"]", (byte)0 });

            migrationBuilder.UpdateData(
                table: "Articles",
                keyColumn: "Id",
                keyValue: new Guid("b9b0db97-6e0d-4428-b826-249aafda76e7"),
                columns: new[] { "BgrImg", "IsApproved" },
                values: new object[] { "[\"HAHAHA_1.png\",\"HAHAHA_2.png\"]", (byte)0 });
        }
    }
}
