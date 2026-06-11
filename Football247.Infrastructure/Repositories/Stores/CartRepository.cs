using Football247.Domain.Entities.Stores;
using Football247.Domain.IRepositories.Store;
using Football247.Domain.Models.EntityModels.DTOs.Cart;
using Football247.Domain.Models.EntityModels.DTOs.Category;
using Football247.Models.Entities;
using Football247.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Football247.Infrastructure.Repositories.Stores
{
    public class CartRepository : Repository<Cart>, ICartRepository
    {
        private readonly Football247DbContext _db;

        public CartRepository(Football247DbContext db) : base(db)
        {
            _db = db;
        }

        public async Task<Cart> UpdateAsync(Guid id, Cart cart)
        {
            var existingEntity = await _db.Carts.FirstOrDefaultAsync(u => u.Id == id);
            if (existingEntity == null)
            {
                return null;
            }

            // Map DTO to Domain Model 
            existingEntity.UserId = cart.UserId;

            await _db.SaveChangesAsync();

            return existingEntity;
        }
    }
}
