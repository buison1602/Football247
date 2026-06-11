using Football247.Application.Command.SendEmailCmd;
using Football247.Application.Helper;
using Football247.Models.Entities;
using Football247.Repositories.IRepository;
using Football247.Shared.Enum.ErrorCode;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Shared.Enum;
using Shared.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Football247.Authorization.Permissions;

namespace Football247.Application.Command.AuthCmd
{
    public class ForgotPasswordCommand : IRequest<MethodResult<string>>
    {
        public string? Email { get; set; }
    }

    public class ForgotPasswordCommandHandler : IRequestHandler<ForgotPasswordCommand, MethodResult<string>>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMediator _mediator;

        public ForgotPasswordCommandHandler(UserManager<ApplicationUser> userManager, IUnitOfWork unitOfWork, IMediator mediator)
        {
            _userManager = userManager;
            _unitOfWork = unitOfWork;
            _mediator = mediator;
        }

        public async Task<MethodResult<string>> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request);
            var methodResult = new MethodResult<string>();

            if (string.IsNullOrEmpty(request.Email))
            {
                methodResult.AddError(StatusCodes.Status400BadRequest, nameof(EnumSystemErrorCode.UserNameNotNull), nameof(request.Email));
                return methodResult;
            }

            var user = await _userManager.Users.FirstOrDefaultAsync(x => (x.Email == request.Email), cancellationToken);
            if (user == null)
            {
                methodResult.AddError(StatusCodes.Status400BadRequest, nameof(EnumSystemErrorCode.EmailNotExist), nameof(request.Email));
                return methodResult;
            }

            var newPassword = new PasswordGeneratorHelper(8, 10).Generate();

            string resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
            if (string.IsNullOrEmpty(resetToken))
            {
                methodResult.StatusCode = StatusCodes.Status500InternalServerError;
                methodResult.AddError(StatusCodes.Status500InternalServerError, nameof(EnumSystemErrorCode.ErrorResetToken), nameof(resetToken));
                return methodResult;
            }

            var hashPassword = _userManager.PasswordHasher.HashPassword(user, newPassword);
            user.PasswordHash = hashPassword;
            await _userManager.UpdateAsync(user);

            var param = new
            {
                Title = newPassword.ToString()
            };

            var subject = $"Mật khẩu mới của bạn";

            if (!string.IsNullOrEmpty(user.Email))
            {
                await _mediator.Send(new SendEmailByTemplateCommand
                { 
                    ToEmails = new List<string> { request.Email },
                    Params = param, 
                    Subject = subject, 
                    Template = EnumSenderTemplate.SendNewPassword 
                }, cancellationToken).ConfigureAwait(false);
            }
            else
            {
                methodResult.AddError(StatusCodes.Status400BadRequest, nameof(EnumSystemErrorCode.EmailNull), nameof(request.Email));
                return methodResult;
            }

            methodResult.Result = request.Email;
            methodResult.StatusCode = StatusCodes.Status200OK;
            return methodResult;
        }
    }
}