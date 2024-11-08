using Fun_Funding.Application.ViewModel.OrderDetailDTO;
using Fun_Funding.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fun_Funding.Application.ViewModel.MarketplaceFileDTO
{
    public class MarketplaceFileInfoResponse
    {
        public string? Name { get; set; }
        public string? URL { get; set; }
        public string? Version { get; set; }
        public string? Description { get; set; }
        public FileType FileType { get; set; }
    }
}
