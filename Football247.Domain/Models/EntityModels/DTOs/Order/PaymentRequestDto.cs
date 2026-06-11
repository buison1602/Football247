using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Football247.Domain.Models.EntityModels.DTOs.Order
{
    public class PaymentRequestDto
    {
        public string OrderCode { get; set; }
        public decimal TotalAmount { get; set; }
        public string Description { get; set; }
    }
}
