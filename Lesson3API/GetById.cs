using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Lesson3API.Entities;
using System.Collections.Generic;
using Azure.Storage.Blobs;
using System.Linq;
using Models.Output;

namespace Lesson3API
{
    public static class GetById
    {
        [FunctionName("GetById")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "{id:guid}")] HttpRequest req, Guid id,
            [CosmosDB(
                databaseName:"beer-db",
                containerName:"beer-container",
                Connection = "DBConnection",
                SqlQuery = "SELECT * FROM c")] IEnumerable<Beer> input,
            [Blob("vhavryliuk-blob-container", Connection = "AzureWebJobsStorage")]
            BlobContainerClient blobContainer,
            ILogger log)
        {
            log.LogInformation($"GetById function has started!");


            var result = input
                .Where(beer => beer.Id == id)
                .Select(beer => new BeerDTO
                {
                    Id = beer.Id,
                    Name = beer.Name,
                    Email = beer.Email,
                    Description = beer.Description
                })
                .FirstOrDefault();

            if (result == null)
            {
                return new NotFoundObjectResult("Beer not found!");
            }

            var blob = blobContainer.GetBlobClient($"{result.Id}.png");

            if (blob != null)
            {
                using (var ms = new MemoryStream())
                {
                    try
                    {
                        blob.DownloadTo(ms);
                    }
                    catch (Exception)
                    {
                    }

                    result.Image = ms.ToArray();
                }
            }

            return new OkObjectResult(result);
        }
    }
}
