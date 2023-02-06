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
using Models.Input;
using Lesson3API.Entities;

namespace Lesson3API
{
    public static class Add
    {
        [FunctionName("Add")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            [CosmosDB(databaseName:"beer-db",
                containerName:"beer-container",
                Connection = "DBConnection")]
                IAsyncCollector<Beer> output,
            ILogger log)
        {
            log.LogInformation($"Add function has started!");
            var body = await new StreamReader(req.Body).ReadToEndAsync();
            
            CreateBeer input;
            try
            {
                input = JsonConvert.DeserializeObject<CreateBeer>(body);
            }
            catch (Exception ex)
            {
                log.LogError($"Excetion occured in method Add with message: {ex.Message}");
                return new BadRequestObjectResult(ex);
            }

            var newBeer = new Beer
            {
                Description = input.Description,
                Name = input.Name,
                Email = req.HttpContext?.User?.Identity?.Name,
                Id = Guid.NewGuid()
            };

            await output.AddAsync(newBeer);
            return new OkObjectResult(newBeer);
        }
    }
}
