using Football247.Domain.Entities.Stores;
using Football247.Domain.Models.EntityModels.DTOs.Order;
using Microsoft.Extensions.Configuration;
using PayOS;
using PayOS.Models.V2.PaymentRequests;

namespace Football247.Application.Service.PaymentService
{
    public class PaymentService : IPaymentService
    {
        private readonly PayOSClient _payOS;
        private readonly IConfiguration _configuration;

        public PaymentService(PayOSClient payOS, IConfiguration configuration)
        {
            _payOS = payOS;
            _configuration = configuration;
        }

        public async Task<string> CreatePaymentAsync(PaymentRequestDto orderInfo)
        {
            // Đọc giá trị từ appsettings.json dựa trên môi trường
            var returnUrl = _configuration["PayOS:ReturnUrl"];
            var cancelUrl = _configuration["PayOS:CancelUrl"];

            var request = new CreatePaymentLinkRequest
            {
                OrderCode = GenerateOrderCode(orderInfo.OrderCode),
                Amount = (int)orderInfo.TotalAmount,
                Description = orderInfo.Description,
                CancelUrl = cancelUrl,
                ReturnUrl = returnUrl
            };

            var response = await _payOS.PaymentRequests.CreateAsync(request);
            return response.CheckoutUrl;
        }

        private long GenerateOrderCode(string orderCodeStr)
        {
            if (long.TryParse(orderCodeStr, out long result))
                return result;

            throw new ArgumentException($"OrderCode không hợp lệ: {orderCodeStr}");
        }

        public async Task<string> GetPaymentLinkAsync(string orderCode)
        {
            if (!long.TryParse(orderCode, out long orderCodeLong))
                throw new ArgumentException($"OrderCode không hợp lệ: {orderCode}");

            var paymentInfo = await _payOS.PaymentRequests.GetAsync(orderCodeLong);
            return $"https://pay.payos.vn/web/{paymentInfo.Id}";
        }
    }
}
