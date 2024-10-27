using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fun_Funding.Domain.Entity;
using Fun_Funding.Application.ViewModel.MarketplaceFileDTO;

namespace Fun_Funding.Application.ViewModel.MarketplaceProjectDTO
{
    public class MarketplaceProjectInfoResponse
    {
        public string? Introduction { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        [Range(0, (double)decimal.MaxValue)]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Price { get; set; }
        public virtual ICollection<MarketplaceFileInfoResponse>? MarketingFiles { get; set; }
    }
}
