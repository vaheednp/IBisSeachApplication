using API.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace API.Services
{
    public interface IProductService
    {
        Task<List<Product>> GetAllAsync();
        Task<Product> GetByIdAsync(string id);
        Task<List<Product>> SearchAsync(string query, string sortBy);
        Task<Product> AddAsync(Product product);
        Task<List<SearchHistoryItem>> GetAllSearchHistoryByUserIdAsync(string userId);

    }
}
