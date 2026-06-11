using AutoMapper;
using Football247.Domain.Models.CommandModels.TagCmdModel;
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

namespace Football247.Application.Command.TagCmd
{
    public class DeleteTagCommand : DeleteTagCommandModel, IRequest<MethodResult<bool>>
    {
    }

    public class DeleteTagHandler : IRequestHandler<DeleteTagCommand, MethodResult<bool>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeleteTagHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<MethodResult<bool>> Handle(DeleteTagCommand request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request);
            var methodResult = new MethodResult<bool>();

            var tag = await _unitOfWork.TagRepository.GetByIdAsync(request.Id);
            if (tag is null)
            {
                methodResult.AddError(StatusCodes.Status404NotFound, nameof(EnumSystemErrorCode.DataNotExist), nameof(request.Id), request.Id);
                return methodResult;
            }

            await _unitOfWork.TagRepository.DeleteAsync(request.Id);
            await _unitOfWork.SaveAsync();

            methodResult.Result = true;
            return methodResult;
        }
    }
}