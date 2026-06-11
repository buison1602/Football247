using AutoMapper;
using Football247.Domain.Models.CommandModels.CategoryCmdModel;
using Football247.Domain.Models.EntityModels.DTOs.Article;
using Football247.Domain.Models.EntityModels.DTOs.Category;
using Football247.Models.Entities;
using Football247.Repositories.IRepository;
using Football247.Shared.Enum.ErrorCode;
using MediatR;
using Microsoft.AspNetCore.Http;
using Shared.Response;

namespace Football247.Application.Command.CategoryCmd
{
    public class UpdateCategoryCommand : UpdateCategoryCommandModel, IRequest<MethodResult<CategoryDto>>
    {
    }

    public class UpdateCategoryHandler : IRequestHandler<UpdateCategoryCommand, MethodResult<CategoryDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UpdateCategoryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<MethodResult<CategoryDto>> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request);
            var methodResult = new MethodResult<CategoryDto>();

            #region Validation
            if (string.IsNullOrEmpty(request.Name))
            {
                methodResult.AddError(StatusCodes.Status400BadRequest, nameof(EnumSystemErrorCode.Required), nameof(request.Name));
                return methodResult;
            }

            if (string.IsNullOrEmpty(request.Slug))
            {
                methodResult.AddError(StatusCodes.Status400BadRequest, nameof(EnumSystemErrorCode.Required), nameof(request.Slug));
                return methodResult;
            }

            var category = await _unitOfWork.CategoryRepository.GetByIdAsync(request.Id);

            if (category == null)
            {
                methodResult.AddError(StatusCodes.Status404NotFound, nameof(EnumSystemErrorCode.DataNotExist), nameof(request.Id), request.Id);
                return methodResult;
            }
            #endregion

            var categoryDomain = _mapper.Map<Category>(request);
            await _unitOfWork.CategoryRepository.UpdateAsync(request.Id, categoryDomain);
            await _unitOfWork.SaveAsync();

            methodResult.Result = _mapper.Map<CategoryDto>(categoryDomain);
            return methodResult;
        }
    }
}
