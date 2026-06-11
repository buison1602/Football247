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

namespace Football247.Application.Command.Store.ProductCategoryCmd
{
    public class DeleteProductCategoryCommand : IRequest<MethodResult<bool>>
    {
        public Guid Id { get; set; }
    }

    public class DeleteProductCategoryCommandHandler : IRequestHandler<DeleteProductCategoryCommand, MethodResult<bool>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public DeleteProductCategoryCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<MethodResult<bool>> Handle(DeleteProductCategoryCommand request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request);
            var methodResult = new MethodResult<bool>();
            var category = await _unitOfWork.ProductCategoryRepository.GetByIdAsync(request.Id);
            if (category == null)
            {
                methodResult.AddError(StatusCodes.Status404NotFound, nameof(EnumSystemErrorCode.DataNotExist), nameof(request.Id), request.Id);
                return methodResult;
            }
            await _unitOfWork.ProductCategoryRepository.DeleteAsync(request.Id);
            await _unitOfWork.SaveAsync();
            methodResult.Result = true;
            return methodResult;
        }
    }
}
