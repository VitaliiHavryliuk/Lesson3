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
using Azure.Storage.Blobs;

namespace Lesson3API
{
    public static class Add
    {
        [FunctionName("Add")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", "options", Route = null)] HttpRequest req,
            [CosmosDB(databaseName:"beer-db",
                containerName:"beer-container",
                Connection = "DBConnection")]
                IAsyncCollector<Beer> output,
            [Blob("vhavryliuk-blob-container", Connection = "AzureWebJobsStorage")]
            BlobContainerClient blobContainer,
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

            if(input.Image.Length != 0)
            {
                var blob = blobContainer.GetBlobClient($"{newBeer.Id}.png");
                await blob.UploadAsync(new MemoryStream(input.Image));
            }

            await output.AddAsync(newBeer);
            return new OkObjectResult(newBeer);
        }
    }
}
