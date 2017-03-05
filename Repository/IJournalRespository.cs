using System.Collections.Generic;
using System.Threading.Tasks;
using dot_net_st_pete_api.Models;
using MongoDB.Driver;

namespace dot_net_st_pete_api.Repository
{
    public interface IJournalRepository
    {
        Task<IEnumerable<JournalEntry>> GetAllJournalEntries();
        Task<JournalEntry> GetJournalEntry(string id);
        Task AddJournalEntry(JournalEntry item);
        Task<DeleteResult> RemoveJournalEntry(string id);
        // Task<UpdateResult> UpdateJournalEntry(string id, string body);
    }
}