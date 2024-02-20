using API.Models;
using API.Services;
using API.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IConfiguration configuration;
        private readonly IUsersService usersService;
        private readonly ILogger<UserController> logger;
        public UserController(IConfiguration configuration, IUsersService usersService, ILogger<UserController> logger)
        {
            this.configuration = configuration;
            this.usersService = usersService;
            this.logger = logger;
        }

        /// <summary>
        /// Generates a JWT token for the provided user credentials.
        /// </summary>
        /// <param name="loginDetails">The user's login details.</param>
        /// <returns>
        /// A JWT token if authentication is successful; otherwise, returns a message indicating login failure.
        /// once token generated, copy that token and paste in to Authorization for authorizing the API
        /// </returns>
        /// <remarks>
        /// This endpoint is used to authenticate users and generate a JWT token upon successful authentication.
        /// </remarks>
        [HttpGet]
        [Route("/GetToken/")]
        public async Task<string> GetTokenAsync([FromQuery] UsersViewModel loginDetails)
        {
            try
            {
                if (loginDetails is null)
                {
                    throw new ArgumentNullException(nameof(loginDetails));
                }

                var isAuthenticate = await usersService.GetAthenticate(loginDetails.Email, loginDetails.Password);
                if (isAuthenticate)
                {
                    var user = await usersService.GetUserByEmaiAsync(loginDetails.Email);
                    var tokenHandler = new JwtSecurityTokenHandler();
                    var key = Encoding.UTF8.GetBytes(configuration["Jwt:SecretKey"]);
                    var creds = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature);
                    var tokenDescriptor = new SecurityTokenDescriptor
                    {
                        Issuer = configuration["Jwt:Issuer"],
                        Audience = configuration["Jwt:Audience"],
                        Subject = new ClaimsIdentity(new[]
                        {
                    new Claim(ClaimTypes.NameIdentifier, user.Id),
                    new Claim(ClaimTypes.Name, user.UserName)
                }),
                        Expires = DateTime.UtcNow.AddMinutes(30),
                        SigningCredentials = creds
                    };
                    var jwtToken = tokenHandler.CreateToken(tokenDescriptor);
                    var jwtTokenString = tokenHandler.WriteToken(jwtToken);
                    return jwtTokenString;
                }
                return $"Login failed";
            }
            catch (Exception ex)
            {
                // Log the exception
                logger.LogError(ex, "An error occurred while generating the token.");

                // Return an error response to the client
                return "Internal server error. Please try again later.";
            }
        }
        /// <summary>
        /// Adds a new user to the system.
        /// </summary>
        /// <param name="user">The user object containing details of the new user to be added.</param>
        /// <returns>
        /// Returns 200 OK if the user is successfully added to the system; otherwise, returns 400 Bad Request.
        /// </returns>
        /// <remarks>
        /// This endpoint is used to add a new user to the system. The user object should contain all the required details
        /// of the new user, such as username, password, email, etc.
        /// you can use this user's email and password for generating access token
        /// 
        /// Sample request:
        /// 
        ///     {
        ///         "userName": "vnp",
        ///         "password": "565656",
        ///         "email": aaa@gmailcom
        ///     }
        ///     
        /// 
        /// </remarks>
        [HttpPost]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AddAsync([FromBody] Users user)
        {
            var result = await usersService.AddAsync(user);
            return Ok(result);
        }
    }
}
