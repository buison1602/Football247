using Football247.Application.Service.UserService;
using Football247.Domain.Entities;
using Football247.Domain.Entities.Stores;
using Football247.Infrastructure.Configs;
using Football247.Infrastructure.Persistence;
using Football247.Models.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Football247.Infrastructure
{
    public class Football247DbContext : BaseDbContext
    {
        public Football247DbContext(DbContextOptions<Football247DbContext> options,
            ICurrentUserService currentUserService,
            IMediator mediator) 
            : base(options, currentUserService, mediator)
        {
        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Article> Articles { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<ArticleTag> ArticleTags { get; set; }
        public DbSet<Image> Images { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<NotificationMessage> NotificationMessages { get; set; }
        public DbSet<Team> Teams { get; set; }
        public DbSet<Match> Matches { get; set; }
        public DbSet<Standing> Standings { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<ProductCategory> ProductCategories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //modelBuilder.ApplyConfigurationsFromAssembly(typeof(Football247DbContext).Assembly);
            modelBuilder.ApplyConfiguration(new MatchEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new StandingEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new ApplicationUserEntityTypeConfiguration());


            #region Seeding data

            var categories = new List<Category>
            {
                new Category()
                {
                    Id = Guid.Parse("091a6e9c-b9d6-4854-9eb5-8239f1501a9f"),
                    Name = "CHAMPIONS LEAGUE",
                    Slug = "champions-league",
                    CreatedDate = new DateTime(2025, 5, 7, 16, 38, 39, 658, DateTimeKind.Utc), // Giá trị tĩnh,
                    UpdatedDate = null
                },

                new Category()
                {
                    Id = Guid.Parse("811d5c7d-30ba-4dd9-b479-505a65a217cf"),
                    Name = "SPAIN ",
                    Slug = "spain",
                    CreatedDate = new DateTime(2025, 5, 7, 16, 38, 39, 658, DateTimeKind.Utc), // Giá trị tĩnh,
                    UpdatedDate = null
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
                    BgrImage = string.Empty,
                    ViewCount = 0,
                    CreatedDate = new DateTime(2025, 5, 7, 16, 38, 39, 658, DateTimeKind.Utc), // Giá trị tĩnh,
                    UpdatedDate = null,
                    IsApproved = false,
                    CreatedUserId = Guid.Empty,
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
                    BgrImage = string.Empty,
                    ViewCount = 0,
                    CreatedDate = new DateTime(2025, 5, 7, 16, 38, 39, 658, DateTimeKind.Utc),
                    UpdatedDate = null,
                    IsApproved = false,
                    CreatedUserId = Guid.Empty,
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
                    CreatedDate = new DateTime(2025, 5, 7, 16, 38, 39, 658, DateTimeKind.Utc) // Giá trị tĩnh,
                },
                new Tag()
                {
                    Id = Guid.Parse("85fb19e4-2bda-4d34-b92c-f116e8a3166b"),
                    Name = "Las Palmas",
                    Slug = "las-palmas-tags",
                    CreatedDate = new DateTime(2025, 5, 7, 16, 38, 39, 658, DateTimeKind.Utc) // Giá trị tĩnh,
                },
            };
            modelBuilder.Entity<Tag>().HasData(tags);



            var images = new List<Image>
            {
                new Image
                {
                    Id = Guid.Parse("8b085068-d940-458b-87f8-fae4cb132139"),
                    ArticleId = Guid.Parse("b9b0db97-6e0d-4428-b826-249aafda76e6"),
                    ImageUrl = "/images/sao-inter.jpg",
                    Caption = "Mehdi Taremi trong màu áo Porto",
                    DisplayOrder = 1,
                    ImageExtension = "jpg",
                    UploadedAt = new DateTime(2025, 5, 7, 16, 38, 39, 658, DateTimeKind.Utc)
                },
                new Image
                {
                    Id = Guid.Parse("5d968e39-4dd8-4e14-89c3-924b65d2dc05"),
                    ArticleId = Guid.Parse("b9b0db97-6e0d-4428-b826-249aafda76e6"),
                    ImageUrl = "/images/duc-huy.jpg",
                    Caption = "Phạm Đức Huy trong một pha tranh chấp",
                    DisplayOrder = 2,
                    ImageExtension = "jpg",
                    UploadedAt = new DateTime(2025, 5, 7, 16, 38, 39, 658, DateTimeKind.Utc)
                }
            };
            modelBuilder.Entity<Image>().HasData(images);



            // Configure composite primary key for ArticleTag
            modelBuilder.Entity<ArticleTag>()
                .HasKey(at => new { at.ArticleId, at.TagId });

            var articleTags = new List<ArticleTag>
            {
                new ArticleTag()
                {
                    ArticleId = Guid.Parse("b9b0db97-6e0d-4428-b826-249aafda76e6"),
                    TagId = Guid.Parse("56ba2d79-f7bb-4be0-8d7b-12be4ad20335"),
                    CreatedDate = new DateTime(2025, 5, 7, 16, 38, 39, 658, DateTimeKind.Utc) // Giá trị tĩnh,
                },
                new ArticleTag()
                {
                    ArticleId = Guid.Parse("b9b0db97-6e0d-4428-b826-249aafda76e7"),
                    TagId = Guid.Parse("85fb19e4-2bda-4d34-b92c-f116e8a3166b"),
                    CreatedDate = new DateTime(2025, 5, 7, 16, 38, 39, 658, DateTimeKind.Utc) // Giá trị tĩnh,
                }
            };

            modelBuilder.Entity<ArticleTag>().HasData(articleTags);

            #endregion

            // --- INDEX CONFIGURATION ---
            modelBuilder.Entity<Article>()
               .HasIndex(a => a.Slug)
               .HasDatabaseName("IX_Articles_Slug")
               .IsUnique();

            modelBuilder.Entity<Article>()
                .HasIndex(a => a.CreatedDate);

            modelBuilder.Entity<Article>()
                .HasIndex(a => a.ViewCount);

            modelBuilder.Entity<Article>()
                .HasIndex(a => new { a.CategoryId, a.IsApproved });

            modelBuilder.Entity<Article>()
                .HasIndex(a => new { a.CategoryId, a.IsApproved, a.CreatedDate })
                .HasDatabaseName("IX_Articles_Listing_Optimized")
                .IsDescending(false, false, true);


            modelBuilder.Entity<Category>()
                .HasIndex(c => c.Slug)
                .HasDatabaseName("IX_Categorys_Slug")
                .IsUnique();


            modelBuilder.Entity<Tag>()
                .HasIndex(t => t.Slug)
                .HasDatabaseName("IX_Tags_Slug")
                .IsUnique();


            modelBuilder.Entity<RefreshToken>()
                .HasIndex(rt => rt.Token)
                .HasDatabaseName("IX_RefreshTokens_Slug")
                .IsUnique();
        }
    }
}
