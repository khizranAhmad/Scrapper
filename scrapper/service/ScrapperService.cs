using System;
using System.Collections.Generic;
using HtmlAgilityPack;
using ScrapySharp.Extensions;
using ScrapySharp.Network;
using System.IO;
using System.Globalization;
using CsvHelper;
using System.Linq;
using System.Threading.Tasks;
using scrapper.config;
using scrapper.utilities;

namespace scrapper.service
{
    public static class ScrapperService
    {
        private readonly static ScrapingBrowser _scrapingbrowser;

        static ScrapperService()
        {
            _scrapingbrowser = new ScrapingBrowser();
        }

        public static async Task<List<string>> GetLinks()
        {
           
            
            var mainPageLinks = new List<string>();

            var html = await GetHtml(Constants.BASE_URL);
            await saveFile("index", "/", html.InnerHtml);
            
            var links = html.CssSelect("a");

            foreach (var link in links)
            {
              
                if (link.Attributes["href"] != null
                    && link.Attributes["href"].Value != null
                    && !link.Attributes["href"].Value.Contains("https://")
                    && !link.Attributes["href"].Value.Contains("http://")
                    && link.Attributes["href"].Value!="/")
                {
                    mainPageLinks.Add(link.Attributes["href"].Value);
                }
                
            }

            return mainPageLinks;
        }
        public static async Task GetChildPage(List<string> urls)
        {
            Console.WriteLine("Downloading pages .........");
            Console.WriteLine();
            var progress = new ProgressBar();
            int counter = 0;
            foreach (var url in urls)
            {
                counter++;
                progress.Report((double)counter / urls.Count());
                string UpdatedUrl = url.Replace("#", "/");
                string folderName = string.Empty;
                string fileName = string.Empty;

                var htmlNode = await GetHtml(Constants.BASE_URL + url);
                var count = UpdatedUrl?.Replace("#", "/").Count(x => x == '/');
                if (count > 1)
                {
                    folderName = UpdatedUrl.Substring(0, UpdatedUrl.LastIndexOf("/"));
                    fileName = UpdatedUrl.Split('/').Last();
                    await saveFile(fileName, folderName, htmlNode.InnerHtml);
                }
                else
                {
                    folderName = UpdatedUrl.Substring(0, UpdatedUrl.LastIndexOf("/"));
                    fileName = UpdatedUrl.Split('/').Last();
                    await saveFile(fileName, "/", htmlNode.InnerHtml);
                }

            }
        }
        static async Task<HtmlNode> GetHtml(string url)
        {
            WebPage webPage = await _scrapingbrowser.NavigateToPageAsync(new Uri(url));
            return webPage.Html;
        }
        static async Task saveFile(string fileName, string folderName, string file)
        {
            string path = Path.Combine(Environment.CurrentDirectory, Constants.BASE_FOLDER + folderName);


            if (!Directory.Exists(path)) Directory.CreateDirectory(path);

            await File.WriteAllTextAsync(Path.Combine(path, $"{fileName}.html"), file);
        }
    }
}
