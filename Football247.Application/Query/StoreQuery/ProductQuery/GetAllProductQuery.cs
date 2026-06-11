using AutoMapper;
using Football247.Domain.Models.EntityModels.DTOs.Product;
using Football247.Domain.Models.EntityModels.DTOs.ProductCategory;
using Football247.Repositories.IRepository;
using MediatR;
using Shared.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Football247.Application.Query.StoreQuery.ProductQuery
{
    public class GetAllProductQuery : IRequest<MethodResult<List<ProductDetailDto>>>
    {
    }

    public class GetAllProductQueryHandler : IRequestHandler<GetAllProductQuery, MethodResult<List<ProductDetailDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetAllProductQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<MethodResult<List<ProductDetailDto>>> Handle(GetAllProductQuery request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request);
            var methodResult = new MethodResult<List<ProductDetailDto>>();

            var products = await _unitOfWork.ProductRepository.GetAllAsync();

            methodResult.Result = _mapper.Map<List<ProductDetailDto>>(products);
            return methodResult;
        }
    }
}
