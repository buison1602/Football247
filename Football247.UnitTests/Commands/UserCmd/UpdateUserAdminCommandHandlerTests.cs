using Football247.Application.Command.UserCmd;
using Football247.Application.Common.Data;
using Football247.Domain.Models.EntityModels.DTOs.User;
using Football247.Models.Entities;
using Football247.Shared.Enum.ErrorCode;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Moq;
using Xunit;
using Shared.Response;

namespace Football247.UnitTests.Commands.UserCmd
{
    public class UpdateUserAdminCommandHandlerTests
    {
        private readonly Mock<UserManager<ApplicationUser>> _mockUserManager;
        private readonly UpdateUserAdminCommandHandler _handler;

        public UpdateUserAdminCommandHandlerTests()
        {
            var userStoreMock = new Mock<IUserStore<ApplicationUser>>();
            _mockUserManager = new Mock<UserManager<ApplicationUser>>(
                userStoreMock.Object, null, null, null, null, null, null, null, null);
            _handler = new UpdateUserAdminCommandHandler(_mockUserManager.Object);
        }

        #region Setup Helpers

        private ApplicationUser CreateTestUser(Guid id = default, string email = "test@example.com")
        {
            return new ApplicationUser
            {
                Id = id == default ? Guid.NewGuid() : id,
                UserName = "testuser",
                Email = email,
                PhoneNumber = "+84912345678",
                AvatarUrl = "https://example.com/avatar.jpg",
                Points = 1000,
                SpinCount = 5,
                ReceiveInAppNotifications = true,
                ReceiveEmailNotifications = true,
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
                LockoutEnabled = false,
                TwoFactorEnabled = false
            };
        }

        #endregion

        #region User Not Found Tests

        [Fact]
        public async Task Handle_UserNotFound_ShouldReturnNotFoundError()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var command = new UpdateUserAdminCommand { Id = userId, Name = "newname" };

            _mockUserManager.Setup(x => x.FindByIdAsync(userId.ToString()))
                .ReturnsAsync((ApplicationUser)null);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(StatusCodes.Status404NotFound, result.StatusCode);
            Assert.Equal(nameof(EnumSystemErrorCode.DataNotExist), result.ErrorMessage);
        }

        #endregion

        #region Validation Tests

        [Fact]
        public async Task Handle_NameLessThan6Characters_ShouldReturnBadRequest()
        {
            // Arrange
            var user = CreateTestUser();
            var command = new UpdateUserAdminCommand { Id = user.Id, Name = "short" };

            _mockUserManager.Setup(x => x.FindByIdAsync(user.Id.ToString()))
                .ReturnsAsync(user);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(StatusCodes.Status400BadRequest, result.StatusCode);
            Assert.Equal(nameof(EnumSystemErrorCode.Min), result.ErrorMessage);
        }

        [Fact]
        public async Task Handle_EmailAlreadyExists_ShouldReturnBadRequest()
        {
            // Arrange
            var user = CreateTestUser();
            var existingUser = CreateTestUser(Guid.NewGuid(), "existing@example.com");
            var newEmail = "existing@example.com";
            var command = new UpdateUserAdminCommand { Id = user.Id, Email = newEmail };

            _mockUserManager.Setup(x => x.FindByIdAsync(user.Id.ToString()))
                .ReturnsAsync(user);
            _mockUserManager.Setup(x => x.FindByEmailAsync(newEmail))
                .ReturnsAsync(existingUser);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(StatusCodes.Status400BadRequest, result.StatusCode);
            Assert.Equal(nameof(EnumSystemErrorCode.DataAlreadyExist), result.ErrorMessage);
        }

        [Theory]
        [InlineData("InvalidRole")]
        [InlineData("CEO")]
        [InlineData("SuperAdmin")]
        [InlineData("Editor")]
        public async Task Handle_InvalidRole_ShouldReturnBadRequest(string invalidRole)
        {
            // Arrange
            var user = CreateTestUser();
            var command = new UpdateUserAdminCommand { Id = user.Id, Role = invalidRole };

            _mockUserManager.Setup(x => x.FindByIdAsync(user.Id.ToString()))
                .ReturnsAsync(user);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(StatusCodes.Status400BadRequest, result.StatusCode);
            Assert.Equal("InvalidRole", result.ErrorMessage);
            Assert.Contains("Admin, Member, User", result.ErrorField ?? "");
        }

        #endregion

        #region Valid Role Tests

