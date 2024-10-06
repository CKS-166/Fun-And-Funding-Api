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
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }  

        [BsonElement("UserId")]
        public Guid UserId { get; set; }  

        [BsonElement("Projected")]
        public string Projected { get; set; }  

        [BsonElement("FileUrls")]
        public List<string> FileUrls { get; set; }
    }
}
