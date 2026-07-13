using Football247.Application.Command.Store.ProductCmd;
using Football247.Repositories.IRepository;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PayOS;
using PayOS.Models.Webhooks;
using Shared.Enum;
using Shared.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Football247.Application.Command.ProcessWebhookCmd
{
    public class ProcessWebhookCommand : IRequest<MethodResult<bool>>
    {
        public Webhook WebhookData { get; set; }
    }

    public class ProcessWebhookCommandHandler : IRequestHandler<ProcessWebhookCommand, MethodResult<bool>>
    {
        private readonly PayOSClient _payOS;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMediator _mediator;

        public ProcessWebhookCommandHandler(PayOSClient payOS, IUnitOfWork unitOfWork, IMediator mediator)
        {
            _payOS = payOS;
            _unitOfWork = unitOfWork;
            _mediator = mediator;
        }

        public async Task<MethodResult<bool>> Handle(ProcessWebhookCommand request, CancellationToken cancellationToken)
        {
            var verifiedData = await _payOS.Webhooks.VerifyAsync(request.WebhookData);

            if (verifiedData.Code != "00")
            {
                Console.WriteLine($">>> BỎ QUA vì Code != 00");
                return new MethodResult<bool> { Result = true };
            }

            // 2. Chuyển đổi OrderCode từ long (PayOS) sang string (Entity của bạn)
            // Và dùng ReadQueryable để truy vấn trực tiếp
            var orderCodeStr = verifiedData.OrderCode.ToString();
            Console.WriteLine($">>> Tìm order: {orderCodeStr}");

            var order = await _unitOfWork.OrderRepository.Queryable
                .FirstOrDefaultAsync(o => o.OrderCode == orderCodeStr, cancellationToken);
            Console.WriteLine($">>> Order found: {order != null}");

            if (order != null)
            {
                // 3. Cập nhật trạng thái
                order.PaymentStatus = EnumPaymentStatus.Paid;
                order.Status = EnumOrderStatuss.Confirmed;

                // Trừ stock
                await _mediator.Send(new DeductStockCommand { OrderId = order.Id }, cancellationToken);

                // Xóa CartItems
                var cart = await _unitOfWork.CartRepository.Queryable
                    .Include(c => c.CartItems)
                    .FirstOrDefaultAsync(c => c.UserId == order.UserId, cancellationToken);

                if (cart != null)
                {
                    foreach (var item in cart.CartItems.ToList())
                    {
                        await _unitOfWork.CartItemRepository.DeleteAsync(item.Id);
                    }
                }

                await _unitOfWork.SaveAsync();
            }

            return new MethodResult<bool> { Result = true };
        }
    }
}