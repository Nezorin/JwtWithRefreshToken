using DataAccesLayer.Entities;
using System.Linq.Expressions;

namespace DataAccesLayer
{
    public interface IDbRepository
    {
        IQueryable<T> Get<T>(Expression<Func<T, bool>> selector) where T : class, IEntity;
        IQueryable<T> GetAll<T>() where T : class, IEntity;
        Task<T> FirstOrDefaultAsync<T>(Expression<Func<T, bool>> selector) where T : class, IEntity;
        Task<int> AddAsync<T>(T newEntity) where T : class, IEntity;
        void Remove<T>(T entity) where T : class, IEntity;
        void Update<T>(T entity) where T : class, IEntity;
        Task<int> SaveChangesAsync();
    }
}
