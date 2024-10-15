using Fun_Funding.Application.IRepository;
using Fun_Funding.Domain.Entity.NoSqlEntities;
using Fun_Funding.Infrastructure.Database;

namespace Fun_Funding.Infrastructure.Repository
{
    public class ChatRepository : MongoBaseRepository<Chat>, IChatRepository
    {
        public ChatRepository(MongoDBContext mongoDB) : base(mongoDB, "chat")
        {
        }
    }
}
