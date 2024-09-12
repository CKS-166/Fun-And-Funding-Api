using Fun_Funding.Application.IRepository;
using Fun_Funding.Domain.Entity;
using Fun_Funding.Infrastructure.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fun_Funding.Infrastructure.Repository
{
    public class RewardItemRepository : BaseRepository<RewardItem>, IRewardItemRepository
    {
        public RewardItemRepository(MyDbContext context) : base(context)
        {
        }
    }
}
