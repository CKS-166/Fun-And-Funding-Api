using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fun_Funding.Domain.Enum
{
    public enum ProjectStatus
    {
        Deleted,
        Pending,
        Processing,
        Successful,
        Failed,
        Rejected,
        Approved,
        Withdrawed
    }
}
