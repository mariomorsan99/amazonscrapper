using ScrapySharp.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using XL.Hyperion.Domain.Contracts;
using XL.Hyperion.Domain.Models;
using XL.Hyperion.Scraper.Net;
using XL.Hyperion.Tools.Exception;
using XL.Hyperion.Tools;
using System.Globalization;

namespace XL.Hyperion.Scraper.Amazoncommx.Data
{
    public class DataEngine : IScraperEngine
    {
        public short Id { get => 111; set => throw new NotImplementedException(); }
        
        const string source = "https://www.amazon.com.mx";

        public ItemInfoDetail RunScraping(ScraperConfig scraperConfig)
        {
            var scrapingBrowser = ScrapySharpTool.GetScrapingBrowser();

            try
            {
                var documentItem = scrapingBrowser.NavigateToPage(new Uri(scraperConfig.Source));
                var item = documentItem.Html.CssSelect("#dp-container").FirstOrDefault();
                var price = item.CssSelect("#priceblock_ourprice").FirstOrDefault()?.InnerText.Replace("$", "").Trim();
                var listPrice = item.CssSelect(".priceBlockStrikePriceString").FirstOrDefault()?.InnerText.Replace("$", "").Trim();

                if (string.IsNullOrWhiteSpace(price))
                {
                    price = item.CssSelect(".offer-price").FirstOrDefault()?.InnerText.Replace("$", "").Trim();
                }

                var itemDetail = new ItemInfoDetail
                {
                    ListPrice = double.Parse(listPrice ?? price, CultureInfo.InvariantCulture),
                    Price = double.Parse(price, CultureInfo.InvariantCulture),
                    Date = Kit.GetDateTime()
                };

                return itemDetail;
            }
            catch (Exception e)
            {
                throw new PlatformException($"xl-user-agent: {scrapingBrowser.UserAgent.UserAgent}", e);
            }
        }

        public SearchInfo Search(string keyword)
         {
            var scrapingBrowser = ScrapySharpTool.GetScrapingBrowser();

            try
            {
                var documentItem = scrapingBrowser.NavigateToPage(new Uri($"{source}/s?k={keyword}"));
                var searchInfo = new SearchInfo
                {
                    CommerceId = Id,
                    Items = new List<ItemSearch>()
                };

                
                var htmlList = documentItem.Html.CssSelect(".s-result-list");

                foreach (var htmlItem in htmlList.CssSelect(".s-result-item"))
                {
                    var imageUri = htmlItem.CssSelect("img").FirstOrDefault()?.GetAttributeValue("src");

                    decimal price = htmlItem.CssSelect(".a-offscreen").FirstOrDefault() != null? Convert.ToDecimal(htmlItem.CssSelect(".a-offscreen").FirstOrDefault().InnerText.Replace("$",null)):0;
                    string cadPrice =Convert.ToString(price);
                    
                    var item = new ItemSearch
                    {
                        ItemName = htmlItem.CssSelect("h2").FirstOrDefault()?.CssSelect("a").FirstOrDefault().InnerText.Replace("\n", "").Trim(),
                        Source = $"{source}{htmlItem.CssSelect("h2").FirstOrDefault()?.CssSelect("a").FirstOrDefault().GetAttributeValue("href")}",
                        Available = true,
                        Price= float.Parse(cadPrice),
                        DetailUrl= $"{source}{htmlItem.CssSelect("h2").FirstOrDefault()?.CssSelect("a").FirstOrDefault().GetAttributeValue("href")}"
                    };

                    if (!string.IsNullOrWhiteSpace(imageUri))
                    {

                        item.ImageUri = new Uri(imageUri);
                    }

                    searchInfo.Items.Add(item);
                }

                return searchInfo;
            }
            catch (Exception e)
            {
                throw new PlatformException($"xl-user-agent: {scrapingBrowser.UserAgent.UserAgent}", e);
            }
        }

        public ItemInfo SearchDetail(ScraperConfig scraperConfig)
        {
            var scrapingBrowser = ScrapySharpTool.GetScrapingBrowser();

            try
            {
                var documentItem = scrapingBrowser.NavigateToPage(new Uri(scraperConfig.Source));

                var item = documentItem.Html.CssSelect("#dp-container").FirstOrDefault();
                var itemDetails = item.CssSelect("#prodDetails").FirstOrDefault();

                if (item is null)
                {
                    return default(ItemInfo);
                }

                var itemInfo = new ItemInfo
                {
                    Brand = item.CssSelect("#bylineInfo").FirstOrDefault()?.InnerText,
                    ItemName = item.CssSelect("#productTitle").FirstOrDefault()?.InnerText.Trim(),
                    CommerceId = scraperConfig.CommerceId,
                    SearchId = scraperConfig.SearchId
                };

                return itemInfo;
            }
            catch (Exception e)
            {
                throw new PlatformException($"xl-user-agent: {scrapingBrowser.UserAgent.UserAgent}", e);
            }
        }
    }
}
