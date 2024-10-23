using Fun_Funding.Application.IRepository;
using Fun_Funding.Domain.Entity;
using Fun_Funding.Infrastructure.Persistence.Database;

namespace Fun_Funding.Infrastructure.Persistence.Repository
{
    public class CommissionFeeRepository : BaseRepository<CommissionFee>, ICommissionFeeRepository
    {
        public CommissionFeeRepository(MyDbContext context) : base(context)
        {
        }
    }
}
