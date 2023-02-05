using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Models.Output;
using System.Collections.Generic;
using System.Linq;

namespace Lesson3API
{
    public static class GetById
    {
        [FunctionName("GetById")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "{id:guid}")] HttpRequest req, Guid id,
            [CosmosDB(
                databaseName:"beer-db",
                containerName:"beer-container",
                Connection = "DBConnection",
                SqlQuery = "SELECT * FROM c")] IEnumerable<Beer> input,
            ILogger log)
        {
            log.LogInformation($"GetById function has started!");


            var result = input.Where(beer => beer.Id == id).FirstOrDefault();

            if(result == null )
            {
                return new NotFoundObjectResult("Beer not found!");
            }

            return new OkObjectResult(result);
        }
    }
}
