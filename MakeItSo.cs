using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Funcs_DataMovement.Models;
using Microsoft.Azure.WebJobs.ServiceBus;

namespace Funcs_DataMovement {
    public static class MakeItSo {
        [FunctionName("MakeItSo")]
        public static IActionResult Run([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req, ILogger log,
            [ServiceBus("all_files", Connection = "JPOServiceBus", EntityType = EntityType.Queue)] out string queueMessage) {
            log.LogInformation("C# HTTP trigger function processed a request.");

            //string command = req.Query["cmd"];

            string requestBody = String.Empty;
            using (StreamReader streamReader = new StreamReader(req.Body)) {
                requestBody =  streamReader.ReadToEnd();
            }
            JPOFileInfo fileInfo = JsonConvert.DeserializeObject<JPOFileInfo>(requestBody);
            queueMessage = JsonConvert.SerializeObject(fileInfo);

            return new OkObjectResult("GotIt!");
        }
    }
}
