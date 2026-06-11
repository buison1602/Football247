using Football247.Domain.Entities.Stores;
using Football247.Domain.Models.EntityModels.DTOs.Cart;
using Football247.Domain.Models.EntityModels.DTOs.Order;
using Football247.Repositories.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Football247.Domain.IRepositories.Store
{
    public interface IOrderRepository : IRepository<Order>
    {
        Task<Order> UpdateAsync(Guid id, Order order);
    }
}
