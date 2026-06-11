using Football247.Domain.Entities.Stores;
using Football247.Domain.IRepositories.Store;
using Football247.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Football247.Infrastructure.Repositories.Stores
{
    public class OrderRepository : Repository<Order>, IOrderRepository
    {
        private readonly Football247DbContext _db;

        public OrderRepository(Football247DbContext db) : base(db)
        {
            _db = db;
        }

        public async Task<Order> UpdateAsync(Guid id, Order order)
        {
            var existingEntity = await _db.Orders.FirstOrDefaultAsync(u => u.Id == id);
            if (existingEntity == null)
            {
                return null;
            }

            // Map DTO to Domain Model 
            existingEntity.OrderCode = order.OrderCode;
            existingEntity.UserId = order.UserId;
            existingEntity.TotalAmount = order.TotalAmount;
            existingEntity.Status = order.Status;
            existingEntity.ReceiverName = order.ReceiverName;
            existingEntity.PhoneNumber = order.PhoneNumber;
            existingEntity.ShippingAddress = order.ShippingAddress;
            existingEntity.Note = order.Note;
            existingEntity.PaymentMethod = order.PaymentMethod;
            existingEntity.PaymentStatus = order.PaymentStatus;

            await _db.SaveChangesAsync();

            return existingEntity;
        }
    }
}
