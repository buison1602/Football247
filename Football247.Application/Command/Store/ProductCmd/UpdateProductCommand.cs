using AutoMapper;
using Football247.Application.Command.Store.ProductCategoryCmd;
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
    public class UpdateProductCommand : IRequest<MethodResult<ProductDetailDto>>
    {
        public Guid Id { get; set; }
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

    public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, MethodResult<ProductDetailDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UpdateProductCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<MethodResult<ProductDetailDto>> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request);
            var methodResult = new MethodResult<ProductDetailDto>();

            var category = await _unitOfWork.ProductRepository.GetByIdAsync(request.Id);
            if (category == null)
            {
                methodResult.AddError(StatusCodes.Status404NotFound, nameof(EnumSystemErrorCode.DataNotExist), nameof(request.Id));
                return methodResult;
            }

            var productDomain = _mapper.Map<Product>(request);
            await _unitOfWork.ProductRepository.UpdateAsync(request.Id, productDomain);
            await _unitOfWork.SaveAsync();

            methodResult.Result = _mapper.Map<ProductDetailDto>(productDomain);
            return methodResult;
        }
    }
}
