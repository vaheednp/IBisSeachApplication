using API.Infrastructure;

namespace API.Models
{
    public class Users : Entity
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }

    }
}
