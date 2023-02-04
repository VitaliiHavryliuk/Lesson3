using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using Models.Output;

namespace Lesson3API
{
    public static class GetAll
    {
        [FunctionName("GetAll")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
            [CosmosDB(
                databaseName:"beer-db",
                containerName:"beer-container",
                Connection = "DBConnection",
                SqlQuery = "SELECT * FROM c")] IEnumerable<Beer> input,
            ILogger log)
        {
            log.LogInformation($"GetAll function has started!");
            log.LogInformation($"{input.Count()} beers received!");
            var result = input.Where(beer => beer.Email == req.HttpContext?.User?.Identity?.Name);
            return new OkObjectResult(result);
        }
    }
} 
