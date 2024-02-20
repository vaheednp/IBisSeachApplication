namespace API.Infrastructure
{
    public interface IAuthHeaderAccessor
    {
        string GetUserId();

        string GetAuthToken();
    }
}
