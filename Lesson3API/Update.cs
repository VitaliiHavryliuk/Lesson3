using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Models.Output;
using System.Collections.Generic;
using System.Linq;
using Models.Input;
using Newtonsoft.Json;
using Microsoft.Azure.Cosmos;
using Lesson3API.Entities;

namespace Lesson3API
{
    public static class Update
    {
        [FunctionName("Update")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "put", Route = null)] HttpRequest req,
            [CosmosDB(databaseName:"beer-db",
                containerName:"beer-container",
                Connection = "DBConnection")]
                CosmosClient client,
            ILogger log)
        {
            log.LogInformation($"Update function has started!");
            var container = client.GetContainer("beer-db", "beer-container");

            UpdateBeer input;
            var body = await new StreamReader(req.Body).ReadToEndAsync();
            
            try
            {
                input = JsonConvert.DeserializeObject<UpdateBeer>(body);

                var updateOperations = new List<PatchOperation> 
                {
                    PatchOperation.Replace("/description",$"{input.Description}"),
                    PatchOperation.Replace("/name", $"{input.Name}")
                };

                await container.PatchItemAsync<Beer>(
                    id: input.Id.ToString(),
                    partitionKey: new PartitionKey(input.Id.ToString()),
                    patchOperations: updateOperations);
            }
            catch (Exception ex)
            {
                log.LogError($"Excetion occured in method Update with message: {ex.Message}");
                return new BadRequestObjectResult(ex);
            }

            return new OkResult();
        }
    }
}
