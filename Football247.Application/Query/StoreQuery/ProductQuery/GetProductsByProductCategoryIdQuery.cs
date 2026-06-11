using AutoMapper;
using Football247.Domain.Models.EntityModels.DTOs.Product;
using Football247.Repositories.IRepository;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Football247.Application.Query.StoreQuery.ProductQuery
{
    public class GetProductsByProductCategoryIdQuery : IRequest<MethodResult<List<ProductDetailDto>>>
    {
        public Guid ProductCategoryId { get; set; }
    }

    public class GetProductsByProductCategoryIdQueryHandler : IRequestHandler<GetProductsByProductCategoryIdQuery, MethodResult<List<ProductDetailDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetProductsByProductCategoryIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<MethodResult<List<ProductDetailDto>>> Handle(GetProductsByProductCategoryIdQuery request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request);
            var methodResult = new MethodResult<List<ProductDetailDto>>();

            var products = await _unitOfWork.ProductRepository.ReadQueryable
                .Where(p => p.ProductCategoryId == request.ProductCategoryId)
                .ToListAsync(cancellationToken);

            methodResult.Result = _mapper.Map<List<ProductDetailDto>>(products);
            return methodResult;
        }
    }
}
