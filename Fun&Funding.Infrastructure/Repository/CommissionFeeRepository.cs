using Fun_Funding.Application.IRepository;
using Fun_Funding.Domain.Entity;
using Fun_Funding.Infrastructure.Database;

namespace Fun_Funding.Infrastructure.Repository
{
    public class CommissionFeeRepository : BaseRepository<CommissionFee>, ICommissionFeeRepository
    {
        public CommissionFeeRepository(MyDbContext context) : base(context)
        {
        }
    }
}
