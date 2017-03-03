using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace dot_net_st_pete_api.Controllers
{
    [Route("[controller]")]
    public class JournalController : Controller
    {
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }
    }
}
