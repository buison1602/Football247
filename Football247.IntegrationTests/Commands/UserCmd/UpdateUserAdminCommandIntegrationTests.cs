using Football247.Application.Command.UserCmd;
using Football247.Application.Common.Data;
using Football247.Domain.Models.EntityModels.DTOs.User;
using Football247.Models.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Shared.Response;

namespace Football247.IntegrationTests.Commands.UserCmd
{
    /// <summary>
    /// Integration Tests for UpdateUserAdminCommand
    /// Tests thực tế quá trình cập nhật user qua database
    /// </summary>
    public class UpdateUserAdminCommandIntegrationTests : IAsyncLifetime
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole<Guid>> _roleManager;
        private readonly IMediator _mediator;
        private ApplicationUser _testUser;

        public UpdateUserAdminCommandIntegrationTests(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole<Guid>> roleManager,
            IMediator mediator)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _mediator = mediator;
        }

        public async Task InitializeAsync()
        {
            // Create test roles
            await EnsureRoleExistsAsync(Roles.Admin);
            await EnsureRoleExistsAsync(Roles.Member);
            await EnsureRoleExistsAsync(Roles.User);

            // Create test user
            _testUser = new ApplicationUser
            {
                Id = Guid.NewGuid(),
                UserName = "testuser",
                Email = "test@example.com",
                PhoneNumber = "+84912345678",
                Points = 1000,
                SpinCount = 5,
                ReceiveInAppNotifications = true,
                ReceiveEmailNotifications = true
            };

            var createResult = await _userManager.CreateAsync(_testUser, "TestPassword123!");
            Assert.True(createResult.Succeeded);

            var roleResult = await _userManager.AddToRoleAsync(_testUser, Roles.User);
            Assert.True(roleResult.Succeeded);
        }

        public async Task DisposeAsync()
        {
            // Cleanup
            if (_testUser != null)
            {
                await _userManager.DeleteAsync(_testUser);
            }
        }

        #region Helper Methods

        private async Task EnsureRoleExistsAsync(string roleName)
        {
            var roleExists = await _roleManager.RoleExistsAsync(roleName);
            if (!roleExists)
            {
                var result = await _roleManager.CreateAsync(new IdentityRole<Guid> { Name = roleName });
                Assert.True(result.Succeeded);
            }
        }

        #endregion

        #region Basic Info Update Tests

        [Fact]
        public async Task UpdateUserAdminCommand_UpdateName_ShouldSuccess()
        {
            // Arrange
            var newName = "updatedusername";
            var command = new UpdateUserAdminCommand
            {
                Id = _testUser.Id,
                Name = newName
            };

            // Act
            var result = await _mediator.Send(command);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);

            var updatedUser = await _userManager.FindByIdAsync(_testUser.Id.ToString());
            Assert.Equal(newName, updatedUser.UserName);
        }

        [Fact]
        public async Task UpdateUserAdminCommand_UpdateEmail_ShouldSuccess()
        {
            // Arrange
            var newEmail = "newemail@example.com";
            var command = new UpdateUserAdminCommand
            {
                Id = _testUser.Id,
                Email = newEmail
            };

            // Act
            var result = await _mediator.Send(command);

            // Assert
            Assert.True(result.IsSuccess);

            var updatedUser = await _userManager.FindByIdAsync(_testUser.Id.ToString());
            Assert.Equal(newEmail, updatedUser.Email);
        }

        [Fact]
        public async Task UpdateUserAdminCommand_UpdatePhoneNumber_ShouldSuccess()
        {
            // Arrange
            var newPhone = "+84987654321";
            var command = new UpdateUserAdminCommand
            {
                Id = _testUser.Id,
                PhoneNumber = newPhone
            };

            // Act
            var result = await _mediator.Send(command);

            // Assert
            Assert.True(result.IsSuccess);

            var updatedUser = await _userManager.FindByIdAsync(_testUser.Id.ToString());
            Assert.Equal(newPhone, updatedUser.PhoneNumber);
        }

        #endregion

        #region Gamification Tests

        [Fact]
        public async Task UpdateUserAdminCommand_UpdatePoints_ShouldSuccess()
        {
            // Arrange
            var newPoints = 5000;
            var command = new UpdateUserAdminCommand
            {
                Id = _testUser.Id,
                Points = newPoints
            };

            // Act
            var result = await _mediator.Send(command);

            // Assert
            Assert.True(result.IsSuccess);

            var updatedUser = await _userManager.FindByIdAsync(_testUser.Id.ToString());
            Assert.Equal(newPoints, updatedUser.Points);
        }

        [Fact]
        public async Task UpdateUserAdminCommand_UpdateSpinCount_ShouldSuccess()
        {
            // Arrange
            var newSpins = 50;
            var command = new UpdateUserAdminCommand
            {
                Id = _testUser.Id,
                SpinCount = newSpins
            };

            // Act
            var result = await _mediator.Send(command);

            // Assert
            Assert.True(result.IsSuccess);

            var updatedUser = await _userManager.FindByIdAsync(_testUser.Id.ToString());
            Assert.Equal(newSpins, updatedUser.SpinCount);
        }

        #endregion

        #region Role Update Tests

        [Fact]
        public async Task UpdateUserAdminCommand_UpdateRoleFromUserToMember_ShouldSuccess()
        {
            // Arrange - Verify starting role
            var currentRoles = await _userManager.GetRolesAsync(_testUser);
            Assert.Contains(Roles.User, currentRoles);

            var command = new UpdateUserAdminCommand
            {
                Id = _testUser.Id,
                Role = Roles.Member
            };

            // Act
            var result = await _mediator.Send(command);

            // Assert
            Assert.True(result.IsSuccess);

            var updatedUser = await _userManager.FindByIdAsync(_testUser.Id.ToString());
            var updatedRoles = await _userManager.GetRolesAsync(updatedUser);
            Assert.Single(updatedRoles);
            Assert.Contains(Roles.Member, updatedRoles);
            Assert.DoesNotContain(Roles.User, updatedRoles);
        }

        [Fact]
        public async Task UpdateUserAdminCommand_UpdateRoleFromMemberToAdmin_ShouldSuccess()
        {
            // Arrange - Set initial role to Member
            await _userManager.RemoveFromRoleAsync(_testUser, Roles.User);
            await _userManager.AddToRoleAsync(_testUser, Roles.Member);

            var command = new UpdateUserAdminCommand
            {
                Id = _testUser.Id,
                Role = Roles.Admin
            };

            // Act
            var result = await _mediator.Send(command);

            // Assert
            Assert.True(result.IsSuccess);

            var updatedUser = await _userManager.FindByIdAsync(_testUser.Id.ToString());
            var updatedRoles = await _userManager.GetRolesAsync(updatedUser);
            Assert.Single(updatedRoles);
            Assert.Contains(Roles.Admin, updatedRoles);
            Assert.DoesNotContain(Roles.Member, updatedRoles);
        }

        [Fact]
        public async Task UpdateUserAdminCommand_UpdateRoleDowngradeAdminToUser_ShouldSuccess()
        {
            // Arrange - Set initial role to Admin
            await _userManager.RemoveFromRoleAsync(_testUser, Roles.User);
            await _userManager.AddToRoleAsync(_testUser, Roles.Admin);

            var command = new UpdateUserAdminCommand
            {
                Id = _testUser.Id,
                Role = Roles.User
            };

            // Act
            var result = await _mediator.Send(command);

            // Assert
            Assert.True(result.IsSuccess);

            var updatedUser = await _userManager.FindByIdAsync(_testUser.Id.ToString());
            var updatedRoles = await _userManager.GetRolesAsync(updatedUser);
            Assert.Single(updatedRoles);
            Assert.Contains(Roles.User, updatedRoles);
            Assert.DoesNotContain(Roles.Admin, updatedRoles);
        }

        #endregion

        #region Security Tests

        [Fact]
        public async Task UpdateUserAdminCommand_UpdateLockoutSettings_ShouldSuccess()
        {
            // Arrange
            var lockoutEnd = new DateTimeOffset(2026, 12, 31, 23, 59, 59, TimeSpan.Zero);
            var command = new UpdateUserAdminCommand
            {
                Id = _testUser.Id,
                LockoutEnabled = true,
                LockoutEnd = lockoutEnd
            };

            // Act
            var result = await _mediator.Send(command);

            // Assert
            Assert.True(result.IsSuccess);

            var updatedUser = await _userManager.FindByIdAsync(_testUser.Id.ToString());
            Assert.True(updatedUser.LockoutEnabled);
            Assert.Equal(lockoutEnd, updatedUser.LockoutEnd);
        }

        [Fact]
        public async Task UpdateUserAdminCommand_UnlockAccount_ShouldSuccess()
        {
            // Arrange - Lock account first
            var lockoutEnd = new DateTimeOffset(2026, 12, 31, 23, 59, 59, TimeSpan.Zero);
            _testUser.LockoutEnd = lockoutEnd;
            await _userManager.UpdateAsync(_testUser);

            var command = new UpdateUserAdminCommand
            {
                Id = _testUser.Id,
                LockoutEnd = null
            };

            // Act
            var result = await _mediator.Send(command);

            // Assert
            Assert.True(result.IsSuccess);

            var updatedUser = await _userManager.FindByIdAsync(_testUser.Id.ToString());
            Assert.Null(updatedUser.LockoutEnd);
        }

        [Fact]
        public async Task UpdateUserAdminCommand_UpdateTwoFactorEnabled_ShouldSuccess()
        {
            // Arrange
            var command = new UpdateUserAdminCommand
            {
                Id = _testUser.Id,
                TwoFactorEnabled = true
            };

            // Act
            var result = await _mediator.Send(command);

            // Assert
            Assert.True(result.IsSuccess);

            var updatedUser = await _userManager.FindByIdAsync(_testUser.Id.ToString());
            Assert.True(updatedUser.TwoFactorEnabled);
        }

        [Fact]
        public async Task UpdateUserAdminCommand_UpdateEmailConfirmed_ShouldSuccess()
        {
            // Arrange
            _testUser.EmailConfirmed = false;
            await _userManager.UpdateAsync(_testUser);

            var command = new UpdateUserAdminCommand
            {
                Id = _testUser.Id,
                EmailConfirmed = true
            };

            // Act
            var result = await _mediator.Send(command);

            // Assert
            Assert.True(result.IsSuccess);

            var updatedUser = await _userManager.FindByIdAsync(_testUser.Id.ToString());
            Assert.True(updatedUser.EmailConfirmed);
        }

        #endregion

        #region Error Cases

        [Fact]
        public async Task UpdateUserAdminCommand_UserNotFound_ShouldReturnNotFound()
        {
            // Arrange
            var nonExistentUserId = Guid.NewGuid();
            var command = new UpdateUserAdminCommand
            {
                Id = nonExistentUserId,
                Name = "newname"
            };

            // Act
            var result = await _mediator.Send(command);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(StatusCodes.Status404NotFound, result.StatusCode);
        }

        [Fact]
        public async Task UpdateUserAdminCommand_InvalidRole_ShouldReturnBadRequest()
        {
            // Arrange
            var command = new UpdateUserAdminCommand
            {
                Id = _testUser.Id,
                Role = "InvalidRole"
            };

            // Act
            var result = await _mediator.Send(command);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(StatusCodes.Status400BadRequest, result.StatusCode);
            Assert.Single(result.Errors);
            Assert.Equal("InvalidRole", result.Errors.First().ErrorCode);
        }

        [Fact]
        public async Task UpdateUserAdminCommand_NameTooShort_ShouldReturnBadRequest()
        {
            // Arrange
            var command = new UpdateUserAdminCommand
            {
                Id = _testUser.Id,
                Name = "short"
            };

            // Act
            var result = await _mediator.Send(command);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(StatusCodes.Status400BadRequest, result.StatusCode);
        }

        [Fact]
        public async Task UpdateUserAdminCommand_DuplicateEmail_ShouldReturnBadRequest()
        {
            // Arrange - Create another user
            var anotherUser = new ApplicationUser
            {
                Id = Guid.NewGuid(),
                UserName = "anotheruser",
                Email = "another@example.com"
            };
            await _userManager.CreateAsync(anotherUser, "TestPassword123!");

            var command = new UpdateUserAdminCommand
            {
                Id = _testUser.Id,
                Email = "another@example.com"
            };

            // Act
            var result = await _mediator.Send(command);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(StatusCodes.Status400BadRequest, result.StatusCode);

            // Cleanup
            await _userManager.DeleteAsync(anotherUser);
        }

        #endregion

        #region Comprehensive Tests

        [Fact]
        public async Task UpdateUserAdminCommand_UpdateMultipleFieldsWithNewRole_ShouldSuccess()
        {
            // Arrange
            var command = new UpdateUserAdminCommand
            {
                Id = _testUser.Id,
                Name = "newusername",
                Email = "newemail@example.com",
                PhoneNumber = "+84999999999",
                Role = Roles.Member,
                Points = 10000,
                SpinCount = 100,
                EmailConfirmed = true,
                TwoFactorEnabled = true
            };

            // Act
            var result = await _mediator.Send(command);

            // Assert
            Assert.True(result.IsSuccess);

            var updatedUser = await _userManager.FindByIdAsync(_testUser.Id.ToString());
            Assert.Equal("newusername", updatedUser.UserName);
            Assert.Equal("newemail@example.com", updatedUser.Email);
            Assert.Equal("+84999999999", updatedUser.PhoneNumber);
            Assert.Equal(10000, updatedUser.Points);
            Assert.Equal(100, updatedUser.SpinCount);
            Assert.True(updatedUser.EmailConfirmed);
            Assert.True(updatedUser.TwoFactorEnabled);

            var roles = await _userManager.GetRolesAsync(updatedUser);
            Assert.Single(roles);
            Assert.Contains(Roles.Member, roles);
        }

        [Fact]
        public async Task UpdateUserAdminCommand_PartialUpdate_ShouldOnlyUpdateProvidedFields()
        {
            // Arrange
            var originalName = _testUser.UserName;
            var originalEmail = _testUser.Email;
            var originalPhone = _testUser.PhoneNumber;
            var originalRole = (await _userManager.GetRolesAsync(_testUser)).First();

            var command = new UpdateUserAdminCommand
            {
                Id = _testUser.Id,
                Points = 9999  // Only update points
                // All other fields are null
            };

            // Act
            var result = await _mediator.Send(command);

            // Assert
            Assert.True(result.IsSuccess);

            var updatedUser = await _userManager.FindByIdAsync(_testUser.Id.ToString());
            Assert.Equal(originalName, updatedUser.UserName);
            Assert.Equal(originalEmail, updatedUser.Email);
            Assert.Equal(originalPhone, updatedUser.PhoneNumber);
            Assert.Equal(9999, updatedUser.Points);

            var roles = await _userManager.GetRolesAsync(updatedUser);
            Assert.Contains(originalRole, roles);
        }

        #endregion
    }
}
