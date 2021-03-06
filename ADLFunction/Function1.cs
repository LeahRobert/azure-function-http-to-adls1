// Code based off Microsoft docs and https://github.com/Azure-Samples/data-lake-store-adls-dot-net-get-started/blob/master/AdlsSDKGettingStarted/Program.cs
// Docs: https://docs.microsoft.com/en-us/azure/data-lake-store/data-lake-store-data-operations-net-sdk#samples

using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

using Microsoft.Rest.Azure.Authentication;
using Microsoft.Azure.DataLake.Store;
using Microsoft.IdentityModel.Clients.ActiveDirectory;

namespace ADLFunction
{
    public static class Function1
    {
        private static string adlsAccountName = "<datalakestore-domain-name>";
        private static string applicationId = "<application-id>";     // Also called client id
        private static string clientSecret = "<client-secret>";
        private static string tenantId = "<tenant-id>";

        [FunctionName("Function1")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function is processing a request.");

            var creds = new ClientCredential(applicationId, clientSecret);
            var clientCreds = ApplicationTokenProvider.LoginSilentAsync(tenantId, creds).GetAwaiter().GetResult();

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);

            // Create ADLS client object
            AdlsClient client = AdlsClient.CreateClient(adlsAccountName, clientCreds);

            try
            {
                string fileName = "/test/demo-test";

                // Create a file - automatically creates any parent directories that don't exist
                // The AdlsOutputStream preserves record boundaries - it does not break records while writing to the store
                /*using (var stream = client.CreateFile(fileName, IfExists.Overwrite))
                {
                    byte[] textByteArray = Encoding.UTF8.GetBytes(requestBody + "\r\n");
                    stream.Write(textByteArray, 0, textByteArray.Length);
                }  */

                // Append to existing file
                using (var stream = client.GetAppendStream(fileName))
                {
                    byte[] textByteArray = Encoding.UTF8.GetBytes("This is the added line.\r\n");
                    stream.Write(textByteArray, 0, textByteArray.Length);
                }

                //Read file contents
                using (var readStream = new StreamReader(client.GetReadStream(fileName)))
                {
                    string line;
                    while ((line = readStream.ReadLine()) != null)
                    {
                        Console.WriteLine(line);
                    }
                }

            }
            catch (AdlsException e)
            {
                PrintAdlsException(e);
            }

            return new OkObjectResult("Success");

            //return name != null
            //    ? (ActionResult)new OkObjectResult($"Hello, {name}")
            //    : new BadRequestObjectResult("Please pass a name on the query string or in the request body");
        }

        private static void PrintAdlsException(AdlsException exp)
        {
            Console.WriteLine("ADLException");
            Console.WriteLine($"   Http Status: {exp.HttpStatus}");
            Console.WriteLine($"   Http Message: {exp.HttpMessage}");
            Console.WriteLine($"   Remote Exception Name: {exp.RemoteExceptionName}");
            Console.WriteLine($"   Server Trace Id: {exp.TraceId}");
            Console.WriteLine($"   Exception Message: {exp.Message}");
            Console.WriteLine($"   Exception Stack Trace: {exp.StackTrace}");
            Console.WriteLine();
        }
    }
}
