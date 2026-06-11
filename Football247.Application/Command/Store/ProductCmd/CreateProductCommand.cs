using AutoMapper;
using Football247.Domain.Entities.Stores;
using Football247.Domain.Models.EntityModels.DTOs.Product;
using Football247.Domain.Models.EntityModels.DTOs.ProductCategory;
using Football247.Repositories.IRepository;
using Football247.Shared.Enum.ErrorCode;
using MediatR;
using Microsoft.AspNetCore.Http;
using Shared.Enum;
using Shared.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Football247.Application.Command.Store.ProductCmd
{
    public class CreateProductCommand : IRequest<MethodResult<ProductDetailDto>>
    {
        public string Name { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public decimal? SalePrice { get; set; }
        public int Stock { get; set; }
        public string? ThumbnailUrl { get; set; }
        public string? Images { get; set; }       // JSON array string: ["url1","url2"]
        public bool IsActive { get; set; } = true;
        public Guid ProductCategoryId { get; set; }
        public EnumSizeProduct Size { get; set; }
    }

    public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, MethodResult<ProductDetailDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CreateProductCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<MethodResult<ProductDetailDto>> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request);
            var methodResult = new MethodResult<ProductDetailDto>();

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
            #endregion

            var categoryDomain = _mapper.Map<Product>(request);
            if (categoryDomain == null)
            {
                methodResult.AddError(StatusCodes.Status500InternalServerError, nameof(EnumSystemErrorCode.ServerError), nameof(request));
                return methodResult;
            }

            categoryDomain = await _unitOfWork.ProductRepository.CreateAsync(categoryDomain);
            if (categoryDomain == null)
            {
                methodResult.AddError(StatusCodes.Status500InternalServerError, nameof(EnumSystemErrorCode.ServerError), nameof(request));
                return methodResult;
            }

            methodResult.StatusCode = StatusCodes.Status200OK;
            methodResult.Result = _mapper.Map<ProductDetailDto>(categoryDomain);
            return methodResult;
        }
    }
}