        [Theory]
        [InlineData(Roles.Admin)]
        [InlineData(Roles.Member)]
        [InlineData(Roles.User)]
        public async Task Handle_ValidRole_ShouldAccept(string validRole)
        {
            // Arrange
            var user = CreateTestUser();
            var command = new UpdateUserAdminCommand { Id = user.Id, Role = validRole };

            _mockUserManager.Setup(x => x.FindByIdAsync(user.Id.ToString()))
                .ReturnsAsync(user);
            _mockUserManager.Setup(x => x.UpdateAsync(It.IsAny<ApplicationUser>()))
                .ReturnsAsync(IdentityResult.Success);
            _mockUserManager.Setup(x => x.GetRolesAsync(user))
                .ReturnsAsync(new List<string> { "OldRole" });
            _mockUserManager.Setup(x => x.RemoveFromRolesAsync(user, It.IsAny<IEnumerable<string>>()))
                .ReturnsAsync(IdentityResult.Success);
            _mockUserManager.Setup(x => x.AddToRoleAsync(user, validRole))
                .ReturnsAsync(IdentityResult.Success);
            _mockUserManager.Setup(x => x.GetRolesAsync(user))
                .ReturnsAsync(new List<string> { validRole });

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
            Assert.NotNull(result.Result);
        }

        #endregion

        #region Basic Info Update Tests

        [Fact]
        public async Task Handle_UpdateName_ShouldSucceed()
        {
            // Arrange
            var user = CreateTestUser();
            var newName = "newusername";
            var command = new UpdateUserAdminCommand { Id = user.Id, Name = newName };

            _mockUserManager.Setup(x => x.FindByIdAsync(user.Id.ToString()))
                .ReturnsAsync(user);
            _mockUserManager.Setup(x => x.UpdateAsync(It.IsAny<ApplicationUser>()))
                .ReturnsAsync(IdentityResult.Success);
            _mockUserManager.Setup(x => x.GetRolesAsync(user))
                .ReturnsAsync(new List<string> { Roles.User });

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(newName, user.UserName);
            _mockUserManager.Verify(x => x.UpdateAsync(user), Times.Once);
        }

        [Fact]
        public async Task Handle_UpdateEmail_ShouldSucceed()
        {
            // Arrange
            var user = CreateTestUser();
            var newEmail = "newemail@example.com";
            var command = new UpdateUserAdminCommand { Id = user.Id, Email = newEmail };

            _mockUserManager.Setup(x => x.FindByIdAsync(user.Id.ToString()))
                .ReturnsAsync(user);
            _mockUserManager.Setup(x => x.FindByEmailAsync(newEmail))
                .ReturnsAsync((ApplicationUser)null);
            _mockUserManager.Setup(x => x.UpdateAsync(It.IsAny<ApplicationUser>()))
                .ReturnsAsync(IdentityResult.Success);
            _mockUserManager.Setup(x => x.GetRolesAsync(user))
                .ReturnsAsync(new List<string> { Roles.User });

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(newEmail, user.Email);
        }

        [Fact]
        public async Task Handle_UpdatePhoneNumber_ShouldSucceed()
        {
            // Arrange
            var user = CreateTestUser();
            var newPhone = "+84987654321";
            var command = new UpdateUserAdminCommand { Id = user.Id, PhoneNumber = newPhone };

            _mockUserManager.Setup(x => x.FindByIdAsync(user.Id.ToString()))
                .ReturnsAsync(user);
            _mockUserManager.Setup(x => x.UpdateAsync(It.IsAny<ApplicationUser>()))
                .ReturnsAsync(IdentityResult.Success);
            _mockUserManager.Setup(x => x.GetRolesAsync(user))
                .ReturnsAsync(new List<string> { Roles.User });

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(newPhone, user.PhoneNumber);
        }

        #endregion

        #region Gamification Tests

        [Fact]
        public async Task Handle_UpdatePoints_ShouldSucceed()
        {
            // Arrange
            var user = CreateTestUser();
            var newPoints = 5000;
            var command = new UpdateUserAdminCommand { Id = user.Id, Points = newPoints };

            _mockUserManager.Setup(x => x.FindByIdAsync(user.Id.ToString()))
                .ReturnsAsync(user);
            _mockUserManager.Setup(x => x.UpdateAsync(It.IsAny<ApplicationUser>()))
                .ReturnsAsync(IdentityResult.Success);
            _mockUserManager.Setup(x => x.GetRolesAsync(user))
                .ReturnsAsync(new List<string> { Roles.User });

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(newPoints, user.Points);
        }

