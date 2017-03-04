using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace dot_net_st_pete_api.Models
{
    public class BaseModel
    {
        public ObjectId Id { get; set; }
        [BsonElement("CreatedAt")]
        public DateTime CreatedAt { get; set; }
        [BsonElement("UpdatedAt")]
        public DateTime UpdatedAt { get; set; }
    }
}