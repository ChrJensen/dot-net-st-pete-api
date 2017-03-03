using System.Collections.Generic;
using dot_net_st_pete_api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;

namespace dot_net_st_pete_api.Controllers
{
    [Route("[controller]")]
    public class JournalController : Controller
    {
        MongoRepository mongo;

        public JournalController(MongoRepository mongo)
        {
            this.mongo = mongo;
        }

        [HttpGet]
        public IEnumerable<JournalEntry> Get()
        {
            return mongo.GetJournalEntries();
        }

        [HttpGet("{id:length(24)}")]
        public IActionResult Get(string id)
        {
            var product = mongo.GetJournalEntry(new ObjectId(id));
            if (product == null)
            {
                return NotFound();
            }
            return new ObjectResult(product);
        }

        [HttpPost]
        [AllowAnonymous]
        public IActionResult Post([FromBody]JournalEntry p)
        {
            mongo.Create(p);
            return new OkObjectResult(p);
        }
        [HttpPut("{id:length(24)}")]
        public IActionResult Put(string id, [FromBody]JournalEntry p)
        {
            var recId = new ObjectId(id);
            var product = mongo.GetJournalEntry(recId);
            if (product == null)
            {
                return NotFound();
            }

            mongo.Update(recId, p);
            return new OkResult();
        }

        [HttpDelete("{id:length(24)}")]
        public IActionResult Delete(string id)
        {
            var product = mongo.GetJournalEntry(new ObjectId(id));
            if (product == null)
            {
                return NotFound();
            }

            mongo.Remove(product.Id);
            return new OkResult();
        }
    }
}
