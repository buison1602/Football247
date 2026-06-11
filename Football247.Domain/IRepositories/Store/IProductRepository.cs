using Football247.Domain.Entities.Stores;
using Football247.Domain.Models.EntityModels.DTOs.Order;
using Football247.Domain.Models.EntityModels.DTOs.Product;
using Football247.Repositories.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Football247.Domain.IRepositories.Store
{
    public interface IProductRepository : IRepository<Product>
    {
        Task<Product> UpdateAsync(Guid id, Product product);
    }
}
