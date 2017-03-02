using System;

namespace dot_net_st_pete_api.Models
{
    public class JournalEntry
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public DateTime SampleDate { get; set; }
        public string Notes { get; set; }
    }
}
