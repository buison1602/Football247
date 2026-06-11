using Football247.Application.Service.PaymentService;
using Football247.Domain.Entities.Stores;
using Football247.Domain.Models.EntityModels.DTOs.Order;
using MediatR;
using Microsoft.AspNetCore.Http;
using Org.BouncyCastle.Asn1.X9;
using Shared.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Football247.Application.Command.PaymentCmd
{
    public class CreatePaymentLinkCommand : IRequest<MethodResult<string>>
    {
        public long OrderCode { get; set; }
        public int Amount { get; set; }
        public string Description { get; set; }
    }

    public class CreatePaymentLinkHandler : IRequestHandler<CreatePaymentLinkCommand, MethodResult<string>>
    {
        private readonly IPaymentService _paymentService;

        public CreatePaymentLinkHandler(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        public async Task<MethodResult<string>> Handle(CreatePaymentLinkCommand request, CancellationToken cancellationToken)
        {
            var methodResult = new MethodResult<string>();

            if (request.Amount <= 0)
            {
                methodResult.AddError(StatusCodes.Status400BadRequest, "InvalidAmount", "Số tiền phải lớn hơn 0");
                return methodResult;
            }

            try
            {
                // Gửi DTO thay vì Entity
                var checkoutUrl = await _paymentService.CreatePaymentAsync(new PaymentRequestDto
                {
                    OrderCode = request.OrderCode.ToString(), // Chuyển long sang string
                    TotalAmount = request.Amount,
                    Description = request.Description
                });

                methodResult.Result = checkoutUrl;
                methodResult.StatusCode = StatusCodes.Status200OK;
            }
            catch (Exception ex)
            {
                methodResult.AddError(StatusCodes.Status500InternalServerError, "PaymentError", ex.Message);
            }

            return methodResult;
        }
    }
}
