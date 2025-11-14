using Football247.Controllers;
using Football247.Models.DTOs.Article;
using Football247.Services.IService;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Football247.UnitTests.Controllers.Article
{
    public class ArticleController_Delete_Tests : ControllerTestBase<ArticleController>
    {
        private readonly Mock<IArticleService> _mockArticleService;
        private readonly ArticleController _controller; 

        public ArticleController_Delete_Tests()
        {
            _mockArticleService = new Mock<IArticleService>();
            _controller = new ArticleController(Logger.Object, _mockArticleService.Object);
        }


        [Fact]
        public async Task Delete_ValidInput_Returns200OkUpdatedWithArticlleDto()
        {
            // ============ ARRANGE ============
            var articleId = Guid.NewGuid();

            var expectedArticleDto = new ArticleDto
            {
                Id = Guid.NewGuid(),
                Title = "Deleted Title",
                Slug = "deleted-title",
                Description = "Deleted Description",
                Content = "Deleted Content",
                Priority = 2,
                BgrImg = new List<string> { "img1.jpg", "img2.jpg" },
                IsApproved = 1,
                CreatorId = Guid.NewGuid(),
                CategoryId = Guid.NewGuid(),
                Tags = new List<Models.DTOs.Tag.TagDto>
                {
                    new Models.DTOs.Tag.TagDto { Id = Guid.NewGuid(), Name = "Tag1", Slug = "tag1" },
                    new Models.DTOs.Tag.TagDto { Id = Guid.NewGuid(), Name = "Tag2", Slug = "tag2" }
                }
            };

            // Setup mock service 
            _mockArticleService
                .Setup(service => service.DeleteAsync(articleId))
                .ReturnsAsync(expectedArticleDto);

            // ============ ACT ============
            var result = await _controller.Delete(articleId);

            // ============ ASSERT ============
            var okResult = Assert.IsType<OkObjectResult>(result);

            Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);

            Assert.NotNull(okResult.Value);

            var returnedArticleDto = Assert.IsType<ArticleDto>(okResult.Value);
            Assert.Equal(expectedArticleDto.Id, returnedArticleDto.Id);
            Assert.Equal(expectedArticleDto.Title, returnedArticleDto.Title);
            Assert.Equal(expectedArticleDto.Slug, returnedArticleDto.Slug);
            Assert.Equal(expectedArticleDto.Description, returnedArticleDto.Description);
            Assert.Equal(expectedArticleDto.Content, returnedArticleDto.Content);
            Assert.Equal(expectedArticleDto.Priority, returnedArticleDto.Priority);
            Assert.Equal(expectedArticleDto.BgrImg, returnedArticleDto.BgrImg);
            Assert.Equal(expectedArticleDto.IsApproved, returnedArticleDto.IsApproved);
            Assert.Equal(expectedArticleDto.CreatorId, returnedArticleDto.CreatorId);
            Assert.Equal(expectedArticleDto.CategoryId, returnedArticleDto.CategoryId);
            Assert.Equal(expectedArticleDto.Tags.Count, returnedArticleDto.Tags.Count);

            for (int i = 0; i < expectedArticleDto.Tags.Count; i++)
            {
                Assert.Equal(expectedArticleDto.Tags[i].Id, returnedArticleDto.Tags[i].Id);
                Assert.Equal(expectedArticleDto.Tags[i].Name, returnedArticleDto.Tags[i].Name);
                Assert.Equal(expectedArticleDto.Tags[i].Slug, returnedArticleDto.Tags[i].Slug);
            }

            _mockArticleService.Verify(
                service => service.DeleteAsync(articleId),
                Times.Once
                );
        }


        [Fact]
        public async Task Delete_ServiceThrowsInvalidOperationException_Returns400BadRequest()
        {
            // ============ ARRANGE ============

            var articleId = Guid.NewGuid();

            var expectedErrorMessage = "An article with the id 'id-la-12345' already exists.";

            // Setup mock service
            _mockArticleService
                .Setup(service => service.DeleteAsync(articleId))
                .ThrowsAsync(new InvalidOperationException(expectedErrorMessage));

            // ============ ACT ============
            var result = await _controller.Delete(articleId);

            // ============ ASSERT ============
            var badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(result);

            Assert.Equal(StatusCodes.Status400BadRequest, badRequestObjectResult.StatusCode);

            Assert.Equal(expectedErrorMessage, badRequestObjectResult.Value);

            _mockArticleService.Verify(
                service => service.DeleteAsync(articleId),
                Times.Once
                );
        }


        [Fact]
        public async Task Delete_ServiceThrowsException_Returns500InternalServerError()
        {
            // ============ ARRANGE ============

            var articleId = Guid.NewGuid();

            var inputDto = new UpdateArticleRequestDto
            {
                Title = "Deleted Title",
                Slug = "deleted-title",
                Description = "Deleted Description",
                Content = "Deleted Content",
                Priority = 2,
                BgrImg = new List<string> { "img1.jpg", "img2.jpg" },
                IsApproved = 1,
                CreatorId = Guid.NewGuid(),
                CategoryId = Guid.NewGuid(),
                TagIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() }
            };

            var expectedErrorMessage = "Database connection failed";

            // Setup mock service
            _mockArticleService
                .Setup(service => service.DeleteAsync(articleId))
                .ThrowsAsync(new Exception(expectedErrorMessage));

            // ============ ACT ============
            var result = await _controller.Delete(articleId);

            // ============ ASSERT ============
            var objectResult = Assert.IsType<ObjectResult>(result);

            Assert.Equal(StatusCodes.Status500InternalServerError, objectResult.StatusCode);

            Assert.Equal(expectedErrorMessage, objectResult.Value);

            _mockArticleService.Verify(
                service => service.DeleteAsync(articleId),
                Times.Once
                );
        }
    }
}
