using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fun_Funding.Domain.Entity
{
    public class MarketingProject : BaseEntity
    {
        public string Introduction {  get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        [Range(0, (double)decimal.MaxValue)]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Price { get; set; }

        public FundingProject FundingProject { get; set; }
        public Guid FundingProjectId { get; set; }
        public virtual ICollection<GamePlatform> GamePlatforms { get; set; }
        public virtual ICollection<DigitalKey> DigitalKeys { get; set; }
        public virtual ICollection<MarketingFile> MarketingFiles { get; set; }
    }
}
