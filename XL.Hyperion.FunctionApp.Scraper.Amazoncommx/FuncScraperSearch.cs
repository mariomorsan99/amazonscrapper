using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using XL.Hyperion.Scraper.Amazoncommx.Logic;
using XL.Hyperion.Domain.Models;
using XL.Hyperion.Tools.Exception;

namespace XL.Hyperion.FunctionApp.Scraper.Amazoncommx
{
    public static class FuncScraperSearch
    {
        [FunctionName("amazoncommx-search")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", Route = "search")] HttpRequest req, ILogger log)
        {
            var dateStart = DateTime.Now;

            log.LogInformation("HTTP trigger function init request.");

            string keyword = req.Query["keyword"];

            if (string.IsNullOrEmpty(keyword))
            {
                return new BadRequestObjectResult("Please pass a name on the query string 'keyword' or in the request body.");
            }

            try
            {
                LogicEngine logicEngine = new LogicEngine();
                SearchInfo searchInfo = logicEngine.Search(keyword);

                var result = new OkObjectResult(searchInfo);

                TimeSpan diff = DateTime.Now - dateStart;
                log.LogInformation($"HTTP trigger function processed a request. Time: {diff.TotalMilliseconds} ms.");
                await Task.Yield();

                return result;
            }
            catch (PlatformException e)
            {
                log.LogError(new EventId(-200), e, $"xl-error-platform: {e.Message} Keyword: {keyword}");
                throw e;
            }
            catch (Exception e)
            {
                log.LogError(new EventId(-100), e, $"xl-error: {e.Message} Keyword: {keyword}");
                throw e;
            }
        }
    }
}