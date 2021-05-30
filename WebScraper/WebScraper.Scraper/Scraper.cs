using System;
using System.IO;
using System.Net;
using System.Text.Json;
using WebScraper.Scraper.Models;

namespace WebScraper.Scraper
{
    public class Scraper
    {
        private readonly Models.Scraper _scraper;

        public Scraper(Models.Scraper scraper)
        {
            _scraper = scraper;
        }

        public Models.Scraper GetData()
        {
            Logger.WriteLine($"Starting to fetch url {_scraper.Url}");
            var request = (HttpWebRequest)WebRequest.Create(_scraper.Url);
            request.AutomaticDecompression = DecompressionMethods.GZip;
            var response = (HttpWebResponse)request.GetResponse();
            var stream = response.GetResponseStream();
            var reader = new StreamReader(stream);
            var html = reader.ReadToEnd();
            if (html.IndexOf(_scraper.Scope.From, StringComparison.Ordinal) == -1)
                return null;
            if (html.LastIndexOf(_scraper.Scope.To, StringComparison.Ordinal) == -1)
                return null;

            html = html[html.IndexOf(_scraper.Scope.From, StringComparison.Ordinal)..html.LastIndexOf(_scraper.Scope.To, StringComparison.Ordinal)];
            Logger.WriteLine($"Done to fetch url {_scraper.Url}");
            foreach (var scraperGroup in _scraper.Groups)
            {
                switch (scraperGroup.GroupItemType)
                {
                    case GroupItemType.Single:
                        var from = html.IndexOf(scraperGroup.From, StringComparison.Ordinal);
                        var to = html.LastIndexOf(scraperGroup.To, StringComparison.Ordinal);
                        scraperGroup.Result.Add(html[from..to]);
                        break;

                    case GroupItemType.Loop:
                        var splitLines = html.Split(scraperGroup.From);
                        for (var i = 1; i < splitLines.Length; i++)
                        {
                            var split = scraperGroup.From + splitLines[i];
                            if (split.IndexOf(scraperGroup.From, StringComparison.Ordinal) == -1)
                                continue;
                            if (split.LastIndexOf(scraperGroup.To, StringComparison.Ordinal) == -1)
                                continue;


                            var result = split[
                                split.IndexOf(scraperGroup.From, StringComparison.Ordinal)..split.LastIndexOf(
                                    scraperGroup.To, StringComparison.Ordinal)];
                            scraperGroup.Result.Add(result);
                            if (scraperGroup.Extraction == null)
                                continue;
                            switch (scraperGroup.Extraction.GroupItemType)
                            {
                                case GroupItemType.Single:
                                    var eSFrom = result.IndexOf(scraperGroup.Extraction.From, StringComparison.Ordinal) + scraperGroup.Extraction.From.Length;
                                    var eSTo = result.Split(scraperGroup.Extraction.From)[1].IndexOf(scraperGroup.Extraction.To, StringComparison.Ordinal) + eSFrom;
                                    Logger.WriteLine($"Extracted the following {result[eSFrom..eSTo]}");
                                    scraperGroup.Extraction?.Result.Add(result[eSFrom..eSTo]);
                                    break;
                                case GroupItemType.Loop:
                                    var splitter = result.Split(scraperGroup.Extraction.From);
                                    var extractionResult = new string[splitter.Length - 1];
                                    for (var j = 1; j < splitter.Length; j++)
                                    {
                                        var extractionSplit = scraperGroup.Extraction.From + splitter[j];
                                        if (extractionSplit.IndexOf(scraperGroup.Extraction.From, StringComparison.Ordinal) == -1)
                                            continue;
                                        if (extractionSplit.LastIndexOf(scraperGroup.Extraction.To, StringComparison.Ordinal) == -1)
                                            continue;
                                        var eLFrom = extractionSplit.IndexOf(scraperGroup.Extraction.From, StringComparison.Ordinal) + scraperGroup.Extraction.From.Length;
                                        var eLTo = extractionSplit.Split(scraperGroup.Extraction.From)[1].IndexOf(scraperGroup.Extraction.To, StringComparison.Ordinal) + eLFrom;
                                        extractionResult[j-1] = extractionSplit[eLFrom..eLTo];
                                    }
                                    Logger.WriteLine($"Extracted the following {JsonSerializer.Serialize(extractionResult)}");
                                    scraperGroup.Extraction?.Result.Add(JsonSerializer.Serialize(extractionResult));
                                    break;
                                default:
                                    throw new ArgumentOutOfRangeException();
                            }
                        }
                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            return _scraper;
        }
    }
}