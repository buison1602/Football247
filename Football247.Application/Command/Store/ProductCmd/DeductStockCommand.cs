using Football247.Repositories.IRepository;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Shared.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Football247.Application.Command.Store.ProductCmd
{
    public class DeductStockCommand : IRequest<MethodResult<bool>>
    {
        public Guid OrderId { get; set; }
    }

    public class DeductStockCommandHandler : IRequestHandler<DeductStockCommand, MethodResult<bool>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeductStockCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<MethodResult<bool>> Handle(DeductStockCommand request, CancellationToken cancellationToken)
        {
            var methodResult = new MethodResult<bool>();

            var order = await _unitOfWork.OrderRepository.Queryable
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(o => o.Id == request.OrderId, cancellationToken);

            if (order == null)
            {
                methodResult.AddError(StatusCodes.Status404NotFound, "OrderNotFound", "Không tìm thấy đơn hàng");
                return methodResult;
            }

            foreach (var item in order.OrderItems)
            {
                if (item.Product == null) continue;

                if (item.Product.Stock < item.Quantity)
                {
                    methodResult.AddError(StatusCodes.Status400BadRequest, "OutOfStock",
                        $"Sản phẩm '{item.Product.Name}' không đủ hàng");
                    return methodResult;
                }

                item.Product.Stock -= item.Quantity;
            }

            await _unitOfWork.SaveAsync();
            methodResult.Result = true;
            return methodResult;
        }
    }
}
