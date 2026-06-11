using Football247.Domain.Entities.Stores;
using Football247.Domain.IRepositories.Store;
using Football247.Domain.Models.EntityModels.DTOs.Cart;
using Football247.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Football247.Infrastructure.Repositories.Stores
{
    public class CartItemRepository : Repository<CartItem>, ICartItemRepository
    {
        private readonly Football247DbContext _db;

        public CartItemRepository(Football247DbContext db) : base(db)
        {
            _db = db;
        }

        public async Task<CartItem> UpdateAsync(Guid id, CartItem cartItem)
        {
            var existingEntity = await _db.CartItems.FirstOrDefaultAsync(u => u.Id == id);
            if (existingEntity == null)
            {
                return null;
            }

            // Map DTO to Domain Model 
            existingEntity.CartId = cartItem.CartId;
            existingEntity.ProductId = cartItem.ProductId;
            existingEntity.Quantity = cartItem.Quantity;
            existingEntity.PriceAtTime = cartItem.PriceAtTime;
            existingEntity.IsDeleted = cartItem.IsDeleted;

            await _db.SaveChangesAsync();

            return existingEntity;
        }
    }
}
