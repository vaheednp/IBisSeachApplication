using API.Models;

namespace API.Services
{
    public interface IUsersService
    {
        Task<List<Users>> GetUsers();
        Task<bool> GetAthenticate(string email, string password);
        Task<Users> GetUserByEmaiAsync(string email);
        Task<Users> AddAsync(Users user);
    }
}
