using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Football247.Application.Command.ArticleCmd;
using Football247.Controllers;
using Football247.Domain.Models.EntityModels.DTOs.Article;
using Football247.Shared.Enum.ErrorCode;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Shared.Response;
using Xunit;

namespace Football247.UnitTests.Controllers.Article
{
    /// <summary>
    /// Unit test cho ArticleController.Create.
    ///
    /// Khác với bản cũ (mock IArticleService trực tiếp), controller hiện tại chỉ
    /// phụ thuộc IMediator, nên ta mock IMediator.Send(CreateArticleCommand) và
    /// verify controller trả về đúng IActionResult mà GetActionResult() tạo ra
    /// từ MethodResult<ArticleDto>.
    ///
    /// Toàn bộ logic validate/slug trùng/lỗi hệ thống nằm trong CreateArticleHandler,
    /// KHÔNG nằm trong controller -> test controller ở đây chỉ verify controller
    /// "chuyển tiếp" đúng, không test lại business logic của handler (việc đó nên
    /// có 1 file test riêng: CreateArticleHandler_Tests.cs).
    /// </summary>
    public class ArticleController_Create_Tests
    {
        private readonly Mock<IMediator> _mockMediator;
        private readonly ArticleController _controller;

        public ArticleController_Create_Tests()
        {
            _mockMediator = new Mock<IMediator>();
            _controller = new ArticleController(_mockMediator.Object);
        }

        private static CreateArticleCommand BuildValidCommand()
        {
            return new CreateArticleCommand
            {
                Title = "Test Article Title",
                Slug = "test-article-title",
                Description = "This is a test article description",
                Content = "This is the full content of the test article",
                Priority = 1,
                CategoryId = Guid.NewGuid(),
                TeamId = null,
                TagIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() }
            };
        }

        #region Scenario 1: Success (200)

        [Fact]
        public async Task Create_ValidCommand_ReturnsOkWithArticleDto()
        {
            // Arrange
            var command = BuildValidCommand();

            var expectedDto = new ArticleDto
            {
                Id = Guid.NewGuid(),
                Title = command.Title,
                Slug = command.Slug,
                Description = command.Description,
                Content = command.Content,
                Priority = command.Priority,
                CategoryId = command.CategoryId,
                CreatedDate = DateTime.UtcNow
            };

            var methodResult = new MethodResult<ArticleDto>
            {
                StatusCode = StatusCodes.Status200OK,
                Result = expectedDto
            };

            _mockMediator
                .Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
                .ReturnsAsync(methodResult);

            // Act
            var result = await _controller.Create(command);

            // Assert
            // GetActionResult() với StatusCode 200 kỳ vọng trả về Ok(...) -> ObjectResult 200
            var objectResult = Assert.IsAssignableFrom<ObjectResult>(result);
            Assert.Equal(StatusCodes.Status200OK, objectResult.StatusCode);

            // TODO: nếu GetActionResult() trả Value = cả MethodResult<ArticleDto>
            // (thay vì chỉ Result) thì đổi assert bên dưới cho khớp.
            var returnedDto = Assert.IsType<ArticleDto>(objectResult.Value);
            Assert.Equal(expectedDto.Id, returnedDto.Id);
            Assert.Equal(expectedDto.Slug, returnedDto.Slug);
            Assert.Equal(expectedDto.Title, returnedDto.Title);

            _mockMediator.Verify(
                m => m.Send(command, It.IsAny<CancellationToken>()),
                Times.Once,
                "Mediator.Send phải được gọi đúng 1 lần với command đầu vào");
        }

        #endregion

        #region Scenario 2: Validation error - thiếu Title (400)

        [Fact]
        public async Task Create_MissingTitle_ReturnsBadRequest()
        {
            // Arrange
            var command = BuildValidCommand();
            command.Title = string.Empty;

            var methodResult = new MethodResult<ArticleDto>();
            methodResult.AddError(
                StatusCodes.Status400BadRequest,
                nameof(EnumSystemErrorCode.Required),
                nameof(command.Title));

            _mockMediator
                .Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
                .ReturnsAsync(methodResult);

            // Act
            var result = await _controller.Create(command);

            // Assert
            var objectResult = Assert.IsAssignableFrom<ObjectResult>(result);
            Assert.Equal(StatusCodes.Status400BadRequest, objectResult.StatusCode);

            _mockMediator.Verify(
                m => m.Send(command, It.IsAny<CancellationToken>()),
                Times.Once);
        }

        #endregion

        #region Scenario 3: Slug đã tồn tại (400 - DataAlreadyExist)

        [Fact]
        public async Task Create_DuplicateSlug_ReturnsBadRequest()
        {
            // Arrange
            var command = BuildValidCommand();
            command.Slug = "duplicate-slug";

            var methodResult = new MethodResult<ArticleDto>();
            methodResult.AddError(
                StatusCodes.Status400BadRequest,
                nameof(EnumSystemErrorCode.DataAlreadyExist),
                nameof(command.Slug));

            _mockMediator
                .Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
                .ReturnsAsync(methodResult);

            // Act
            var result = await _controller.Create(command);

            // Assert
            var objectResult = Assert.IsAssignableFrom<ObjectResult>(result);
            Assert.Equal(StatusCodes.Status400BadRequest, objectResult.StatusCode);

            _mockMediator.Verify(
                m => m.Send(command, It.IsAny<CancellationToken>()),
                Times.Once);
        }

        #endregion

        #region Scenario 4: Lỗi hệ thống khi CreateAsync trả null (500)

        [Fact]
        public async Task Create_RepositoryReturnsNull_ReturnsInternalServerError()
        {
            // Arrange
            var command = BuildValidCommand();

            var methodResult = new MethodResult<ArticleDto>();
            methodResult.AddError(
                StatusCodes.Status500InternalServerError,
                nameof(EnumSystemErrorCode.ServerError),
                "articleDomain");

            _mockMediator
                .Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
                .ReturnsAsync(methodResult);

            // Act
            var result = await _controller.Create(command);

            // Assert
            var objectResult = Assert.IsAssignableFrom<ObjectResult>(result);
            Assert.Equal(StatusCodes.Status500InternalServerError, objectResult.StatusCode);

            _mockMediator.Verify(
                m => m.Send(command, It.IsAny<CancellationToken>()),
                Times.Once);
        }

        #endregion

        #region Scenario 5: Verify command được forward nguyên vẹn (không bị controller sửa)

        [Fact]
        public async Task Create_ForwardsExactCommandInstance_ToMediator()
        {
            // Arrange
            var command = BuildValidCommand();
            CreateArticleCommand? capturedCommand = null;

            _mockMediator
                .Setup(m => m.Send(It.IsAny<CreateArticleCommand>(), It.IsAny<CancellationToken>()))
                .Callback<IRequest<MethodResult<ArticleDto>>, CancellationToken>((cmd, _) =>
                    capturedCommand = cmd as CreateArticleCommand)
                .ReturnsAsync(new MethodResult<ArticleDto>
                {
                    StatusCode = StatusCodes.Status200OK,
                    Result = new ArticleDto { Slug = command.Slug }
                });

            // Act
            await _controller.Create(command);

            // Assert - controller không được phép tạo command mới hay sửa field nào
            Assert.NotNull(capturedCommand);
            Assert.Same(command, capturedCommand);
        }

        #endregion
    }
}