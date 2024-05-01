using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace CourseWorkWindowsFormsApp
{
    internal class EmployeeRepository<T> : IDbRepository<T> where T : class
    {
        private readonly DbContext _dbContext;

        public EmployeeRepository(DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IQueryable<T> Get(Expression<Func<T, bool>> selector)
        {
            return _dbContext.Set<T>().Where(selector);
        }

        public IQueryable<T> Get()
        {
            return _dbContext.Set<T>();
        }

        public IQueryable<T> GetAll()
        {
            return _dbContext.Set<T>().AsQueryable();
        }

        public async Task<Guid> Add(T newEntity)
        {
            await _dbContext.Set<T>().AddAsync(newEntity);
            await _dbContext.SaveChangesAsync();
            return Guid.NewGuid();
        }

        public async Task AddRange(IEnumerable<T> newEntities)
        {
            await _dbContext.Set<T>().AddRangeAsync(newEntities);
            await _dbContext.SaveChangesAsync();
        }

        public async Task Delete(Guid entity)
        {
            var entityToDelete = await _dbContext.Set<T>().FindAsync(entity);
            if (entityToDelete != null)
            {
                _dbContext.Set<T>().Remove(entityToDelete);
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task Remove(T entity)
        {
            _dbContext.Set<T>().Remove(entity);
            await _dbContext.SaveChangesAsync();
        }

        public async Task RemoveRange(IEnumerable<T> entities)
        {
            _dbContext.Set<T>().RemoveRange(entities);
            await _dbContext.SaveChangesAsync();
        }

        public async Task Update(T entity)
        {
            _dbContext.Set<T>().Update(entity);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateRange(IEnumerable<T> entities)
        {
            _dbContext.Set<T>().UpdateRange(entities);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _dbContext.SaveChangesAsync();
        }
    }
}
