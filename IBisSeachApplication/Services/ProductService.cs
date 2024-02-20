using API.Infrastructure;
using API.Infrastructure.Repositories;
using API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.Configuration;
using System.Linq.Expressions;
using System.Security.Claims;

namespace API.Services
{
    public class ProductService : IProductService
    {
        private readonly IRepository<Product> productRepositories;
        private readonly IRepository<SearchHistoryItem> searchHistoryItemRepositories;
        private readonly IAuthHeaderAccessor authHeaderAccessor;
        private readonly IMemoryCache memoryCache;

        public ProductService(IRepository<Product> productRepositories,
            IRepository<SearchHistoryItem> searchHistoryItemRepositories,
            IAuthHeaderAccessor authHeaderAccessor,
            IMemoryCache memoryCache)
        {
            this.productRepositories = productRepositories;
            this.searchHistoryItemRepositories = searchHistoryItemRepositories;
            this.authHeaderAccessor = authHeaderAccessor;
            this.memoryCache = memoryCache;
        }       

        public async Task<List<Product>> GetAllAsync()
        {
            var products = await productRepositories.GetAllAsync();
            return products;
        }

        public async Task<Product> GetByIdAsync(string id)
        {
            var product = await productRepositories.GetByIdAsync(id);
            return product;
        }

        public async Task<List<Product>> SearchAsync(string query, string? sortBy)
        {
            try
            {
                // Save search history
                await SaveSearchHistoryAsync(query);

                // Check if the search results are cached
                var cacheKey = $"SearchResult:{query}:{sortBy}";
                var cachedResults = memoryCache.Get(cacheKey) as List<Product>;

                if (cachedResults != null)
                {
                    // Return cached results if available
                    return cachedResults;
                }                              

                // Perform search query
                var result = await productRepositories.SearchAsync(entity =>
                entity.Name.Contains(query) || entity.Description.Contains(query) || entity.Price.ToString().Contains(query));

                if (!string.IsNullOrEmpty(sortBy))
                {
                    switch (sortBy.ToLower())
                    {
                        case "name":
                            result = result.OrderBy(entity => entity.Name).ToList();
                            break;
                        case "description":
                            // Sorting by date (assuming there's a DateTime property in Product entity)
                            result = result.OrderBy(entity => entity.Description).ToList();
                            break;
                        case "price":
                            // Sorting by popularity (assuming there's a Popularity property in Product entity)
                            result = result.OrderBy(entity => entity.Price).ToList();
                            break;
                        default:
                            throw new ArgumentException("Invalid sort criteria. Please specify 'name', 'description', or 'price'.");
                    }
                }
                // Cache the search results for future requests
                memoryCache.Set(cacheKey, result, DateTimeOffset.UtcNow.AddMinutes(10)); // Cache for 10 minutes
                return result;
            }
            catch (ArgumentException ex)
            {
                // Handle specific argument-related exceptions
                throw;
            }
            catch (Exception ex)
            {
                // Handle other exceptions and provide a meaningful error message
                throw new Exception("An error occurred while searching. Please try again later or contact the system administrator for assistance.", ex);
            }
        }
        private async Task SaveSearchHistoryAsync(string query)
        {
            try
            {

                // Save search history to database
                var user = authHeaderAccessor.GetUserId();
                if (user != null)
                {
                    var searchHistoryItem = new SearchHistoryItem
                    {
                        SearchQuery = query,
                        SearchTime = DateTime.UtcNow,
                        UserId = user
                    };

                    await searchHistoryItemRepositories.AddSearchHistoryAsync(searchHistoryItem);
                }
               
            }
            catch (Exception ex)
            {
                // Log error saving search history
                // Log.Error(ex, "An error occurred while saving search history.");
                throw new Exception(ex.Message);
            }
        }
        public async Task<Product> AddAsync(Product product)
        {
            return await productRepositories.AddAsync(product);
        }
        public async Task<List<SearchHistoryItem>> GetAllSearchHistoryByUserIdAsync(string userId)
        {
            var searchHistory = await searchHistoryItemRepositories.GetAllAsync();
            var result = searchHistory.Where(x => x.UserId == userId).ToList();
            return result;
        }
    }
}
