using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using dot_net_st_pete_api.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace dot_net_st_pete_api.Repository
{
    public class JournalRepository : IJournalRepository
    {
        private readonly JournalContext _context = null;

        public JournalRepository(IOptions<MongoSettings> settings)
        {
            _context = new JournalContext(settings);
        }

        public async Task AddJournalEntry(JournalEntry item)
        {
            item.CreatedAt = DateTime.Now;
            item.UpdatedAt = DateTime.Now;
            await _context.JournalEntries.InsertOneAsync(item);
        }

        public async Task<IEnumerable<JournalEntry>> GetAllJournalEntries()
        {
            return await _context.JournalEntries.Find(_ => true).ToListAsync();
        }

        public async Task<JournalEntry> GetJournalEntry(string id)
        {
            var filter = Builders<JournalEntry>.Filter.Eq("Id", id);
            return await _context.JournalEntries
                                 .Find(filter)
                                 .FirstOrDefaultAsync();
        }

        public async Task<DeleteResult> RemoveJournalEntry(string id)
        {
            return await _context.JournalEntries.DeleteOneAsync(
                        Builders<JournalEntry>.Filter.Eq("Id", id));
        }

        // todo: finish wiring up update
        public async Task<UpdateResult> UpdateJournalEntry(string id, JournalEntry j)
        {
            var filter = Builders<JournalEntry>.Filter.Eq(s => s.Id.ToString(), id);
            var update = Builders<JournalEntry>.Update
                                .Set(s => s.BeerName, j.BeerName);
            // .CurrentDate(s => s.UpdatedAt);
            return await _context.JournalEntries.UpdateOneAsync(filter, update);
        }
    }
}