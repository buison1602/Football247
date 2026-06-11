using Football247.Domain.Models.CommandModels.ArticleCmdModel;
using Football247.Repositories.IRepository;
using Football247.Shared.Enum.ErrorCode;
using MediatR;
using Microsoft.AspNetCore.Http;
using Shared.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Football247.Application.Command.ArticleCmd
{
    public class DeleteArticleCommand : DeleteArticleCommandModel, IRequest<MethodResult<bool>>
    {
    }

    public class DeleteArticleHandler : IRequestHandler<DeleteArticleCommand, MethodResult<bool>>
        {
            private readonly IUnitOfWork _unitOfWork;
    
            public DeleteArticleHandler(IUnitOfWork unitOfWork)
            {
                _unitOfWork = unitOfWork;
            }
    
            public async Task<MethodResult<bool>> Handle(DeleteArticleCommand request, CancellationToken cancellationToken)
            {
                ArgumentNullException.ThrowIfNull(request);
                var methodResult = new MethodResult<bool>();
    
                var article = await _unitOfWork.ArticleRepository.GetByIdAsync(request.Id);
                if (article == null)
                {
                    methodResult.AddError(StatusCodes.Status404NotFound, nameof(EnumSystemErrorCode.DataNotExist), nameof(request.Id), request.Id);
                    return methodResult;
                }
    
                await _unitOfWork.ArticleRepository.DeleteAsync(request.Id);

                methodResult.Result = true;
                return methodResult;
            }
    }
}