        [Fact]
        public async Task Handle_UpdateSpinCount_ShouldSucceed()
        {
            // Arrange
            var user = CreateTestUser();
            var newSpins = 50;
            var command = new UpdateUserAdminCommand { Id = user.Id, SpinCount = newSpins };

            _mockUserManager.Setup(x => x.FindByIdAsync(user.Id.ToString()))
                .ReturnsAsync(user);
            _mockUserManager.Setup(x => x.UpdateAsync(It.IsAny<ApplicationUser>()))
                .ReturnsAsync(IdentityResult.Success);
            _mockUserManager.Setup(x => x.GetRolesAsync(user))
                .ReturnsAsync(new List<string> { Roles.User });

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(newSpins, user.SpinCount);
        }

        #endregion

        #region Role Update Tests

        [Fact]
        public async Task Handle_UpdateRoleFromUserToMember_ShouldRemoveOldAndAddNew()
        {
            // Arrange
            var user = CreateTestUser();
            var command = new UpdateUserAdminCommand { Id = user.Id, Role = Roles.Member };

            _mockUserManager.Setup(x => x.FindByIdAsync(user.Id.ToString()))
                .ReturnsAsync(user);
            _mockUserManager.Setup(x => x.UpdateAsync(It.IsAny<ApplicationUser>()))
                .ReturnsAsync(IdentityResult.Success);
            _mockUserManager.Setup(x => x.GetRolesAsync(user))
                .ReturnsAsync(new List<string> { Roles.User });
            _mockUserManager.Setup(x => x.RemoveFromRolesAsync(user, It.IsAny<IEnumerable<string>>()))
                .ReturnsAsync(IdentityResult.Success);
            _mockUserManager.Setup(x => x.AddToRoleAsync(user, Roles.Member))
                .ReturnsAsync(IdentityResult.Success);
            _mockUserManager.Setup(x => x.GetRolesAsync(user))
                .ReturnsAsync(new List<string> { Roles.Member });

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            _mockUserManager.Verify(x => x.RemoveFromRolesAsync(user, It.IsAny<IEnumerable<string>>()), Times.Once);
            _mockUserManager.Verify(x => x.AddToRoleAsync(user, Roles.Member), Times.Once);
        }

