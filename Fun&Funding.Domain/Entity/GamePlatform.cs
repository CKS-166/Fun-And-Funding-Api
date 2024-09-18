using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fun_Funding.Domain.Entity
{
    public class GamePlatform : BaseEntity
    {
        public string PlatformName { get; set; }
        public virtual ICollection<MarketingProject> MarketingProjects { get; set; }
    }
}
