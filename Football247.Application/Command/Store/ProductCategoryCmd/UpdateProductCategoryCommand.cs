using AutoMapper;
using Football247.Domain.Entities.Stores;
using Football247.Domain.Models.EntityModels.DTOs.Category;
using Football247.Domain.Models.EntityModels.DTOs.ProductCategory;
using Football247.Models.Entities;
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
    public class UpdateProductCategoryCommand : IRequest<MethodResult<ProductCategoryDto>>
    {
        public Guid Id { get; set; }
        public string Name { get; set; } 
        public string Slug { get; set; } 
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
    }

    public class UpdateProductCategoryCommandHandler : IRequestHandler<UpdateProductCategoryCommand, MethodResult<ProductCategoryDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public UpdateProductCategoryCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<MethodResult<ProductCategoryDto>> Handle(UpdateProductCategoryCommand request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request);
            var methodResult = new MethodResult<ProductCategoryDto>();

            var category = await _unitOfWork.ProductCategoryRepository.GetByIdAsync(request.Id);
            if (category == null)
            {
                methodResult.AddError(StatusCodes.Status404NotFound, nameof(EnumSystemErrorCode.DataNotExist), nameof(request.Id));
                return methodResult;
            }

            var categoryDomain = _mapper.Map<ProductCategory>(request);
            await _unitOfWork.ProductCategoryRepository.UpdateAsync(request.Id, categoryDomain);
            await _unitOfWork.SaveAsync();

            methodResult.Result = _mapper.Map<ProductCategoryDto>(categoryDomain);
            return methodResult;
        }
    }
}
