using API.Models;
using System.Linq.Expressions;

namespace API.Infrastructure.Repositories
{
    public interface IRepository<T> where T : class
    {
        Task<List<T>> GetAllAsync();
        Task<T> GetByIdAsync(string id);
        Task<List<T>> SearchAsync(Expression<Func<T, bool>> predicate);
        Task<T> AddAsync(T entity);
        Task AddSearchHistoryAsync(T entity);
        //Task UpdateAsync(T entity);
        //Task DeleteAsync(int id);
    }
}