        [Fact]
        public async Task Handle_FailToRemoveOldRoles_ShouldReturnServerError()
        {
            // Arrange
            var user = CreateTestUser();
            var command = new UpdateUserAdminCommand { Id = user.Id, Role = Roles.Admin };

            _mockUserManager.Setup(x => x.FindByIdAsync(user.Id.ToString()))
                .ReturnsAsync(user);
            _mockUserManager.Setup(x => x.UpdateAsync(It.IsAny<ApplicationUser>()))
                .ReturnsAsync(IdentityResult.Success);
            _mockUserManager.Setup(x => x.GetRolesAsync(user))
                .ReturnsAsync(new List<string> { Roles.User });
            _mockUserManager.Setup(x => x.RemoveFromRolesAsync(user, It.IsAny<IEnumerable<string>>()))
                .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "Failed to remove role" }));

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(StatusCodes.Status500InternalServerError, result.StatusCode);
            Assert.Contains("Failed to remove old roles", result.ErrorMessage ?? "");
        }

        [Fact]
        public async Task Handle_FailToAddNewRole_ShouldReturnServerError()
        {
            // Arrange
            var user = CreateTestUser();
            var command = new UpdateUserAdminCommand { Id = user.Id, Role = Roles.Admin };

            _mockUserManager.Setup(x => x.FindByIdAsync(user.Id.ToString()))
                .ReturnsAsync(user);
            _mockUserManager.Setup(x => x.UpdateAsync(It.IsAny<ApplicationUser>()))
                .ReturnsAsync(IdentityResult.Success);
            _mockUserManager.Setup(x => x.GetRolesAsync(user))
                .ReturnsAsync(new List<string> { Roles.User });
            _mockUserManager.Setup(x => x.RemoveFromRolesAsync(user, It.IsAny<IEnumerable<string>>()))
                .ReturnsAsync(IdentityResult.Success);
            _mockUserManager.Setup(x => x.AddToRoleAsync(user, Roles.Admin))
                .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "Failed to add role" }));

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(StatusCodes.Status500InternalServerError, result.StatusCode);
            Assert.Contains("Failed to assign new role", result.ErrorMessage ?? "");
        }

        #endregion

        #region Security Tests

        [Fact]
        public async Task Handle_UpdateLockoutSettings_ShouldSucceed()
        {
            // Arrange
            var user = CreateTestUser();
            var lockoutEnd = new DateTimeOffset(2026, 12, 31, 23, 59, 59, TimeSpan.Zero);
            var command = new UpdateUserAdminCommand
            {
                Id = user.Id,
                LockoutEnabled = true,
                LockoutEnd = lockoutEnd
            };

            _mockUserManager.Setup(x => x.FindByIdAsync(user.Id.ToString()))
                .ReturnsAsync(user);
            _mockUserManager.Setup(x => x.UpdateAsync(It.IsAny<ApplicationUser>()))
                .ReturnsAsync(IdentityResult.Success);
            _mockUserManager.Setup(x => x.GetRolesAsync(user))
                .ReturnsAsync(new List<string> { Roles.User });

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.True(user.LockoutEnabled);
            Assert.Equal(lockoutEnd, user.LockoutEnd);
        }

        [Fact]
        public async Task Handle_ResetAccessFailedCount_ShouldBeCalledWhenTrue()
        {
            // Arrange
            var user = CreateTestUser();
            var command = new UpdateUserAdminCommand
            {
                Id = user.Id,
                ResetAccessFailedCount = true
            };

            _mockUserManager.Setup(x => x.FindByIdAsync(user.Id.ToString()))
                .ReturnsAsync(user);
            _mockUserManager.Setup(x => x.UpdateAsync(It.IsAny<ApplicationUser>()))
                .ReturnsAsync(IdentityResult.Success);
            _mockUserManager.Setup(x => x.ResetAccessFailedCountAsync(user))
                .ReturnsAsync(IdentityResult.Success);
            _mockUserManager.Setup(x => x.GetRolesAsync(user))
                .ReturnsAsync(new List<string> { Roles.User });

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            _mockUserManager.Verify(x => x.ResetAccessFailedCountAsync(user), Times.Once);
        }

        #endregion

        #region Comprehensive Tests

        [Fact]
        public async Task Handle_UpdateMultipleFieldsIncludingRole_ShouldSucceed()
        {
            // Arrange
            var user = CreateTestUser();
            var command = new UpdateUserAdminCommand
            {
                Id = user.Id,
                Name = "newusername",
                Email = "newemail@example.com",
                PhoneNumber = "+84987654321",
                Role = Roles.Member,
                Points = 10000,
                SpinCount = 100,
                LockoutEnabled = false,
                EmailConfirmed = true,
                TwoFactorEnabled = true
            };

            _mockUserManager.Setup(x => x.FindByIdAsync(user.Id.ToString()))
                .ReturnsAsync(user);
            _mockUserManager.Setup(x => x.FindByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync((ApplicationUser)null);
            _mockUserManager.Setup(x => x.UpdateAsync(It.IsAny<ApplicationUser>()))
                .ReturnsAsync(IdentityResult.Success);
            _mockUserManager.Setup(x => x.GetRolesAsync(user))
                .ReturnsAsync(new List<string> { Roles.User });
            _mockUserManager.Setup(x => x.RemoveFromRolesAsync(user, It.IsAny<IEnumerable<string>>()))
                .ReturnsAsync(IdentityResult.Success);
            _mockUserManager.Setup(x => x.AddToRoleAsync(user, Roles.Member))
                .ReturnsAsync(IdentityResult.Success);
            _mockUserManager.Setup(x => x.GetRolesAsync(user))
                .ReturnsAsync(new List<string> { Roles.Member });

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
            Assert.NotNull(result.Result);
            Assert.Equal("newusername", user.UserName);
            Assert.Equal("newemail@example.com", user.Email);
            Assert.Equal("+84987654321", user.PhoneNumber);
            Assert.Equal(10000, user.Points);
            Assert.Equal(100, user.SpinCount);
        }

        [Fact]
        public async Task Handle_NullFields_ShouldNotUpdateCorrespondingProperties()
        {
            // Arrange
            var user = CreateTestUser();
            var originalName = user.UserName;
            var originalEmail = user.Email;
            var command = new UpdateUserAdminCommand
            {
                Id = user.Id,
                Name = null,
                Email = null,
                Role = null
                // All fields are null
            };

            _mockUserManager.Setup(x => x.FindByIdAsync(user.Id.ToString()))
                .ReturnsAsync(user);
            _mockUserManager.Setup(x => x.UpdateAsync(It.IsAny<ApplicationUser>()))
                .ReturnsAsync(IdentityResult.Success);
            _mockUserManager.Setup(x => x.GetRolesAsync(user))
                .ReturnsAsync(new List<string> { Roles.User });

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(originalName, user.UserName);
            Assert.Equal(originalEmail, user.Email);
        }

        #endregion
    }
}
