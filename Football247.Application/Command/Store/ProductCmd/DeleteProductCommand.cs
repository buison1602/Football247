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

namespace Football247.Application.Command.Store.ProductCmd
{
    public class DeleteProductCommand : IRequest<MethodResult<bool>>
    {
        public Guid Id { get; set; }
    }

    public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand, MethodResult<bool>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public DeleteProductCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<MethodResult<bool>> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request);
            var methodResult = new MethodResult<bool>();
            var product = await _unitOfWork.ProductRepository.GetByIdAsync(request.Id);
            if (product == null)
            {
                methodResult.AddError(StatusCodes.Status404NotFound, nameof(EnumSystemErrorCode.DataNotExist), nameof(request.Id), request.Id);
                return methodResult;
            }
            await _unitOfWork.ProductRepository.DeleteAsync(request.Id);
            await _unitOfWork.SaveAsync();
            methodResult.Result = true;
            return methodResult;
        }
    }
}

