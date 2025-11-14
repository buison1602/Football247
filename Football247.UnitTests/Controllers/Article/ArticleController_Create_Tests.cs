using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Football247.Controllers;
using Football247.Models.DTOs.Article;
using Football247.Services.IService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Football247.UnitTests.Controllers.Article
{
    /// <summary>
    /// Unit tests cho ArticleController.Create method
    /// Test 3 scenarios: Success, InvalidOperationException, Exception
    /// </summary>
    public class ArticleController_Create_Tests : ControllerTestBase<ArticleController>
    {
        #region Setup & Dependencies
        
        // Mock IArticleService - dependency chính của controller
        private readonly Mock<IArticleService> _mockArticleService;
        
        // Controller instance để test
        private readonly ArticleController _controller;

        public ArticleController_Create_Tests()
        {
            // Khởi tạo mock service
            _mockArticleService = new Mock<IArticleService>();
            
            // Inject dependencies vào controller
            // Logger.Object đến từ ControllerTestBase
            _controller = new ArticleController(Logger.Object, _mockArticleService.Object);
        }

        #endregion

        #region Test Scenario 1: Success (Happy Path)

        /// <summary>
        /// Test case: Tạo article thành công
        /// Expected: Trả về 201 Created với ArticleDto và Location header
        /// </summary>
        [Fact]
        public async Task Create_ValidInput_Returns201CreatedWithArticleDto()
        {
            // ============ ARRANGE ============
            // Tạo input DTO (không cần mock IFormFile vì controller không xử lý file)
            var inputDto = new AddArticleRequestDto
            {
                Title = "Test Article Title",
                Slug = "test-article-title",
                Description = "This is a test article description",
                Content = "This is the full content of the test article",
                Priority = 1,
                BgrImgs = new List<IFormFile>(), // Empty list - không cần mock
                Captions = new List<string>(),   // Empty list
                IsApproved = 0,
                CreatorId = Guid.NewGuid(),
                CategoryId = Guid.NewGuid(),
                TagIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() }
            };

            // Tạo expected output từ service
            var expectedArticleDto = new ArticleDto
            {
                Id = Guid.NewGuid(),
                Title = inputDto.Title,
                Slug = inputDto.Slug,
                Description = inputDto.Description,
                Content = inputDto.Content,
                Priority = inputDto.Priority,
                ViewCount = 0,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = null,
                IsApproved = 0,
                CreatorId = inputDto.CreatorId,
                CreatorName = "Test Creator",
                CategoryId = inputDto.CategoryId,
                CategoryName = "Test Category",
                Tags = new List<Models.DTOs.Tag.TagDto>
                {
                    new Models.DTOs.Tag.TagDto { Id = inputDto.TagIds[0], Name = "Tag1", Slug = "tag1" },
                    new Models.DTOs.Tag.TagDto { Id = inputDto.TagIds[1], Name = "Tag2", Slug = "tag2" }
                },
                Images = new List<Models.DTOs.Image.ImageDto>()
            };

            // Setup mock service: Khi gọi CreateAsync với inputDto, trả về expectedArticleDto
            _mockArticleService
                .Setup(service => service.CreateAsync(inputDto))
                .ReturnsAsync(expectedArticleDto);

            // ============ ACT ============
            // Gọi method cần test
            var result = await _controller.Create(inputDto);

            // ============ ASSERT ============
            
            // 1. Verify result type = CreatedAtActionResult
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
            
            // 2. Verify status code = 201 Created
            Assert.Equal(StatusCodes.Status201Created, createdAtActionResult.StatusCode);
            
            // 3. Verify action name = "GetArticleBySlug" (từ CreatedAtAction)
            Assert.Equal(nameof(ArticleController.GetArticleBySlug), createdAtActionResult.ActionName);
            
            // 4. Verify route values chứa slug đúng
            Assert.NotNull(createdAtActionResult.RouteValues);
            Assert.True(createdAtActionResult.RouteValues.ContainsKey("articleSlug"));
            Assert.Equal(expectedArticleDto.Slug, createdAtActionResult.RouteValues["articleSlug"]);
            
            // 5. Verify response body = expectedArticleDto
            var returnedArticleDto = Assert.IsType<ArticleDto>(createdAtActionResult.Value);
            Assert.Equal(expectedArticleDto.Id, returnedArticleDto.Id);
            Assert.Equal(expectedArticleDto.Title, returnedArticleDto.Title);
            Assert.Equal(expectedArticleDto.Slug, returnedArticleDto.Slug);
            Assert.Equal(expectedArticleDto.Description, returnedArticleDto.Description);
            Assert.Equal(expectedArticleDto.Content, returnedArticleDto.Content);
            
            // 6. Verify service được gọi ĐÚNG 1 LẦN với đúng input
            _mockArticleService.Verify(
                service => service.CreateAsync(inputDto),
                Times.Once,
                "Service CreateAsync should be called exactly once with the input DTO"
            );
            
            // 7. Verify Logger được gọi (optional - check logging behavior)
            Logger.Verify(
                logger => logger.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Start")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once,
                "Logger should log Information when method starts"
            );
        }

        #endregion

        #region Test Scenario 2: Business Logic Error (InvalidOperationException)

        /// <summary>
        /// Test case: Service throw InvalidOperationException (ví dụ: slug bị trùng)
        /// Expected: Trả về 400 BadRequest với error message
        /// </summary>
        [Fact]
        public async Task Create_ServiceThrowsInvalidOperationException_Returns400BadRequest()
        {
            // ============ ARRANGE ============
            var inputDto = new AddArticleRequestDto
            {
                Title = "Duplicate Article",
                Slug = "duplicate-slug", // Slug đã tồn tại
                Description = "Test description",
                Content = "Test content",
                Priority = 1,
                BgrImgs = new List<IFormFile>(),
                Captions = new List<string>(),
                IsApproved = 0,
                CreatorId = Guid.NewGuid(),
                CategoryId = Guid.NewGuid(),
                TagIds = new List<Guid> { Guid.NewGuid() }
            };

            var expectedErrorMessage = "An article with the slug 'duplicate-slug' already exists.";

            // Setup mock service: Throw InvalidOperationException
            _mockArticleService
                .Setup(service => service.CreateAsync(inputDto))
                .ThrowsAsync(new InvalidOperationException(expectedErrorMessage));

            // ============ ACT ============
            var result = await _controller.Create(inputDto);

            // ============ ASSERT ============
            
            // 1. Verify result type = BadRequestObjectResult
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            
            // 2. Verify status code = 400 BadRequest
            Assert.Equal(StatusCodes.Status400BadRequest, badRequestResult.StatusCode);
            
            // 3. Verify response body chứa error message
            Assert.Equal(expectedErrorMessage, badRequestResult.Value);
            
            // 4. Verify service được gọi 1 lần
            _mockArticleService.Verify(
                service => service.CreateAsync(inputDto),
                Times.Once
            );
            
            // 5. Verify Logger ghi log Warning
            Logger.Verify(
                logger => logger.Log(
                    LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Create Article failed")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once,
                "Logger should log Warning when InvalidOperationException occurs"
            );
        }

        #endregion

        #region Test Scenario 3: Unexpected Error (Exception)

        /// <summary>
        /// Test case: Service throw Exception (lỗi không mong đợi)
        /// Expected: Trả về 500 Internal Server Error với error message
        /// </summary>
        [Fact]
        public async Task Create_ServiceThrowsException_Returns500InternalServerError()
        {
            // ============ ARRANGE ============
            var inputDto = new AddArticleRequestDto
            {
                Title = "Test Article",
                Slug = "test-article",
                Description = "Test description",
                Content = "Test content",
                Priority = 1,
                BgrImgs = new List<IFormFile>(),
                Captions = new List<string>(),
                IsApproved = 0,
                CreatorId = Guid.NewGuid(),
                CategoryId = Guid.NewGuid(),
                TagIds = new List<Guid> { Guid.NewGuid() }
            };

            var expectedErrorMessage = "Database connection failed";

            // Setup mock service: Throw generic Exception (unexpected error)
            _mockArticleService
                .Setup(service => service.CreateAsync(inputDto))
                .ThrowsAsync(new Exception(expectedErrorMessage));

            // ============ ACT ============
            var result = await _controller.Create(inputDto);

            // ============ ASSERT ============
            
            // 1. Verify result type = ObjectResult (vì StatusCode(500, message))
            var objectResult = Assert.IsType<ObjectResult>(result);
            
            // 2. Verify status code = 500 Internal Server Error
            Assert.Equal(StatusCodes.Status500InternalServerError, objectResult.StatusCode);
            
            // 3. Verify response body chứa error message
            Assert.Equal(expectedErrorMessage, objectResult.Value);
            
            // 4. Verify service được gọi 1 lần
            _mockArticleService.Verify(
                service => service.CreateAsync(inputDto),
                Times.Once
            );
            
            // 5. Verify Logger ghi log Error
            Logger.Verify(
                logger => logger.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("error")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once,
                "Logger should log Error when unexpected Exception occurs"
            );
        }

        #endregion

        #region Bonus Test: Verify CreatedAtAction Route Generation

        /// <summary>
        /// Test case: Verify Location header được generate đúng format
        /// Expected: Location = /api/Article/list/{slug}
        /// </summary>
        [Fact]
        public async Task Create_ValidInput_GeneratesCorrectLocationHeader()
        {
            // ============ ARRANGE ============
            var inputDto = new AddArticleRequestDto
            {
                Title = "Location Test",
                Slug = "location-test-slug",
                Description = "Test",
                Content = "Test",
                Priority = 1,
                BgrImgs = new List<IFormFile>(),
                Captions = new List<string>(),
                IsApproved = 0,
                CreatorId = Guid.NewGuid(),
                CategoryId = Guid.NewGuid(),
                TagIds = new List<Guid> { Guid.NewGuid() }
            };

            var expectedDto = new ArticleDto
            {
                Id = Guid.NewGuid(),
                Slug = inputDto.Slug,
                Title = inputDto.Title,
                Description = inputDto.Description,
                Content = inputDto.Content,
                Priority = inputDto.Priority,
                CreatorId = inputDto.CreatorId,
                CategoryId = inputDto.CategoryId
            };

            _mockArticleService
                .Setup(s => s.CreateAsync(inputDto))
                .ReturnsAsync(expectedDto);

            // ============ ACT ============
            var result = await _controller.Create(inputDto);

            // ============ ASSERT ============
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            
            // Verify route values để tạo Location header
            Assert.NotNull(createdResult.RouteValues);
            Assert.Single(createdResult.RouteValues); // Chỉ có 1 route value
            Assert.Equal("location-test-slug", createdResult.RouteValues["articleSlug"]);
            
            // Verify action name
            Assert.Equal("GetArticleBySlug", createdResult.ActionName);
            
            // Note: Location header thực tế sẽ được ASP.NET Core generate:
            // "http://localhost/api/Article/list/location-test-slug"
        }

        #endregion
    }
}
