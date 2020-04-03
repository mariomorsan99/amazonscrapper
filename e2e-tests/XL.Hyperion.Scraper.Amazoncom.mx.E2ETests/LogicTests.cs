using System;
using XL.Hyperion.Domain.Models;
using XL.Hyperion.Scraper.Amazoncommx.Logic;
using XL.Hyperion.Tools.Data.Config.Sql;

namespace XL.Hyperion.Scraper.Amazoncom.mx.E2ETests
{
    public static class LogicTests
    {
        public static void RunScraping()
        {
            SqlTopazBootstrapper.LoadConfigSqlRetry();

            Guid searchId = Guid.NewGuid();
            Guid itemInfoId = Guid.NewGuid();
            ScraperConfig scraperConfig = new ScraperConfig
            {
                CommerceId = 111,
                SearchId = searchId,
                ItemInfoId = itemInfoId,
                Keyword = "",
                Source = "https://www.sams.com.mx/celulares/iphone-xr-apple-64gb-white-at-t/980010722"
            };

            try
            {
                LogicEngine logicEngine = new LogicEngine();

                logicEngine.RunScraping(scraperConfig);

            }
            catch (Exception)
            {

            }
        }
    }
}
