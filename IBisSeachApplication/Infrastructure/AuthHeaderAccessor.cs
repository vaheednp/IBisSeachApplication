using API.Services;
using Microsoft.AspNetCore.Identity;
using System.Configuration;
using System.Security.Claims;

namespace API.Infrastructure
{
    public class AuthHeaderAccessor : IAuthHeaderAccessor
    {
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IConfiguration configuration;
        public AuthHeaderAccessor(IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
        {
            this.httpContextAccessor = httpContextAccessor;
            this.configuration = configuration;
        }


        public string GetAuthToken()
        {
            var btoken = httpContextAccessor.HttpContext.Request.Headers["Authorization"].ToString();
            return btoken;
        }

        public string GetUserId()
        {
            var token = GetAuthToken()?.Split(" ").Last();
            // Validate the token and extract claims
            var tokenService = new TokenService(configuration);
            var principal = tokenService.ValidateToken(token);

            // Extract user ID and username from claims
            var userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var username = principal.FindFirst(ClaimTypes.Name)?.Value;
            return userId;
        }
    }
}
