using AutoMapper;
using Football247.Domain.Models.EntityModels.DTOs.Article;
using Football247.Domain.Models.EntityModels.DTOs.Product;
using Football247.Repositories.IRepository;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.Common.Models.Paging;
using Shared.Enum;
using Shared.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Football247.Application.Query.StoreQuery.ProductQuery
{
    public class SearchProductQuery : BaseQueryModel, IRequest<MethodResult<PagingItemsModel<ProductDetailDto>>>
    {
        public Guid? ProductCategoryId { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public EnumSizeProduct? Size { get; set; }
    }

    public class SearchProductQueryHandler : IRequestHandler<SearchProductQuery, MethodResult<PagingItemsModel<ProductDetailDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public SearchProductQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<MethodResult<PagingItemsModel<ProductDetailDto>>> Handle(SearchProductQuery request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request);
            var methodResult = new MethodResult<PagingItemsModel<ProductDetailDto>>();

            var query = _unitOfWork.ProductRepository.Queryable
                .Where(p => p.IsActive);

            if (request.ProductCategoryId.HasValue)
            {
                query = query.Where(p => p.ProductCategoryId == request.ProductCategoryId.Value);
            }

            if (!string.IsNullOrEmpty(request.Keyword))
            {
                query = query.Where(p => p.Name.Contains(request.Keyword) || p.Description.Contains(request.Keyword));
            }

            if (request.MinPrice.HasValue)
            {
                query = query.Where(p => (p.SalePrice ?? p.Price) >= request.MinPrice.Value);
            }

            if (request.MaxPrice.HasValue)
            {
                query = query.Where(p => (p.SalePrice ?? p.Price) <= request.MaxPrice.Value);
            }

            if (request.Size.HasValue)
            {
                query = query.Where(p => p.Size == request.Size.Value);
            }

            var totalItems = await query.CountAsync(cancellationToken);

            var products = await query
                .OrderByDescending(a => a.CreatedDate)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);
        
            var productDtos = _mapper.Map<List<ProductDetailDto>>(products);

            methodResult.Result = new PagingItemsModel<ProductDetailDto>(productDtos, request, totalItems);
            return methodResult;
        }
    }
}
