using API.Infrastructure;

namespace API.Models
{
    public class SearchHistoryItem : Entity
    {
        public string SearchQuery { get; set; }
        public DateTime SearchTime { get; set; }
        public string UserId { get; set; }
    }
}
