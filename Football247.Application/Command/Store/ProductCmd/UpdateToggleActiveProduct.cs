using AutoMapper;
using Football247.Domain.Models.EntityModels.DTOs.Product;
using Football247.Repositories.IRepository;
using Football247.Shared.Enum.ErrorCode;
using MediatR;
using Microsoft.AspNetCore.Http;
using Shared.Response;

namespace Football247.Application.Command.Store.ProductCmd
{
    public class UpdateToggleActiveProduct : IRequest<MethodResult<ProductDetailDto>>
    {
        public Guid Id { get; set; }
        public bool IsActive { get; set; }
    }

    public class UpdateToggleActiveProductHandler : IRequestHandler<UpdateToggleActiveProduct, MethodResult<ProductDetailDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public UpdateToggleActiveProductHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<MethodResult<ProductDetailDto>> Handle(UpdateToggleActiveProduct request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request);
            var methodResult = new MethodResult<ProductDetailDto>();
            var product = await _unitOfWork.ProductRepository.GetByIdAsync(request.Id);
            
            if (product == null)
            {
                methodResult.AddError(StatusCodes.Status404NotFound, nameof(EnumSystemErrorCode.DataNotExist), nameof(request.Id));
                return methodResult;
            }

            product.IsActive = request.IsActive;
            await _unitOfWork.ProductRepository.UpdateAsync(request.Id, product);
            await _unitOfWork.SaveAsync();
            
            methodResult.Result = _mapper.Map<ProductDetailDto>(product);
            return methodResult;
        }
    }
}
