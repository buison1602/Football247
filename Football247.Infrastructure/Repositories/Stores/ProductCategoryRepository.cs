using Football247.Domain.Entities.Stores;
using Football247.Domain.IRepositories.Store;
using Football247.Domain.Models.EntityModels.DTOs.ProductCategory;
using Football247.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Football247.Infrastructure.Repositories.Stores
{
    public class ProductCategoryRepository : Repository<ProductCategory>, IProductCategoryRepository
    {
        private readonly Football247DbContext _db;

        public ProductCategoryRepository(Football247DbContext db) : base(db)
        {
            _db = db;
        }

        public async Task<ProductCategory> UpdateAsync(Guid id, ProductCategory productCategory)
        {
            var existingEntity = await _db.ProductCategories.FirstOrDefaultAsync(u => u.Id == id);
            if (existingEntity == null)
            {
                return null;
            }

            // Map DTO to Domain Model 
            existingEntity.Name = productCategory.Name;
            existingEntity.Slug = productCategory.Slug;
            existingEntity.Description = productCategory.Description;
            existingEntity.ImageUrl = productCategory.ImageUrl;
            existingEntity.ParentCategoryId = productCategory.ParentCategoryId;

            await _db.SaveChangesAsync();

            return existingEntity;
        }
    }
}
