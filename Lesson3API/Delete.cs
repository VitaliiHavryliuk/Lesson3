using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.Cosmos.Linq;
using Microsoft.Azure.Cosmos;
using Models.Output;

namespace Lesson3API
{
    public static class Delete
    {
        [FunctionName("Delete")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "{id:guid}")] HttpRequest req,
            [CosmosDB(databaseName:"beer-db",
                containerName:"beer-container",
                Connection = "DBConnection")]
                CosmosClient client,
            Guid id,
            ILogger log)
        {
            log.LogInformation($"Delete function has started!");

            try
            {
                var container = client.GetContainer("beer-db", "beer-container");
                await container.DeleteItemAsync<Beer>($"{id}", new PartitionKey(id.ToString()));             
            }
            catch (Exception ex)
            {
                log.LogError($"Excetion occured in method Delete with message: {ex.Message}");
                return new BadRequestObjectResult(ex);
            }

            return new OkResult();
        }
    }
}
