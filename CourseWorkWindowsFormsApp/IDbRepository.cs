using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace CourseWorkWindowsFormsApp
{
    internal interface IDbRepository<T> where T : class
    {
        IQueryable<T> Get(Expression<Func<T, bool>> selector);
        IQueryable<T> Get();
        IQueryable<T> GetAll();

        Task<Guid> Add(T newEntity);
        Task AddRange(IEnumerable<T> newEntities);

        Task Delete(Guid entity);

        Task Remove(T entity);
        Task RemoveRange(IEnumerable<T> entities);

        Task Update(T entity);
        Task UpdateRange(IEnumerable<T> entities);

        Task<int> SaveChangesAsync();
    }
}
