using Fun_Funding.Application.ViewModel.MarketplaceFileDTO;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fun_Funding.Application.ViewModel.MarketplaceProjectDTO
{
    public class MarketplaceProjectAddRequest
    {
        [Required]
        public string Introduction { get; set; } = string.Empty;
        [Required]
        public string Name { get; set; } = string.Empty;
        [Required]
        public string Description { get; set; } = string.Empty;
        [Required]
        [Range(0, (double)decimal.MaxValue)]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Price { get; set; }
        [Required]
        public Guid FundingProjectId { get; set; }
        [Required]
        public List<MarketplaceFileAddRequest> MarketplaceFiles { get; set; } = new List<MarketplaceFileAddRequest>();
    }
}
