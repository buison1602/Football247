using Football247.Application.Service.PaymentService;
using Football247.Domain.Entities.Stores;
using Football247.Domain.Models.EntityModels.DTOs.Order;
using Football247.Repositories.IRepository;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Shared.Enum;
using Shared.Response;

namespace Football247.Application.Command.Store.OrderCmd
{
    public class CheckoutCommand : IRequest<MethodResult<string>>
    {
        public Guid UserId { get; set; }
        public string ReceiverName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string ShippingAddress { get; set; } = string.Empty;
        public string? Note { get; set; }
        public EnumPaymentMethodd PaymentMethod { get; set; } = EnumPaymentMethodd.Online;
    }

    public class CheckoutCommandHandler : IRequestHandler<CheckoutCommand, MethodResult<string>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPaymentService _paymentService;

        public CheckoutCommandHandler(IUnitOfWork unitOfWork, IPaymentService paymentService)
        {
            _unitOfWork = unitOfWork;
            _paymentService = paymentService;
        }

        public async Task<MethodResult<string>> Handle(CheckoutCommand request, CancellationToken cancellationToken)
        {
            var methodResult = new MethodResult<string>();

            // 1. Lấy Cart + CartItems + Product của user
            var cart = await _unitOfWork.CartRepository.Queryable
                .Include(c => c.CartItems)
                    .ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync(c => c.UserId == request.UserId, cancellationToken);

            if (cart == null || !cart.CartItems.Any())
            {
                methodResult.AddError(StatusCodes.Status400BadRequest, "EmptyCart", "Giỏ hàng trống");
                return methodResult;
            }

            // 2. Kiểm tra tồn kho
            foreach (var item in cart.CartItems)
            {
                if (item.Product == null || item.Product.Stock < item.Quantity)
                {
                    methodResult.AddError(StatusCodes.Status400BadRequest, "OutOfStock",
                        $"Sản phẩm '{item.Product?.Name ?? item.ProductId.ToString()}' không đủ hàng");
                    return methodResult;
                }
            }

            // Sau bước kiểm tra tồn kho, trước BeginTransaction

            // Kiểm tra xem user có đơn hàng Online chưa thanh toán không
            // ✅ 3. Check đơn Unpaid cũ — trả lại link thay vì tạo mới
            if (request.PaymentMethod == EnumPaymentMethodd.Online)
            {
                var existingUnpaidOrder = await _unitOfWork.OrderRepository.Queryable
                    .FirstOrDefaultAsync(o =>
                        o.UserId == request.UserId &&
                        o.PaymentStatus == EnumPaymentStatus.Unpaid &&
                        o.PaymentMethod == EnumPaymentMethodd.Online,
                        cancellationToken);

                if (existingUnpaidOrder != null)
                {
                    // Lấy lại checkout URL từ PayOS thay vì tạo mới
                    try
                    {
                        var existingPaymentInfo = await _paymentService.GetPaymentLinkAsync(existingUnpaidOrder.OrderCode);
                        methodResult.Result = existingPaymentInfo; // checkoutUrl
                        methodResult.StatusCode = StatusCodes.Status200OK;
                        return methodResult;
                    }
                    catch
                    {
                        // Nếu link PayOS đã expired thì hủy đơn cũ, tạo đơn mới bên dưới
                        existingUnpaidOrder.Status = EnumOrderStatuss.Cancelled;
                        await _unitOfWork.SaveAsync();
                    }
                }
            }

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                // 3. Tạo OrderCode
                var orderCodeNumber = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(); // long, unique
                var orderCode = orderCodeNumber.ToString();
                var totalAmount = cart.CartItems.Sum(ci => ci.PriceAtTime * ci.Quantity);

                // 4. Tạo Order
                var order = new Order
                {
                    OrderCode = orderCode,
                    UserId = request.UserId,
                    TotalAmount = totalAmount,
                    ReceiverName = request.ReceiverName,
                    PhoneNumber = request.PhoneNumber,
                    ShippingAddress = request.ShippingAddress,
                    Note = request.Note,
                    PaymentMethod = request.PaymentMethod,
                    Status = EnumOrderStatuss.Pending,
                    PaymentStatus = EnumPaymentStatus.Unpaid,
                    OrderItems = cart.CartItems.Select(ci => new OrderItem
                    {
                        ProductId = ci.ProductId,
                        ProductName = ci.Product!.Name,
                        ProductThumbnail = ci.Product.ThumbnailUrl,
                        Quantity = ci.Quantity,
                        Price = ci.PriceAtTime
                    }).ToList()
                };

                await _unitOfWork.OrderRepository.CreateAsync(order);

                // 5. Trừ tồn kho
                foreach (var item in cart.CartItems)
                {
                    item.Product!.Stock -= item.Quantity;
                }

                await _unitOfWork.SaveAsync();
                await _unitOfWork.CommitTransactionAsync();

                // 7. Gọi PayOS nếu thanh toán Online
                if (request.PaymentMethod == EnumPaymentMethodd.Online)
                {
                    var checkoutUrl = await _paymentService.CreatePaymentAsync(new PaymentRequestDto
                    {
                        OrderCode = orderCodeNumber.ToString(),
                        TotalAmount = (int)totalAmount,
                        Description = $"Thanh toan {orderCodeNumber}"
                    });
                    methodResult.Result = checkoutUrl;
                }
                else
                {
                    // COD: trả về OrderCode để FE redirect trang thành công
                    methodResult.Result = orderCode;
                }

                methodResult.StatusCode = StatusCodes.Status200OK;
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                methodResult.AddError(StatusCodes.Status500InternalServerError, "CheckoutError", ex.Message);
            }

            return methodResult;
        }
    }
}
