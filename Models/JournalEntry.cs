using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace dot_net_st_pete_api.Models
{
    public class JournalEntry : BaseModel
    {
        [BsonElement("BeerName")]
        public string BeerName { get; set; }
        [BsonElement("SampleDate")]
        public DateTime SampleDate { get; set; }
        [BsonElement("SampleNotes")]
        public string SampleNotes { get; set; }
    }
}
