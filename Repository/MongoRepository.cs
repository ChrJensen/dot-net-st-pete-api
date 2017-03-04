using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using System.Collections.Generic;
using dot_net_st_pete_api.Models;

namespace dot_net_st_pete_api.Repository
{
    public class MongoRepository
    {
        MongoClient _client;
        MongoServer _server;
        MongoDatabase _db;

        public MongoRepository()
        {
            _client = new MongoClient("mongodb://localhost:27017");
            _server = _client.GetServer();
            _db = _server.GetDatabase("dot-net-st-pete-api");
        }

        public IEnumerable<JournalEntry> GetJournalEntries()
        {
            return _db.GetCollection<JournalEntry>("JournalEntry").FindAll();
        }


        public JournalEntry GetJournalEntry(ObjectId id)
        {
            var res = Query<JournalEntry>.EQ(p => p.Id, id);
            return _db.GetCollection<JournalEntry>("JournalEntry").FindOne(res);
        }

        public JournalEntry Create(JournalEntry p)
        {
            _db.GetCollection<JournalEntry>("JournalEntry").Save(p);
            return p;
        }

        public void Update(ObjectId id, JournalEntry p)
        {
            p.Id = id;
            var res = Query<JournalEntry>.EQ(pd => pd.Id, id);
            var operation = Update<JournalEntry>.Replace(p);
            _db.GetCollection<JournalEntry>("JournalEntry").Update(res, operation);
        }

        public void Remove(ObjectId id)
        {
            var res = Query<JournalEntry>.EQ(e => e.Id, id);
            var operation = _db.GetCollection<JournalEntry>("JournalEntry").Remove(res);
        }

        public User CreateUser(User u)
        {
            _db.GetCollection<User>("User").Save(u);
            return u;
        }

        public User GetUser(string email)
        {
            var res = Query<User>.EQ(u => u.Email, email);
            return _db.GetCollection<User>("User").FindOne(res);
        }
    }
}