using Football247.Data;
using Football247.Repositories.IRepository;
using Microsoft.EntityFrameworkCore;

namespace Football247.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly Football247DbContext _db;
        private readonly DbSet<T> _dbSet;

        public Repository(Football247DbContext db)
        {
            _db = db;
            _dbSet = db.Set<T>();
        }

        public virtual async Task<T> CreateAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
            await _db.SaveChangesAsync();
            return entity;
        }

        public virtual async Task<T?> DeleteAsync(Guid id)
        {
            var existingEntity = await _dbSet.FindAsync(id);
            if (existingEntity == null)
            {
                return null;
            } 
            _dbSet.Remove(existingEntity);
            await _db.SaveChangesAsync();

            return existingEntity;
        }

        public virtual async Task<List<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task<T> GetByIdAsync(Guid id)
        {
            return await _dbSet.FindAsync(id);
        }
    }
}
