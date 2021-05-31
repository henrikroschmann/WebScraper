using System.Collections.Generic;

namespace WebScraper.Scraper.Models
{
    public class GroupItem
    {
        public string From { get; set; }
        public string To { get; set; }
        public List<string> Result { get; set; } = new();

        public GroupItemType GroupItemType { get; set; }
        public GroupItem Extraction { get; set; }
    }
}
