using System.Collections.Generic;

namespace WebScraper.Scraper.Models
{
    public class Scraper
    {
        public string Url { get; set; }
        public Scope Scope { get; set; }
        public List<GroupItem> Groups { get; set; }
    }
}
