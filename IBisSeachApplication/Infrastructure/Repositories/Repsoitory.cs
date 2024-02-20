using API.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace API.Infrastructure.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly TestDbContext testDbContext;
        public Repository(TestDbContext testDbContext)
        {
            this.testDbContext = testDbContext;
        }
        public async Task<List<T>> GetAllAsync()
        {
            return await testDbContext.Set<T>().ToListAsync();
        }

        public async Task<T> GetByIdAsync(string id)
        {
            return await testDbContext.Set<T>().FindAsync(id);
        }
        public async Task<List<T>> SearchAsync(Expression<Func<T, bool>> predicate)
        {
            return await testDbContext.Set<T>().Where(predicate).ToListAsync();
        }

        public async Task<T> AddAsync(T entity)
        {
            await testDbContext.Set<T>().AddAsync(entity);
            await testDbContext.SaveChangesAsync();
            return entity;
        }
        public async Task AddSearchHistoryAsync(T entity)
        {
            await testDbContext.Set<T>().AddAsync(entity);
            await testDbContext.SaveChangesAsync();
        }

        //public async Task UpdateAsync(T entity)
        //{
        //    testDbContext.Set<T>().Update(entity);
        //    await testDbContext.SaveChangesAsync();
        //}

        //public async Task DeleteAsync(int id)
        //{
        //    var entity = await GetByIdAsync(id);
        //    if (entity != null)
        //    {
        //        testDbContext.Set<T>().Remove(entity);
        //        await testDbContext.SaveChangesAsync();
        //    }
        //}
    }
}
