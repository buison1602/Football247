using Football247.Domain.Models.CommandModels.SendEmailCmdModel;
using Football247.Domain.ValueSettings;
using MediatR;
using Microsoft.AspNetCore.Http;
using Shared.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Football247.Application.Command.SendEmailCmd
{
    public class SendListEmailCommandModel
    {
        public IList<SendEmailCommandModel> ListEmails { get; set; } = new List<SendEmailCommandModel>();
    }

    public class SendListEmailCommand : SendListEmailCommandModel, IRequest<MethodResult<bool>>
    {
    }

    public class SendListEmailCommandHandler
   : IRequestHandler<SendListEmailCommand, MethodResult<bool>>
    {
        private readonly AppSetting _appSetting;

        public SendListEmailCommandHandler(AppSetting appSetting)
        {
            _appSetting = appSetting;
        }

        public async Task<MethodResult<bool>> Handle(
            SendListEmailCommand request,
            CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request);

            //foreach (var item in request.ListEmails)
            //{
            //    var sendEmail = new SendEmailModel
            //    {
            //        Subject = item.Subject,
            //        ToEmails = item.ToEmails,
            //        CcEmails = item.CcEmails,
            //        BccEmails = item.BccEmails,
            //        Content = item.Content
            //    };

            //    using var emailMessage =
            //        EmailHelper.CreateEmailMessage(sendEmail, _appSetting);

            //    await EmailHelper.SendAsync(emailMessage, _appSetting);
            //}

            return new MethodResult<bool>
            {
                StatusCode = StatusCodes.Status200OK,
                Result = true
            };
        }
    }
}
