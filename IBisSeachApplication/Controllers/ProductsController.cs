using API.Infrastructure;
using API.Infrastructure.Repositories;
using API.Models;
using API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq.Expressions;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace IBisSeachApplication.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService productService;
        private readonly ILogger<ProductsController> logger;

        public ProductsController(IProductService productService, ILogger<ProductsController> logger)
        {
            this.productService = productService;
            this.logger = logger;
        }
        /// <summary>
        /// Retrieves all products.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<List<Product>>> GetAllAsync()
        {
            var products = await productService.GetAllAsync();
            return Ok(products);
        }
        /// <summary>
        /// Retrieves an item by ID.
        /// </summary>
        /// <param name="id">The ID of the product to retrieve.</param>
        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetByIdAsync(string id)
        {
            var product = await productService.GetByIdAsync(id);
            if (product == null)
                return NotFound();

            return Ok(product);
        }
        /// <summary>
        /// Searches for products based on the provided query string and allows refining the search results with filtering options.
        /// </summary>
        /// <param name="query">The search query string to match against product names, descriptions, and other attributes.</param>
        /// <param name="sortBy">Optional. Specifies the criteria by which to sort the search results ('name', 'description', 'price').</param>
        /// <returns>Returns a list of products that match the search query, sorted according to the specified criteria (if provided).</returns>
        
        [HttpGet("search")]
        public async Task<ActionResult<List<Product>>> SearchAsync(string query, string? sortBy)
        {
            try
            {
                var searchResults = await productService.SearchAsync(query, sortBy);
                return Ok(searchResults);
            }
            catch (ArgumentException ex)
            {
                logger.LogInformation(ex.Message);
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                // Log the exception for system administrators
                logger.LogInformation(ex, "An error occurred while searching.");

                // Return a generic error message to the user
                return StatusCode(500, "An error occurred while processing your request. Please try again later.");
            }
        }
        /// <summary>
        /// Creates a new product.
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     {
        ///         "name": "Product Name",
        ///         "description": "Product Description",
        ///         "price": 10.99
        ///     }
        ///     
        /// </remarks>
        /// <param name="product">The product object to create.</param>
        /// <returns>Returns the newly created product.</returns>
        /// <response code="200">Returns the newly created product.</response>
        /// <response code="400">If the product data is invalid.</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AddAsync([FromBody] Product product)
        {
            var result = await productService.AddAsync(product);
            return Ok(result);
        }
        /// <summary>
        /// Retrieves all search histories by userID.
        /// </summary>
        /// /// <param name="userId">The UserId of the Serch history to retrieve.</param>
        [HttpGet("searchhistory/userid")]
        public async Task<ActionResult<List<SearchHistoryItem>>> GetAllSearchHistoryByUserIdAsync(string userId)
        {
            var products = await productService.GetAllSearchHistoryByUserIdAsync(userId);
            if (products?.Count == 0)
                return NotFound();
            return Ok(products);
        }
    }

}