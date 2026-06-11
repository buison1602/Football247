using Football247.Domain.Entities.Stores;
using Football247.Domain.Models.EntityModels.DTOs.Order;
using Org.BouncyCastle.Asn1.X9;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Football247.Application.Service.PaymentService
{
    public interface IPaymentService
    {
        Task<string> CreatePaymentAsync(PaymentRequestDto orderInfo);
        Task<string> GetPaymentLinkAsync(string orderCode);
    }
}
