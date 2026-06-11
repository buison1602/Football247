using Football247.Repositories.IRepository;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Football247.Application.Query.StoreQuery.CartQuery
{
    public class GetCartItemCountQuery : IRequest<MethodResult<int>>
    {
        public Guid UserId { get; set; }
    }

    public class GetCartItemCountQueryHandler : IRequestHandler<GetCartItemCountQuery, MethodResult<int>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetCartItemCountQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<MethodResult<int>> Handle(GetCartItemCountQuery request, CancellationToken cancellationToken)
        {
            var methodResult = new MethodResult<int>();

            // Query đếm tổng số lượng (Quantity) siêu nhẹ.
            // Dựa vào UserId trên Cart, trỏ thẳng xuống CartItems
            var totalCount = await _unitOfWork.CartItemRepository.ReadQueryable
                .Where(ci => ci.Cart.UserId == request.UserId)
                .SumAsync(ci => ci.Quantity, cancellationToken);

            methodResult.Result = totalCount;
            return methodResult;
        }
    }
}
