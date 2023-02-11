using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using Lesson3API.Entities;
using Models.Output;
using Azure.Storage.Blobs;
using System.IO;
using System.Security.Claims;
using Microsoft.Extensions.Primitives;
using System.IdentityModel.Tokens.Jwt;
using System;
using System.Text.Json;

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
            [Blob("vhavryliuk-blob-container", Connection = "AzureWebJobsStorage")]
            BlobContainerClient blobContainer,
            ILogger log)
        {
            log.LogInformation($"GetAll function has started!");
            log.LogInformation($"{input.Count()} beers received!");

            string email;
            if(!Utils.TryGetEmailFromRequest(req, out email)) return new UnauthorizedResult();

            var result = input
                .Where(beer => beer.Email == email)
                .Select(beer => new BeerDTO
                {
                    Id = beer.Id,
                    Name = beer.Name,
                    Email = beer.Email,
                    Description = beer.Description
                });
            return new OkObjectResult(result);
        }
    }
} 
