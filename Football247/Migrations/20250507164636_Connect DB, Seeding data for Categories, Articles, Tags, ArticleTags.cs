using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Football247.Migrations
{
    /// <inheritdoc />
    public partial class ConnectDBSeedingdataforCategoriesArticlesTagsArticleTags : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Articles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Slug = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Priority = table.Column<int>(type: "int", nullable: false),
                    BgrImg = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ViewCount = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsApproved = table.Column<byte>(type: "tinyint", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Articles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Slug = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tags",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Slug = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tags", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ArticleTags",
                columns: table => new
                {
                    ArticleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TagId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArticleTags", x => new { x.ArticleId, x.TagId });
                    table.ForeignKey(
                        name: "FK_ArticleTags_Articles_ArticleId",
                        column: x => x.ArticleId,
                        principalTable: "Articles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ArticleTags_Tags_TagId",
                        column: x => x.TagId,
                        principalTable: "Tags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Articles",
                columns: new[] { "Id", "BgrImg", "CategoryId", "Content", "CreatedAt", "CreatorId", "Description", "IsApproved", "Priority", "Slug", "Title", "UpdatedAt", "ViewCount" },
                values: new object[,]
                {
                    { new Guid("b9b0db97-6e0d-4428-b826-249aafda76e6"), "hahaha.jpg", new Guid("091a6e9c-b9d6-4854-9eb5-8239f1501a9f"), "Inter Milan đã đánh bại Barcelona bằng tổng tỷ số 7-6 sau hai lượt trận bán kết, qua đó góp mặt ở chung kết Champions League......", new DateTime(2025, 5, 7, 16, 38, 39, 658, DateTimeKind.Utc), null, "Mehdi Taremi, cầu thủ từng khiến tuyển thủ Việt Nam Phạm Đức Huy bị mất trí nhớ cách đây 7 năm, có thể đi vào lịch sử bóng đá Iran tại Champions League.", (byte)0, 1, "sao-inter-milan-tung-khien-tuyen-thu-viet-nam-mat-ket-noi", "Sao Inter Milan từng khiến tuyển thủ Việt Nam 'mất kết nối'", null, 0 },
                    { new Guid("b9b0db97-6e0d-4428-b826-249aafda76e7"), "hahaha.jpg", new Guid("811d5c7d-30ba-4dd9-b479-505a65a217cf"), "Las Palmas và Rayo Vallecano lúc này đang ở vị thế rất khác nhau. Nếu như Vallecano đang nằm trong nhóm được dự cúp châu Âu mùa sau (đứng thứ 8) thì Las Palmas lại phải đối mặt với nguy cơ xuống hạng.", new DateTime(2025, 5, 7, 16, 38, 39, 658, DateTimeKind.Utc), null, "Nhận định bóng đá trận Las Palmas vs Vallecano diễn ra vào lúc 02h00 ngày 10/5 trong khuôn khổ vòng 35 La Liga 2024/25. Bongdaplus phân tích thông tin lực lượng, đội hình dự kiến, soi kèo nhà cái, dự đoán tỉ số.", (byte)0, 1, "nhan-dinh-bong-da-las-palmas-vs-vallecano-02h00-ngay-10-5-suc-manh-ke-khon-cung", "Nhận định bóng đá Las Palmas vs Vallecano, 02h00 ngày 10/5: Sức mạnh kẻ khốn cùng\r\n", null, 0 }
                });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "CreatedAt", "Name", "Slug" },
                values: new object[,]
                {
                    { new Guid("091a6e9c-b9d6-4854-9eb5-8239f1501a9f"), new DateTime(2025, 5, 7, 16, 38, 39, 658, DateTimeKind.Utc), "CHAMPIONS LEAGUE", "champions-league" },
                    { new Guid("811d5c7d-30ba-4dd9-b479-505a65a217cf"), new DateTime(2025, 5, 7, 16, 38, 39, 658, DateTimeKind.Utc), "SPAIN ", "spain" }
                });

            migrationBuilder.InsertData(
                table: "Tags",
                columns: new[] { "Id", "CreatedAt", "Name", "Slug" },
                values: new object[,]
                {
                    { new Guid("56ba2d79-f7bb-4be0-8d7b-12be4ad20335"), new DateTime(2025, 5, 7, 16, 38, 39, 658, DateTimeKind.Utc), "MU", "mu-tags" },
                    { new Guid("85fb19e4-2bda-4d34-b92c-f116e8a3166b"), new DateTime(2025, 5, 7, 16, 38, 39, 658, DateTimeKind.Utc), "Las Palmas", "las-palmas-tags" }
                });

            migrationBuilder.InsertData(
                table: "ArticleTags",
                columns: new[] { "ArticleId", "TagId", "CreatedAt" },
                values: new object[,]
                {
                    { new Guid("b9b0db97-6e0d-4428-b826-249aafda76e6"), new Guid("56ba2d79-f7bb-4be0-8d7b-12be4ad20335"), new DateTime(2025, 5, 7, 16, 38, 39, 658, DateTimeKind.Utc) },
                    { new Guid("b9b0db97-6e0d-4428-b826-249aafda76e7"), new Guid("85fb19e4-2bda-4d34-b92c-f116e8a3166b"), new DateTime(2025, 5, 7, 16, 38, 39, 658, DateTimeKind.Utc) }
                });

            migrationBuilder.CreateIndex(
                name: "IX_ArticleTags_TagId",
                table: "ArticleTags",
                column: "TagId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ArticleTags");

            migrationBuilder.DropTable(
                name: "Categories");

            migrationBuilder.DropTable(
                name: "Articles");

            migrationBuilder.DropTable(
                name: "Tags");
        }
    }
}
