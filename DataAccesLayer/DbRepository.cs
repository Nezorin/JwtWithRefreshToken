using DataAccesLayer.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace DataAccesLayer
{
    public class DbRepository(DataContext context) : IDbRepository
    {
        private readonly DataContext _context = context;

        public IQueryable<T> GetAll<T>() where T : class, IEntity
        {
            return _context.Set<T>().AsQueryable();
        }
        public IQueryable<T> Get<T>(Expression<Func<T, bool>> selector) where T : class, IEntity
        {
            return _context.Set<T>().Where(selector).AsQueryable();
        }
        public async Task<T> FirstOrDefaultAsync<T>(Expression<Func<T, bool>> selector) where T : class, IEntity
        {
            return await _context.Set<T>().FirstOrDefaultAsync(selector);
        }
        public async Task<int> AddAsync<T>(T newEntity) where T : class, IEntity
        {
            var entity = await _context.Set<T>().AddAsync(newEntity);
            return entity.Entity.Id;
        }
        public void Remove<T>(T entity) where T : class, IEntity
        {
            _context.Set<T>().Remove(entity);
        }
        public void Update<T>(T entity) where T : class, IEntity
        {
            _context.Set<T>().Update(entity);
        }
        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
