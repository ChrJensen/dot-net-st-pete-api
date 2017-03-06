using System;
using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace dot_net_st_pete_api.Models
{
    public class JournalEntry
    {
        [BsonId]
        public ObjectId Id { get; set; }
        [Required]
        public string Brewery { get; set; }
        [Required]
        [BsonElement("BeerName")]
        public string BeerName { get; set; }
        [Required]
        [BsonElement("SampleDate")]
        public DateTime SampleDate { get; set; }
        [BsonElement("SampleNotes")]
        public string SampleNotes { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
