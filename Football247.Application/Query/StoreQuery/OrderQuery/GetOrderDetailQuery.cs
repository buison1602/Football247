using Football247.Domain.Entities.Stores;
using Football247.Domain.Models.EntityModels.DTOs.Order;
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

namespace Football247.Application.Query.StoreQuery.OrderQuery
{
    public class GetOrderDetailQuery : IRequest<MethodResult<OrderDetailDto>>
    {
        //public Guid UserId { get; set; }
        public Guid OrderId { get; set; }
    }

    public class GetOrderDetailQueryHandler : IRequestHandler<GetOrderDetailQuery, MethodResult<OrderDetailDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetOrderDetailQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<MethodResult<OrderDetailDto>> Handle(GetOrderDetailQuery request, CancellationToken cancellationToken)
        {
            var methodResult = new MethodResult<OrderDetailDto>();
            var order = await _unitOfWork.OrderRepository.ReadQueryable
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.Id == request.OrderId , cancellationToken); // && o.UserId == request.UserId

            if (order == null)
            {
                methodResult.AddError(StatusCodes.Status404NotFound, "NotFound", "Không tìm thấy đơn hàng");
                return methodResult;
            }

            methodResult.Result = MapToDetail(order);
            methodResult.StatusCode = StatusCodes.Status200OK;
            return methodResult;
        }

        private static OrderDetailDto MapToDetail(Order o) => new()
        {
            Id = o.Id,
            OrderCode = o.OrderCode,
            TotalAmount = o.TotalAmount,
            Status = o.Status.ToString(),
            PaymentStatus = o.PaymentStatus.ToString(),
            PaymentMethod = o.PaymentMethod.ToString(),
            CreatedDate = o.CreatedDate,
            ItemCount = o.OrderItems.Count,
            ReceiverName = o.ReceiverName,
            PhoneNumber = o.PhoneNumber,
            ShippingAddress = o.ShippingAddress,
            Note = o.Note,
            Items = o.OrderItems.Select(i => new OrderItemDto
            {
                ProductId = i.ProductId,
                ProductName = i.ProductName,
                ProductThumbnail = i.ProductThumbnail,
                Quantity = i.Quantity,
                Price = i.Price,
                TotalPrice = i.TotalPrice
            }).ToList()
        };
    }
}
