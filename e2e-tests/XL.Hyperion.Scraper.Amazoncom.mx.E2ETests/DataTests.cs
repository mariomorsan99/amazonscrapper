using System;
using XL.Hyperion.Domain.Contracts;
using XL.Hyperion.Domain.Models;
using XL.Hyperion.Scraper.Amazoncommx.Data;

namespace XL.Hyperion.Scraper.Amazoncom.mx.E2ETests
{
    public static class DataTests
    {
  
        public static void SearchList()
        {
            string keyword = "Mate 30 pro";
            IScraperEngine scraperEngine = new DataEngine();
            SearchInfo searchInfo = scraperEngine.Search(keyword);
        }

        public static void SearchDetail()
        {
            Guid searchId = Guid.NewGuid();
            ScraperConfig scraperConfig = new ScraperConfig
            {
                CommerceId = 111,
                SearchId = searchId,
                Keyword = "",
                Source = "https://www.amazon.com.mx/Apple-iPhone-Gris-Espacial-Renewed/dp/B07NH5GZFB/ref=ice_ac_b_dpb?__mk_es_MX=%C3%85M%C3%85%C5%BD%C3%95%C3%91&keywords=iphone&qid=1576455883&sr=8-2"
            };

            ItemInfo itemInfoExpected = new ItemInfo
            {
                CommerceId = 111,
                SearchId = searchId,
                Brand = "APPLE",
                ItemCode = "MQD32E/A",
                ItemName = " Apple iPhone 8 Gris Espacial 64 GB "
            };

            IScraperEngine scraperEngine = new DataEngine();

            ItemInfo itemInfo = scraperEngine.SearchDetail(scraperConfig);
        }

        public static void RunScraping()
        {
            Guid searchId = Guid.NewGuid();
            Guid itemInfoId = Guid.NewGuid();
            ScraperConfig scraperConfig = new ScraperConfig
            {
                CommerceId = 111,
                SearchId = searchId,
                ItemInfoId = itemInfoId,
                Keyword = "",
                Source = "https://www.amazon.com.mx/AFUNTA-Mu%C3%B1equera-reemplazo-Compatible-Transpirable/dp/B07L75WKKW/ref=pd_bxgy_121_2/141-2212725-7362833?_encoding=UTF8&pd_rd_i=B07L75WKKW&pd_rd_r=69e78c58-ea27-412f-bb41-ae8429eaa938&pd_rd_w=R3IyJ&pd_rd_wg=gqjOi&pf_rd_p=76b46a93-b06d-4295-9dad-635165de1341&pf_rd_r=JQ25BS6KJ8R92Q4845RA&psc=1&refRID=JQ25BS6KJ8R92Q4845RA"
            };

            ItemInfoDetail itemInfoDetailExpexted = new ItemInfoDetail
            {
                ListPrice = 24039,
                Price = 21859,
                Stock = 0
            };

            IScraperEngine scraperEngine = new DataEngine();

            ItemInfoDetail itemInfoDetail = scraperEngine.RunScraping(scraperConfig);
        }
    }
}
