
# WebScraper with recipe

Project in devlopment for myself to scrape data from the web based on input recipe

Example of recipe extracting url from page

```
new Scraper.Models.Scraper()
            {
                Url = "https://epaccess.penser.se/",
                Scope = new Scope()
                {
                    From = "<div class=\"submenu-items\">",
                    To = "</div> <!-- .submenu-items -->"
                },
                Groups = new List<GroupItem>
                {
                    new()
                    {
                        From = "<a",
                        To = "</a",
                        GroupItemType = GroupItemType.Loop,
                        Extraction = new GroupItem()
                        {
                            From = "href=\"",
                            To = "\""
                        }
                    }
                }
            };
```

Return: string
```
    http://xxx.xxx.xxx/
```
## Deployment

To deploy this project run just buid the solution in visual studio
Early stages nothing to see. 


  
