using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Azure.Storage.Blobs;
using System.IO;

namespace Lesson3API
{
    public static class GetPhoto
    {
        [FunctionName("GetById")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "GetPhoto/{id:guid}")] HttpRequest req, Guid id,           
            [Blob("vhavryliuk-blob-container", Connection = "AzureWebJobsStorage")]
            BlobContainerClient blobContainer,
            ILogger log)
        {
            log.LogInformation($"GetPhoto function has started!");

            var blob = blobContainer.GetBlobClient($"{id}.png");

            if (!await blob.ExistsAsync()) return new NotFoundResult();

            using var stream = new MemoryStream();
            await blob.DownloadToAsync(stream);

            return new FileContentResult(stream.ToArray(), "image/png");
        }
    }
}
