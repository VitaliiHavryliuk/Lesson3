using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Azure.Storage.Blobs;

namespace Lesson3API
{
    public static class DeletePhoto
    {
        [FunctionName("DeletePhoto")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", "options", Route = "DeletePhoto/{id:guid}")] HttpRequest req,
            Guid id,
            [Blob("vhavryliuk-blob-container", Connection = "AzureWebJobsStorage")]
            BlobContainerClient blobContainer,
            ILogger log)
        {
            log.LogInformation($"Delete function has started!");
            await blobContainer.DeleteBlobIfExistsAsync($"{id}.png");
            return new OkResult();
        }
    }
}
