
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
using System.Text.RegularExpressions;
using System;

namespace FunctionApp2
{
    public static class Function1
    {
        [FunctionName("Function1")]
        public static async System.Threading.Tasks.Task<IActionResult> RunAsync([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)]HttpRequest req, TraceWriter log)
        {
            log.Info("C# HTTP trigger function processed a request.");

            Regex rx = new Regex("<[^>]*>");

            string htmlCode = req.Query["Code"];

            string request = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(request);
            htmlCode = htmlCode ?? data?.Code;

            var parser = new HtmlParser();

            var source = htmlCode;
            var document = parser.Parse(source);
            var docuLinq = document.All.Where(m => m.LocalName == "img");
            var count = docuLinq.Count();

            string words = rx.Replace(htmlCode, "");
            MatchCollection collection = Regex.Matches(words, @"[\S]+");

            var gcd = GreatestCommonDenominator(collection.Count, count);

            if(htmlCode != null)
            {
                ActionResult glenn = (ActionResult)new OkObjectResult($"Total number of images in the page: {count}\n" +
                    $"Total number of words: {collection.Count}\n" +
                    $"Image to text Ratio: {count/gcd}:{collection.Count/gcd}");
                return glenn;
            }
            else
            {
                return new BadRequestObjectResult("Please pass a website on the query string or in the request body");
            }
        }

        static int GreatestCommonDenominator(int x, int y)
        {
            if(y == 0)
            {
                return Math.Abs(x);
            }
            else
            {
                return GreatestCommonDenominator(y, x % y);
            }

        }
    }
}
