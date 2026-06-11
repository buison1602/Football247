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
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        private readonly Football247DbContext _db;

        public ProductRepository(Football247DbContext db) : base(db)
        {
            _db = db;
        }

        public async Task<Product> UpdateAsync(Guid id, Product product)
        {
            var existingEntity = await _db.Products.FirstOrDefaultAsync(u => u.Id == id);
            if (existingEntity == null)
            {
                return null;
            }

            // Map DTO to Domain Model 
            existingEntity.Name = product.Name;
            existingEntity.Slug = product.Slug;
            existingEntity.Description = product.Description;
            existingEntity.Price = product.Price;
            existingEntity.SalePrice = product.SalePrice;
            existingEntity.Stock = product.Stock;
            existingEntity.ThumbnailUrl = product.ThumbnailUrl;
            existingEntity.Images = product.Images;
            existingEntity.IsActive = product.IsActive;
            existingEntity.ProductCategoryId = product.ProductCategoryId;
            existingEntity.Size = product.Size;

            await _db.SaveChangesAsync();

            return existingEntity;
        }
    }
}
