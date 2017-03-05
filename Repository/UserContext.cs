using dot_net_st_pete_api.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace dot_net_st_pete_api.Repository
{
    public class UserContext
    {
        private readonly IMongoDatabase _database = null;

        public UserContext(IOptions<MongoSettings> settings)
        {
            var client = new MongoClient(settings.Value.ConnectionString);
            if (client != null)
                _database = client.GetDatabase(settings.Value.Database);
        }

        public IMongoCollection<User> Users
        {
            get
            {
                return _database.GetCollection<User>("User");
            }
        }
    }
}