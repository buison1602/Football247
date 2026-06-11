using Football247.Domain.Models.CommandModels.SendEmailCmdModel;
using Football247.Domain.Models.EntityModels.DTOs.SendEmail;
using Football247.Domain.ValueSettings;
using Football247.Shared.Enum.ErrorCode;
using MediatR;
using Microsoft.AspNetCore.Http;
using MimeKit;
using Shared.Constants;
using Shared.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Football247.Application.Command.SendEmailCmd
{
    public class SendEmailCommand : SendEmailCommandModel, IRequest<MethodResult<bool>>
    {
    }

    public class SendEmailCommandHandler : IRequestHandler<SendEmailCommand, MethodResult<bool>>
    {
        private readonly AppSetting _appSetting;

        public SendEmailCommandHandler(AppSetting appSetting)
        {
            _appSetting = appSetting;
        }

        public async Task<MethodResult<bool>> Handle(SendEmailCommand request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request);
            var methodResult = new MethodResult<bool>();

            if (request.ToEmails == null || !request.ToEmails.Any() || string.IsNullOrEmpty(request.Subject) || string.IsNullOrEmpty(request.Content))
            {
                methodResult.AddError(StatusCodes.Status400BadRequest, nameof(EnumSystemErrorCode.InValidFormat), nameof(request.ToEmails));
                return methodResult;
            }

            SendEmailDto sendEmailDto = new SendEmailDto
            {
                ToEmails = request.ToEmails,
                Subject = request.Subject,
                Content = request.Content
            };

            using (var emailMessage = CreateEmailMessage(sendEmailDto))
            {
                await Send(emailMessage);
            }

            methodResult.StatusCode = StatusCodes.Status200OK;
            methodResult.Result = true;
            return methodResult;
        }


        private MimeMessage CreateEmailMessage(SendEmailDto message)
        {
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress(SenderSettings.HostName, _appSetting?.Smtp?.From ?? string.Empty));

            if (message.ToEmails != null)
            {
                foreach (var item in message.ToEmails)
                {
                    emailMessage.To.Add(new MailboxAddress(item, item));
                }
            }

            emailMessage.Subject = message.Subject;
            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = message.Content };

            return emailMessage;
        }

        private async Task<bool> Send(MimeMessage mailmessage)
        {
            using (var client = new MailKit.Net.Smtp.SmtpClient())
            {
                try
                {
                    client.ServerCertificateValidationCallback = (s, c, h, e) => true;
                    await client.ConnectAsync(_appSetting?.Smtp?.SmtpServer ?? string.Empty, _appSetting?.Smtp?.Port ?? 0, true);
                    client.AuthenticationMechanisms.Remove("XOAUTH2");
                    await client.AuthenticateAsync(_appSetting?.Smtp?.Username ?? string.Empty, _appSetting?.Smtp?.Password ?? string.Empty);
                    await client.SendAsync(mailmessage);
                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return false;
                    throw;
                }
                finally
                {
                    await client.DisconnectAsync(true);
                    client.Dispose();
                }
            }
        }
    }
}
