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
    public class OrderItemRepository : Repository<OrderItem>, IOrderItemRepository
    {
        private readonly Football247DbContext _db;

        public OrderItemRepository(Football247DbContext db) : base(db)
        {
            _db = db;
        }

        public async Task<OrderItem> UpdateAsync(Guid id, OrderItem orderItem)
        {
            var existingEntity = await _db.OrderItems.FirstOrDefaultAsync(u => u.Id == id);
            if (existingEntity == null)
            {
                return null;
            }

            // Map DTO to Domain Model 
            existingEntity.OrderId = orderItem.OrderId;
            existingEntity.ProductId = orderItem.ProductId;
            existingEntity.ProductName = orderItem.ProductName;
            existingEntity.ProductThumbnail = orderItem.ProductThumbnail;
            existingEntity.Quantity = orderItem.Quantity;
            existingEntity.Price = orderItem.Price;

            await _db.SaveChangesAsync();

            return existingEntity;
        }
    }
}
