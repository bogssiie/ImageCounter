
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;
using AngleSharp;
using AngleSharp.Parser.Html;
using System.Linq;

namespace FunctionApp2
{
    public static class Function1
    {
        [FunctionName("Function1")]
        public static async System.Threading.Tasks.Task<IActionResult> RunAsync([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)]HttpRequest req, TraceWriter log)
        {
            log.Info("C# HTTP trigger function processed a request.");

            var config = Configuration.Default.WithDefaultLoader();
            var address = "https://azure.microsoft.com/en-us/services/functions/";
            var context = BrowsingContext.New(config);
            //awaits waits for the scripts to be loaded before firing
            var document = await context.OpenAsync(address);
            var cellSelector = "img";
            var cells = document.QuerySelectorAll(cellSelector);

            var count = cells.Count();
        

            return count > 0
                ? (ActionResult)new OkObjectResult($"Total number of images in the page: {count}")
                : new BadRequestObjectResult("Please pass a website on the query string or in the request body");
        }
    }
}
