using API.Infrastructure.Repositories;
using API.Models;

namespace API.Services
{
    public class UsersService : IUsersService
    {
        private readonly IRepository<Users> usersRepositories;
        public UsersService(IRepository<Users> usersRepositories)
        {
            this.usersRepositories = usersRepositories;
        }
        public async Task<bool> GetAthenticate(string userName, string password)
        {
            var users = await usersRepositories.GetAllAsync();
            var user = users.FirstOrDefault(x => x.Email == userName && x.Password == password);
            if (user != null)
            {
                return true;
            }
            return false;
        }

        public async Task<List<Users>> GetUsers()
        {
            var users = await usersRepositories.GetAllAsync();
            return users;
        }
        public async Task<Users> GetUserByEmaiAsync(string email)
        {
            var users = await usersRepositories.SearchAsync(x => x.Email == email);
            var user = users.FirstOrDefault();
            return user;
        }
        public async Task<Users> AddAsync(Users user)
        {
            return await usersRepositories.AddAsync(user);
        }
    }
}
