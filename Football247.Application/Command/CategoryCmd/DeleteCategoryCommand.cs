using Football247.Domain.Models.CommandModels.CategoryCmdModel;
using Football247.Repositories.IRepository;
using Football247.Shared.Enum.ErrorCode;
using MediatR;
using Microsoft.AspNetCore.Http;
using Shared.Response;

namespace Football247.Application.Command.CategoryCmd
{
    public class DeleteCategoryCommand : DeleteCategoryCommandModel, IRequest<MethodResult<bool>>
    {
    }

    public class DeleteCategoryHandler : IRequestHandler<DeleteCategoryCommand, MethodResult<bool>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeleteCategoryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<MethodResult<bool>> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request);
            var methodResult = new MethodResult<bool>();
            var category = await _unitOfWork.CategoryRepository.GetByIdAsync(request.Id);

            if (category == null)
            {
                methodResult.AddError(StatusCodes.Status404NotFound, nameof(EnumSystemErrorCode.DataNotExist), nameof(request.Id), request.Id);
                return methodResult;
            }

            await _unitOfWork.CategoryRepository.DeleteAsync(request.Id);
            await _unitOfWork.SaveAsync();
            methodResult.Result = true;
            return methodResult;
        }
    }
}