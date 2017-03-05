using dot_net_st_pete_api.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace dot_net_st_pete_api.Repository
{
    public class JournalContext
    {
        private readonly IMongoDatabase _database = null;

        public JournalContext(IOptions<MongoSettings> settings)
        {
            var client = new MongoClient(settings.Value.ConnectionString);
            if (client != null)
                _database = client.GetDatabase(settings.Value.Database);
        }

        public IMongoCollection<JournalEntry> JournalEntries
        {
            get
            {
                return _database.GetCollection<JournalEntry>("JournalEntry");
            }
        }
    }
}