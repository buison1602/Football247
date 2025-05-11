using Football247.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace Football247.Data
{
    public class Football247DbContext : DbContext 
    {
        public Football247DbContext(DbContextOptions<Football247DbContext> dbContextOptions) : base(dbContextOptions)
        {
        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Article> Articles { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<ArticleTag> ArticleTags { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Seeding data
            // Because seeding data is a one-time operation, we can use static values for CreatedAt
            var categories = new List<Category>
            {
                new Category()
                {
                    Id = Guid.Parse("091a6e9c-b9d6-4854-9eb5-8239f1501a9f"),
                    Name = "CHAMPIONS LEAGUE",
                    Slug = "champions-league",
                    CreatedAt = new DateTime(2025, 5, 7, 16, 38, 39, 658, DateTimeKind.Utc) // Giá trị tĩnh,
                },

                new Category()
                {
                    Id = Guid.Parse("811d5c7d-30ba-4dd9-b479-505a65a217cf"),
                    Name = "SPAIN ",
                    Slug = "spain",
                    CreatedAt = new DateTime(2025, 5, 7, 16, 38, 39, 658, DateTimeKind.Utc) // Giá trị tĩnh,
                },
            };

            modelBuilder.Entity<Category>().HasData(categories);



            var aticles = new List<Article>
            {
                new Article()
                {
                    Id = Guid.Parse("b9b0db97-6e0d-4428-b826-249aafda76e6"),
                    Title = "Sao Inter Milan từng khiến tuyển thủ Việt Nam 'mất kết nối'",
                    Slug = "sao-inter-milan-tung-khien-tuyen-thu-viet-nam-mat-ket-noi",
                    Description = "Mehdi Taremi, cầu thủ từng khiến tuyển thủ Việt Nam Phạm Đức Huy bị mất trí nhớ cách đây 7 năm, có thể đi vào lịch sử bóng đá Iran tại Champions League.",
                    Content = "Inter Milan đã đánh bại Barcelona bằng tổng tỷ số 7-6 sau hai lượt trận bán kết, qua đó góp mặt ở chung kết Champions League......",
                    Priority = 1,
                    BgrImg = "hahaha.jpg",
                    ViewCount = 0,
                    CreatedAt = new DateTime(2025, 5, 7, 16, 38, 39, 658, DateTimeKind.Utc), // Giá trị tĩnh,
                    UpdatedAt = null,
                    IsApproved = 0,
                    CreatorId = null,
                    CategoryId = Guid.Parse("091a6e9c-b9d6-4854-9eb5-8239f1501a9f"),
                },

                new Article()
                {
                    Id = Guid.Parse("b9b0db97-6e0d-4428-b826-249aafda76e7"),
                    Title = "Nhận định bóng đá Las Palmas vs Vallecano, 02h00 ngày 10/5: Sức mạnh kẻ khốn cùng\r\n",
                    Slug = "nhan-dinh-bong-da-las-palmas-vs-vallecano-02h00-ngay-10-5-suc-manh-ke-khon-cung",
                    Description = "Nhận định bóng đá trận Las Palmas vs Vallecano diễn ra vào lúc 02h00 ngày 10/5 trong khuôn khổ vòng 35 La Liga 2024/25. Bongdaplus phân tích thông tin lực lượng, đội hình dự kiến, soi kèo nhà cái, dự đoán tỉ số.",
                    Content = "Las Palmas và Rayo Vallecano lúc này đang ở vị thế rất khác nhau. Nếu như Vallecano đang nằm trong nhóm được dự cúp châu Âu mùa sau (đứng thứ 8) thì Las Palmas lại phải đối mặt với nguy cơ xuống hạng.",
                    Priority = 1,
                    BgrImg = "hahaha.jpg",
                    ViewCount = 0,
                    CreatedAt = new DateTime(2025, 5, 7, 16, 38, 39, 658, DateTimeKind.Utc),
                    UpdatedAt = null,
                    IsApproved = 0,
                    CreatorId = null,
                    CategoryId = Guid.Parse("811d5c7d-30ba-4dd9-b479-505a65a217cf"),
                }
            };

            modelBuilder.Entity<Article>().HasData(aticles);



            var tags = new List<Tag>
            {
               new Tag()
                {
                    Id = Guid.Parse("56ba2d79-f7bb-4be0-8d7b-12be4ad20335"),
                    Name = "MU",
                    Slug = "mu-tags",
                    CreatedAt = new DateTime(2025, 5, 7, 16, 38, 39, 658, DateTimeKind.Utc) // Giá trị tĩnh,
                },
                new Tag()
                {
                    Id = Guid.Parse("85fb19e4-2bda-4d34-b92c-f116e8a3166b"),
                    Name = "Las Palmas",
                    Slug = "las-palmas-tags",
                    CreatedAt = new DateTime(2025, 5, 7, 16, 38, 39, 658, DateTimeKind.Utc) // Giá trị tĩnh,
                },
            };

            modelBuilder.Entity<Tag>().HasData(tags);




            // Configure composite primary key for ArticleTag
            modelBuilder.Entity<ArticleTag>()
                .HasKey(at => new { at.ArticleId, at.TagId });

            var articleTags = new List<ArticleTag>
            {
                new ArticleTag()
                {
                    ArticleId = Guid.Parse("b9b0db97-6e0d-4428-b826-249aafda76e6"),
                    TagId = Guid.Parse("56ba2d79-f7bb-4be0-8d7b-12be4ad20335"),
                    CreatedAt = new DateTime(2025, 5, 7, 16, 38, 39, 658, DateTimeKind.Utc) // Giá trị tĩnh,
                },
                new ArticleTag()
                {
                    ArticleId = Guid.Parse("b9b0db97-6e0d-4428-b826-249aafda76e7"),
                    TagId = Guid.Parse("85fb19e4-2bda-4d34-b92c-f116e8a3166b"),
                    CreatedAt = new DateTime(2025, 5, 7, 16, 38, 39, 658, DateTimeKind.Utc) // Giá trị tĩnh,
                }
            };

            modelBuilder.Entity<ArticleTag>().HasData(articleTags);
        }
    }
}
