using AutoMapper;
using Football247.Domain.Models.EntityModels.DTOs.Product;
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

namespace Football247.Application.Query.StoreQuery.ProductQuery
{
    public class GetProductById : IRequest<MethodResult<ProductDetailDto>>
    {
        public Guid Id { get; set; }
    }

    public class GetProductByIdHandler : IRequestHandler<GetProductById, MethodResult<ProductDetailDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetProductByIdHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }   

        public async Task<MethodResult<ProductDetailDto>> Handle(GetProductById request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request);
            var methodResult = new MethodResult<ProductDetailDto>();

            var product = await _unitOfWork.ProductRepository.GetByIdAsync(request.Id);
            if (product == null)
            {
                methodResult.AddError(StatusCodes.Status404NotFound, nameof(EnumSystemErrorCode.DataNotExist), nameof(request.Id), request.Id);
                return methodResult;
            }

            methodResult.Result = _mapper.Map<ProductDetailDto>(product);
            return methodResult;
        }
    }
}
