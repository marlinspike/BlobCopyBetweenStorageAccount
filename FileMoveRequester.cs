using System;
using System.IO;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.WebJobs.ServiceBus;
using Newtonsoft.Json;

using Funcs_DataMovement.Models;

namespace Funcs_DataMovement
{
    public static class FileMoveRequester {
        //[FunctionName("FileMoveRequester")]
        //[return: ServiceBus("all_files", Connection = "JPOServiceBus")] 
        //public static string Run([BlobTrigger("outbox/{name}", Connection = "jpovirginia")]Stream myBlob, string name, ILogger log){
        //    log.LogInformation($"C# Blob trigger function Processed blob\n Name:{name} \n Size: {myBlob.Length} Bytes");
        //    var msg = new JPOFileInfo {
        //        source = "jpovirginia",
        //        destination = "jpoiowa",//"jpoarizona",
        //        tags = "tag1, tag2, tag3",
        //        origin = "Elvis",
        //        description = "Return to sender",
        //        date = DateTime.Now,
        //        fileName = name
        //    };
        //    return JsonConvert.SerializeObject(msg);
        //}
    }
}
