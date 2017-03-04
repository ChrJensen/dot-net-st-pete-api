using MongoDB.Bson;

namespace dot_net_st_pete_api.Models
{
    public class User
    {
        public ObjectId Id { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

    }
}