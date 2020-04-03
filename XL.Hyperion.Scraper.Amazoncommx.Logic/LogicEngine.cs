using XL.Hyperion.Domain.Contracts;
using XL.Hyperion.Domain.Models;
using XL.Hyperion.Scraper.Amazoncommx.Data;
using XL.Hyperion.Scraper.Event;

namespace XL.Hyperion.Scraper.Amazoncommx.Logic
{
    public class LogicEngine
    {
        public void RunScraping(ScraperConfig scraperConfig)
        {
            IScraperEngine dataEngine = new DataEngine();

            ItemInfoDetail itemInfoDetail = dataEngine.RunScraping(scraperConfig);

            if (itemInfoDetail.Price == 0)
            {
                TelemetryEvent.SendEventRunScraping(scraperConfig, itemInfoDetail);
            }

            Scraper.Data.DataEngine.SetItemInfoDetail(scraperConfig.ItemInfoId, itemInfoDetail);
        }

        public SearchInfo Search(string keyword)
        {
            IScraperEngine dataEngine = new DataEngine();

            SearchInfo searchInfo = dataEngine.Search(keyword);

            if (searchInfo.Items.Count == 0)
            {
                TelemetryEvent.SendEventSearch(keyword);
            }

            return searchInfo;
        }

        public ItemInfo SearchDetail(ScraperConfig scraperConfig)
        {
            IScraperEngine dataEngine = new DataEngine();

            ItemInfo itemInfo = dataEngine.SearchDetail(scraperConfig);

            if (string.IsNullOrEmpty(itemInfo.ItemName))
            {
                TelemetryEvent.SendEventSearchDetail(scraperConfig, itemInfo);
            }

            return itemInfo;
        }
    }
}