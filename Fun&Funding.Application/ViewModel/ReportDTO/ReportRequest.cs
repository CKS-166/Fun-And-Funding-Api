using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fun_Funding.Application.ViewModel.ReportDTO
{
    public class ReportRequest
    { 
        public Guid ProjectId { get; set; }
        public string Content { get; set; }
        public List<string> FileUrls { get; set; }
    }
}
