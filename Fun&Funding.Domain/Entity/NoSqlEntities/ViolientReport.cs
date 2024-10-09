using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fun_Funding.Domain.Entity.NoSqlEntities
{
    public class ViolientReport
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public Guid Id { get; set; }

        [BsonElement("ReporterId")]
        public Guid ReporterId { get; set; }

        [BsonElement("ProjectId")]
        public Guid ProjectId { get; set; }
        [BsonElement("Content")]
        public string Content { get; set; }
        [BsonElement("IsHandle")]
        public bool IsHandle { get; set; } 
        [BsonElement("ReportDate")]
        public DateTime Date { get; set; }

        [BsonElement("FileUrls")]
        public List<string> FileUrls { get; set; }
    }
}
