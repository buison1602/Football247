using AutoMapper;
using Football247.Application.Common.Data;
using Football247.Domain.Models.CommandModels.UserCmdModel;
using Football247.Domain.Models.EntityModels.DTOs.User;
using Football247.Models.Entities;
using Football247.Shared.Enum.ErrorCode;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Shared.Response;

namespace Football247.Application.Command.UserCmd
{
    public class UpdateUserAdminCommand : UpdateUserAdminCommandModel, IRequest<MethodResult<UserDto>>
    {
    }

    public class UpdateUserAdminCommandHandler : IRequestHandler<UpdateUserAdminCommand, MethodResult<UserDto>>
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UpdateUserAdminCommandHandler(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<MethodResult<UserDto>> Handle(UpdateUserAdminCommand request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request);
            var methodResult = new MethodResult<UserDto>();

            // === Tìm user ===
            var user = await _userManager.FindByIdAsync(request.Id.ToString());
            if (user == null)
            {
                methodResult.AddError(StatusCodes.Status404NotFound, nameof(EnumSystemErrorCode.DataNotExist), 
                    nameof(request.Id), request.Id.ToString());
                return methodResult;
            }   

            // === Validation ===
            if (!string.IsNullOrWhiteSpace(request.Name) && request.Name.Length < 6)
            {
                methodResult.AddError(StatusCodes.Status400BadRequest, nameof(EnumSystemErrorCode.Min), 
                    nameof(request.Name), "Name must be at least 6 characters long.");
                return methodResult;
            }

            if (!string.IsNullOrWhiteSpace(request.Email) && request.Email != user.Email)
            {
                // Check if email already exists
                var existingUserWithEmail = await _userManager.FindByEmailAsync(request.Email);
                if (existingUserWithEmail != null)
                {
                    methodResult.AddError(StatusCodes.Status400BadRequest, nameof(EnumSystemErrorCode.DataAlreadyExist), 
                        nameof(request.Email), request.Email);
                    return methodResult;
                }
            }

            // === Validate Role ===
            if (!string.IsNullOrWhiteSpace(request.Role))
            {
                var validRoles = new[] { Roles.Admin, Roles.Member, Roles.User };
                if (!validRoles.Contains(request.Role))
                {
                    methodResult.AddError(StatusCodes.Status400BadRequest, "InvalidRole",
                        $"Role must be one of: {string.Join(", ", validRoles)}");
                    return methodResult;
                }
            }

            // === Update Basic Info ===
            if (!string.IsNullOrWhiteSpace(request.Name))
                user.UserName = request.Name;

            if (!string.IsNullOrWhiteSpace(request.Email) && request.Email != user.Email)
            {
                user.Email = request.Email;
                // ⚠️ Email không được tự động xác nhận, admin cần set EmailConfirmed = true nếu muốn
            }

            if (!string.IsNullOrWhiteSpace(request.PhoneNumber))
                user.PhoneNumber = request.PhoneNumber;

            if (!string.IsNullOrWhiteSpace(request.AvatarUrl))
                user.AvatarUrl = request.AvatarUrl;

            // === Update Notification Preferences ===
            if (request.ReceiveInAppNotifications.HasValue)
                user.ReceiveInAppNotifications = request.ReceiveInAppNotifications.Value;

            if (request.ReceiveEmailNotifications.HasValue)
                user.ReceiveEmailNotifications = request.ReceiveEmailNotifications.Value;

            // === Update Gamification ===
            if (request.Points.HasValue && request.Points.Value >= 0)
                user.Points = request.Points.Value;

            if (request.SpinCount.HasValue && request.SpinCount.Value >= 0)
                user.SpinCount = request.SpinCount.Value;

            // === Update Security ===
            if (request.LockoutEnabled.HasValue)
                user.LockoutEnabled = request.LockoutEnabled.Value;

            if (request.LockoutEnd.HasValue)
                user.LockoutEnd = request.LockoutEnd.Value;

            if (request.EmailConfirmed.HasValue)
                user.EmailConfirmed = request.EmailConfirmed.Value;

            if (request.PhoneNumberConfirmed.HasValue)
                user.PhoneNumberConfirmed = request.PhoneNumberConfirmed.Value;

            if (request.TwoFactorEnabled.HasValue)
                user.TwoFactorEnabled = request.TwoFactorEnabled.Value;

            // === Reset Access Failed Count ===
            if (request.ResetAccessFailedCount.HasValue && request.ResetAccessFailedCount.Value)
            {
                await _userManager.ResetAccessFailedCountAsync(user);
            }

            // === Save Changes ===
            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                methodResult.AddError(StatusCodes.Status500InternalServerError, nameof(EnumSystemErrorCode.ServerError), 
                    $"Failed to update user: {errors}");
                return methodResult;
            }

            // === Update Role (Nếu có) ===
            if (!string.IsNullOrWhiteSpace(request.Role))
            {
                try
                {
                    // Lấy tất cả vai trò hiện tại của user
                    var currentRoles = await _userManager.GetRolesAsync(user);

                    // Xóa tất cả vai trò cũ
                    if (currentRoles.Any())
                    {
                        var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);
                        if (!removeResult.Succeeded)
                        {
                            var errors = string.Join(", ", removeResult.Errors.Select(e => e.Description));
                            methodResult.AddError(StatusCodes.Status500InternalServerError, nameof(EnumSystemErrorCode.ServerError),
                                $"Failed to remove old roles: {errors}");
                            return methodResult;
                        }
                    }

                    // Gán vai trò mới
                    var addResult = await _userManager.AddToRoleAsync(user, request.Role);
                    if (!addResult.Succeeded)
                    {
                        var errors = string.Join(", ", addResult.Errors.Select(e => e.Description));
                        methodResult.AddError(StatusCodes.Status500InternalServerError, nameof(EnumSystemErrorCode.ServerError),
                            $"Failed to assign new role: {errors}");
                        return methodResult;
                    }
                }
                catch (Exception ex)
                {
                    methodResult.AddError(StatusCodes.Status500InternalServerError, nameof(EnumSystemErrorCode.ServerError),
                        $"Error updating role: {ex.Message}");
                    return methodResult;
                }
            }

            // === Lấy roles cập nhật ===
            var updatedRoles = await _userManager.GetRolesAsync(user);

            methodResult.Result = new UserDto
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                AvatarUrl = user.AvatarUrl ?? string.Empty,
                Points = user.Points,
                SpinCount = user.SpinCount,
                ReceiveEmailNotifications = user.ReceiveEmailNotifications,
                ReceiveInAppNotifications = user.ReceiveInAppNotifications
            };

            return methodResult;
        }
    }
}