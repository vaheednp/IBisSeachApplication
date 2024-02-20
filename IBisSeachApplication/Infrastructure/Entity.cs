using static API.Infrastructure.Entity;

namespace API.Infrastructure
{
    public abstract class Entity
    {
        public virtual string Id { get; protected set; }
        protected Entity()
        {
            Id = Guid.NewGuid().ToString();
        }
    }
}
