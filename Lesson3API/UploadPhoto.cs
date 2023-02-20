using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Azure.Storage.Blobs;
using System.Linq;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace Lesson3API
{
    public static class UploadPhoto
    {
        [FunctionName("UploadPhoto")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", "options", Route = "UploadPhoto/{id:guid}")] HttpRequest req,
            Guid id,
            [Blob("vhavryliuk-blob-container", Connection = "AzureWebJobsStorage")]
            BlobContainerClient blobContainer,
            ILogger log)
        {
            log.LogInformation($"UploadPhoto function has started!");

            var file = req.Form.Files.FirstOrDefault();

            if (file?.ContentType != "image/png") return new BadRequestResult();

            var blob = blobContainer.GetBlobClient($"{id}.png");

            await blob.DeleteIfExistsAsync();

            Image img = Image.FromStream(file.OpenReadStream());
            var thumb = img.GetThumbnailImage(300, 600, () => false, IntPtr.Zero);
            using (var stream = new MemoryStream())
            {
                thumb.Save(stream, ImageFormat.Png);
                stream.Position = 0;
                await blob.UploadAsync(stream);
            }
            

            return new OkResult();
        }
    }
}
