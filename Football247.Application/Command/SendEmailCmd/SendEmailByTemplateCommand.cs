using Football247.Application.Helper;
using Football247.Domain.Models.CommandModels.SendEmailCmdModel;
using MediatR;
using Microsoft.AspNetCore.Http;
using Shared.Response;


namespace Football247.Application.Command.SendEmailCmd
{
    public class SendEmailByTemplateCommand : SendEmailCommandModel, IRequest<MethodResult<bool>>
    {
    }

    public class SendEmailByTemplateCommandHandler : IRequestHandler<SendEmailByTemplateCommand, MethodResult<bool>>
    {
        private readonly IMediator _mediator;

        public SendEmailByTemplateCommandHandler(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<MethodResult<bool>> Handle(SendEmailByTemplateCommand request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request);
            MethodResult<bool> methodResult = new MethodResult<bool>();

            #region Validation

            if (request.Template == null || request.Params == null)
            {
                methodResult.Result = false;
                return methodResult;
            }

            var baseDir = AppDomain.CurrentDomain.BaseDirectory;
            var path = Path.Combine(baseDir, "Resource", $"{request.Template}.html");

            // Nếu chạy Debug ở Visual Studio (Local), thư mục Resource có thể nằm ở thư mục gốc Project
            if (!File.Exists(path))
            {
                var projectDirectory = Directory.GetParent(baseDir)?.Parent?.Parent?.Parent?.FullName;
                if (projectDirectory != null)
                {
                    path = Path.Combine(projectDirectory, "Resource", $"{request.Template}.html");
                }
            }

            if (!File.Exists(path))
            {
                Console.WriteLine("LỖI: File template không tồn tại tại đường dẫn trên!");
                return methodResult;
            }

            using StreamReader streamReader = new StreamReader(path);
            Console.WriteLine("\n\n check lan 0 \n\n\n");

            var body = await streamReader.ReadToEndAsync(cancellationToken);

            var @params = ObjectHelper.GetDictionary(request.Params);
            if (@params != null)
            {
                foreach (var item in @params)
                {
                    body = body.Replace($"[{item.Key}]", item.Value, StringComparison.CurrentCultureIgnoreCase);
                }
            }

            methodResult = await _mediator.Send(new SendEmailCommand
            {
                Subject = request.Subject,
                ToEmails = request.ToEmails,
                Content = body,
                Params = request.Params,
                Template = request.Template,
            }, cancellationToken);

            #endregion Validation

            methodResult.StatusCode = StatusCodes.Status200OK;
            methodResult.Result = true;
            return methodResult;
        }
    }
}
