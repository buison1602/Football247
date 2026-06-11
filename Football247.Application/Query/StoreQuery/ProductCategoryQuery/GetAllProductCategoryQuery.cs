using AutoMapper;
using Football247.Application.Query.CategoryQuery;
using Football247.Domain.Models.EntityModels.DTOs.Category;
using Football247.Domain.Models.EntityModels.DTOs.ProductCategory;
using Football247.Repositories.IRepository;
using MediatR;
using Shared.Response;

namespace Football247.Application.Query.StoreQuery.ProductCategoryQuery
{
    public class GetAllProductCategoryQuery : IRequest<MethodResult<List<ProductCategoryDto>>>
    {
    }

    public class GetAllProductCategoryQueryHandler : IRequestHandler<GetAllProductCategoryQuery, MethodResult<List<ProductCategoryDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetAllProductCategoryQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<MethodResult<List<ProductCategoryDto>>> Handle(GetAllProductCategoryQuery request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request);
            var methodResult = new MethodResult<List<ProductCategoryDto>>();
            
            var productCategories = await _unitOfWork.ProductCategoryRepository.GetAllAsync();

            methodResult.Result = _mapper.Map<List<ProductCategoryDto>>(productCategories);
            return methodResult;
        }
    }
}