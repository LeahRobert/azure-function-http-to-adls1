// https://www.cyotek.com/blog/upload-data-to-blob-storage-with-azure-functions

using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System.Text;

namespace BlobFunction
{
    public static class BlobFunction
    {
        private static string accountName = "leahblob";
        private static string accessKey = "bmGwmPwbvCVFThA7GI2CmzEgmz4fwlyBzeq9S7y6Qme8+l8Ej+BO3JeHTOwDbOqRA2x8fFdYXqp9f8TY1mFa6g==";
        private static string connectionString = "DefaultEndpointsProtocol=https;AccountName=" + accountName + ";AccountKey=" + accessKey + ";EndpointSuffix=core.windows.net";
        private static CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);

        [FunctionName("BlobFunction")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req, ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");
            
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            requestBody = requestBody.Replace(System.Environment.NewLine, " ");
            //dynamic data = JsonConvert.DeserializeObject(requestBody);
            //name = name ?? data?.name;

            string fileName = "blob-yvr-test";

            await AppendBlob(fileName, requestBody, log);

            return new OkObjectResult("Success");
        }

        private async static Task AppendBlob(string name, string data, ILogger log)
        {
            string accountName = "leahblob";
            string accessKey = "bmGwmPwbvCVFThA7GI2CmzEgmz4fwlyBzeq9S7y6Qme8+l8Ej+BO3JeHTOwDbOqRA2x8fFdYXqp9f8TY1mFa6g==";
            string connectionString = "DefaultEndpointsProtocol=https;AccountName=" + accountName + ";AccountKey=" + accessKey + ";EndpointSuffix=core.windows.net";
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);
            CloudBlobClient client;
            CloudBlobContainer container;
            CloudAppendBlob blob;

            client = storageAccount.CreateCloudBlobClient();
            container = client.GetContainerReference("yvrblob");
            await container.CreateIfNotExistsAsync();

            blob = container.GetAppendBlobReference(name);

            //bool blobExists = await blob.ExistsAsync();

            if (!(await blob.ExistsAsync()))
            {
                await blob.CreateOrReplaceAsync();
            }

            //blob.Properties.ContentType = "application/json";

            using (Stream stream = new MemoryStream(Encoding.UTF8.GetBytes(data)))
            {
                //await blob.UploadFromStreamAsync(stream);
                await blob.AppendTextAsync(data);
                log.LogInformation("Appended Data");
            }
        }

    }
}
