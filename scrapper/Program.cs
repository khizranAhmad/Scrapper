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
using scrapper.service;
using scrapper.config;

namespace scrapper
{
    class Program
    {
        static ScrapingBrowser _scrapingbrowser = new ScrapingBrowser();
        static async Task Main(string[] args)
        {
            // Extracting the links
            var links = await ScrapperService.GetLinks();
            // Extracting the childe pages
            await ScrapperService.GetChildPage(links);            
        }
    }

}