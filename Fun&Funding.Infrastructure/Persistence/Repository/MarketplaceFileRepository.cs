using Fun_Funding.Application.Interfaces.IRepository;
using Fun_Funding.Domain.Entity;
using Fun_Funding.Infrastructure.Persistence.Database;

namespace Fun_Funding.Infrastructure.Persistence.Repository
{
    public class MarketplaceFileRepository : BaseRepository<MarketplaceFile>, IMarketplaceFileRepository
    {
        public MarketplaceFileRepository(MyDbContext context) : base(context)
        {
        }
    }
}
