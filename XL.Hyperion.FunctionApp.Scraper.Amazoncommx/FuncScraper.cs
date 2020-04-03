using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XL.Hyperion.Domain.Models;
using XL.Hyperion.Scraper.Amazoncommx.Logic;
using XL.Hyperion.Tools.Data.Config.Sql;
using XL.Hyperion.Tools.Exception;

namespace XL.Hyperion.FunctionApp.Scraper.Amazoncommx
{
    public static class FuncScraper
    {
        [FunctionName("amazoncommx")]
        public static async Task Run([ServiceBusTrigger("%ServiceBusQueueName%", Connection = "ServiceBusQueueConnectionString")]string message, ILogger log)
        {
            log.LogInformation("***** Iniciando scraper-amazoncommx *****");

            var dateStart = DateTime.Now;

            SqlTopazBootstrapper.LoadConfigSqlRetry();

            var exceptions = new List<Exception>();

            try
            {
                ScraperConfig scraperConfig = JsonConvert.DeserializeObject<ScraperConfig>(message);

                LogicEngine logicEngine = new LogicEngine();
                logicEngine.RunScraping(scraperConfig);

                TimeSpan diff = DateTime.Now - dateStart;
                log.LogInformation($"Mensaje procesado: {message} Time: {diff.TotalMilliseconds} ms.");
                await Task.Yield();
            }
            catch (PlatformException e)
            {
                exceptions.Add(e);
                log.LogError(new EventId(-200), e, $"xl-error-platform: {e.Message} MessageBody: {message}");
            }
            catch (Exception e)
            {
                exceptions.Add(e);
                log.LogError(new EventId(-100), e, $"xl-error: {e.Message} MessageBody: {message}");
            }

            if (exceptions.Count > 1)
                throw new AggregateException(exceptions);

            if (exceptions.Count == 1)
                throw exceptions.Single();
        }
    }
}
