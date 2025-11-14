using Football247.Controllers;
using Football247.Models.DTOs.Article;
using Football247.Services.IService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Football247.UnitTests.Controllers.Article
{
    public class ArticleController_GetByCategory_Tests : ControllerTestBase<ArticleController>
    {
        private readonly Mock<IArticleService> _mockArticleService;
        private readonly ArticleController _controller;

        public ArticleController_GetByCategory_Tests()
        {
            _mockArticleService = new Mock<IArticleService>();
            _controller = new ArticleController(Logger.Object, _mockArticleService.Object);
        }

        [Fact]
        public async Task GetByCategory_ValidInput_Returns200OkWithArticlesDto()
        {
            // ============ ARRANGE ============
            var categorySlug = "premier-league"; 
            var page = 1;

            var expectedListArticlesDto = new List<ArticlesDto>
                {
                new ArticlesDto
                {
                    Id = Guid.NewGuid(),
                    Title = "ArticlesDto Title 1",
                    Slug = "articlesDto-title-1",
                    Description = "ArticlesDto Description 1",
                    Priority = 1,
                    BgrImg = "img1.jpg",
                    CreatedAt = DateTime.UtcNow,
                    Tags = new List<string>{ "tag1", "tag2" }
                },
                new ArticlesDto
                {
                    Id = Guid.NewGuid(),
                    Title = "ArticlesDto Title 2",
                    Slug = "articlesDto-title-2",
                    Description = "ArticlesDto Description 2",
                    Priority = 2,
                    BgrImg = "img2.jpg",
                    CreatedAt = DateTime.UtcNow,
                    Tags = new List<string>{ "tag3", "tag4" }
                }
            };

            _mockArticleService
                .Setup(service => service.GetByCategoryAsync(categorySlug, page))
                .ReturnsAsync(expectedListArticlesDto);

            // ============ ACT ============
            var result = await _controller.GetByCategory(categorySlug, page);

            // ============ ASSERT ============
            var okResult = Assert.IsType<OkObjectResult>(result);

            Assert.Equal(200, okResult.StatusCode);

            Assert.NotNull(okResult.Value);

            var returnedArticlesDto = Assert.IsType<List<ArticlesDto>>(okResult.Value);

            Assert.Equal(2, returnedArticlesDto.Count);

            Assert.Equal(expectedListArticlesDto[0].Id, returnedArticlesDto[0].Id);
            Assert.Equal(expectedListArticlesDto[0].Title, returnedArticlesDto[0].Title);
            Assert.Equal(expectedListArticlesDto[0].Slug, returnedArticlesDto[0].Slug);
            Assert.Equal(expectedListArticlesDto[0].Description, returnedArticlesDto[0].Description);
            Assert.Equal(expectedListArticlesDto[0].Priority, returnedArticlesDto[0].Priority);
            Assert.Equal(expectedListArticlesDto[0].BgrImg, returnedArticlesDto[0].BgrImg);

            Assert.Equal(expectedListArticlesDto[1].Id, returnedArticlesDto[1].Id);
            Assert.Equal(expectedListArticlesDto[1].Title, returnedArticlesDto[1].Title);

            Assert.Equal(2, returnedArticlesDto[0].Tags.Count);
            Assert.Contains("tag1", returnedArticlesDto[0].Tags);
            Assert.Contains("tag2", returnedArticlesDto[0].Tags);

            _mockArticleService.Verify(
                service => service.GetByCategoryAsync(categorySlug, page),
                Times.Once,
                "Service GetByCategoryAsync should be called exactly once with correct parameters"
            );
        }
    }
}
