using AutoMapper;
using Football247.Domain.Models.CommandModels.CategoryCmdModel;
using Football247.Domain.Models.EntityModels.DTOs.Category;
using Football247.Models.Entities;
using Football247.Repositories.IRepository;
using Football247.Shared.Enum.ErrorCode;
using MediatR;
using Microsoft.AspNetCore.Http;
using Shared.Response;

namespace Football247.Application.Command.CategoryCmd
{
    public class CreateCategoryCommand : CreateCategoryCommandModel, IRequest<MethodResult<CategoryDto>>
    {
    }

    public class CreateCategoryHandler : IRequestHandler<CreateCategoryCommand, MethodResult<CategoryDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CreateCategoryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<MethodResult<CategoryDto>> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
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

            var category = await _unitOfWork.CategoryRepository.GetBySlugAsync(request.Slug);

            if (category != null)
            {
                methodResult.AddError(StatusCodes.Status400BadRequest, nameof(EnumSystemErrorCode.DataAlreadyExist), nameof(request.Slug), request.Slug);
                return methodResult;
            }
            #endregion

            var categoryDomain = _mapper.Map<Category>(request);
            if (categoryDomain == null) 
            {
                methodResult.AddError(StatusCodes.Status500InternalServerError, nameof(EnumSystemErrorCode.ServerError), nameof(request));
                return methodResult;
            }

            categoryDomain = await _unitOfWork.CategoryRepository.CreateAsync(categoryDomain);
            if (categoryDomain == null)
            {
                methodResult.AddError(StatusCodes.Status500InternalServerError, nameof(EnumSystemErrorCode.ServerError), nameof(request));
                return methodResult;
            }

            methodResult.StatusCode = StatusCodes.Status200OK;
            methodResult.Result = _mapper.Map<CategoryDto>(categoryDomain);
            return methodResult;
        }
    }
}
