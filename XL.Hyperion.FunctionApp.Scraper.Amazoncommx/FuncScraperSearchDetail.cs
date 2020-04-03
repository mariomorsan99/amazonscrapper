using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using XL.Hyperion.Domain.Models;
using XL.Hyperion.Scraper.Amazoncommx.Logic;
using XL.Hyperion.Tools.Exception;

namespace XL.Hyperion.FunctionApp.Scraper.Amazoncommx
{
    public static class FuncScraperSearchDetail
    {
        [FunctionName("amazoncommx-searchdetail")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = "search/detail")] HttpRequest req, ILogger log)
        {
            var dateStart = DateTime.Now;

            log.LogInformation("HTTP trigger function init request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

            try
            {
                ScraperConfig scraperConfig = JsonConvert.DeserializeObject<ScraperConfig>(requestBody);

                if (scraperConfig == null || string.IsNullOrEmpty(scraperConfig.Source))
                {
                    return new BadRequestObjectResult("Please pass ScraperConfig in the request body.");
                }

                LogicEngine logicEngine = new LogicEngine();
                ItemInfo itemInfo = logicEngine.SearchDetail(scraperConfig);

                var result = new OkObjectResult(itemInfo);

                TimeSpan diff = DateTime.Now - dateStart;
                log.LogInformation($"HTTP trigger function processed a request. Time: {diff.TotalMilliseconds} ms.");

                return result;
            }
            catch (PlatformException e)
            {
                log.LogError(new EventId(-200), e, $"xl-error-platform: {e.Message} RequestBody: {requestBody}");
                throw e;
            }
            catch (Exception e)
            {
                log.LogError(new EventId(-100), e, $"xl-error: {e.Message} RequestBody: {requestBody}");
                throw e;
            }
        }
    }
}