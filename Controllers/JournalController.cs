using System.Collections.Generic;
using System.Threading.Tasks;
using dot_net_st_pete_api.Models;
using dot_net_st_pete_api.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using Newtonsoft.Json;

namespace dot_net_st_pete_api.Controllers
{
    [Route("[controller]")]
    public class JournalController : Controller
    {
        private readonly IJournalRepository journalRepository;

        public JournalController(IJournalRepository journalRepository)
        {
            this.journalRepository = journalRepository;
        }

        [HttpGet]
        public Task<IEnumerable<JournalEntry>> Get()
        {
            return GetEntries();
        }

        private async Task<IEnumerable<JournalEntry>> GetEntries()
        {
            return await journalRepository.GetAllJournalEntries();
        }

        [HttpPost]
        [AllowAnonymous]
        public IActionResult Post([FromBody]JournalEntry j)
        {
            journalRepository.AddJournalEntry(j);
            return new OkObjectResult(j);
        }
    }
}
