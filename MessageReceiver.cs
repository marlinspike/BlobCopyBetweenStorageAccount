using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Funcs_DataMovement.Models;
using Newtonsoft.Json;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Specialized;
using Azure.Storage.Blobs.Models;
using System.Threading.Tasks;
using Azure;
using Azure.Storage.Sas;
using Funcs_DataMovement.Utils;
using System.Collections.Generic;
using System.Linq;

namespace Funcs_DataMovement{
    public static class MessageReceiver {
        [FunctionName("MessageReceiver")]
        [return: CosmosDB(databaseName: "jpo", collectionName: "logdb", ConnectionStringSetting = "cosmosdb_log")]
        public static async Task<LogItem>  Run(
            [ServiceBusTrigger("all_files", "mysub", Connection = "JPOServiceBus")] string sbmsg, ILogger log) {
            JPOFileInfo fileInfo = JsonConvert.DeserializeObject<JPOFileInfo>(sbmsg);
            string out_container = Environment.GetEnvironmentVariable("outgoing_container"); //Source Container name
            string in_container = Environment.GetEnvironmentVariable("incoming_container"); //Dest Container name
            
            var sourceBlobAccount = Environment.GetEnvironmentVariable(fileInfo.source); //Source Blob Account
            var destBlobAccount = Environment.GetEnvironmentVariable(fileInfo.destination); //Dest Blob Account
            var sourceClient = new BlobServiceClient(sourceBlobAccount);
            var destClient = new BlobServiceClient(destBlobAccount);

            //Get rererence to Source Blob
            var sourceContainer = sourceClient.GetBlobContainerClient(out_container + fileInfo.source);
            var sourceBlob = sourceContainer.GetBlobClient(fileInfo.fileName);
            
            //Get or Create a reference to destination Blob Container and Blob
            var destContainer = destClient.GetBlobContainerClient(in_container + fileInfo.destination);
            var destBlob = destContainer.GetBlobClient(out_container + fileInfo.fileName);

            await CopyBlobAsync(sourceContainer, destContainer, fileInfo);

            var document = (new LogItem() {
                destination = fileInfo.destination,
                source = fileInfo.source,
                fileName = fileInfo.fileName,
                operation = "MessageReceiver",
                timestamp = DateTime.Now,
                customerID = fileInfo.customerID,
                tags = fileInfo.tags,
                version = Environment.GetEnvironmentVariable("version")
            });

            log.LogInformation($"---- Received message: {JsonConvert.SerializeObject(fileInfo)}");
            return document;
        }


        private static async Task CopyBlobAsync(BlobContainerClient container, BlobContainerClient destContainer, JPOFileInfo fileInfo) {
            try {
                // Get the name of the first blob in the container to use as the source.
                string blobName = fileInfo.fileName;

                // Create a BlobClient representing the source blob to copy.
                BlobClient sourceBlob = container.GetBlobClient(blobName);

                // Ensure that the source blob exists.
                if (await sourceBlob.ExistsAsync()) {
                    // Lease the source blob for the copy operation to prevent another client from modifying it.
                    BlobLeaseClient lease = sourceBlob.GetBlobLeaseClient();

                    // Specifying -1 for the lease interval creates an infinite lease.
                    //await lease.AcquireAsync(TimeSpan.FromSeconds(100));

                    // Get the source blob's properties and display the lease state.
                    BlobProperties sourceProperties = await sourceBlob.GetPropertiesAsync();
                    Console.WriteLine($"Lease state: {sourceProperties.LeaseState}");

                    Uri blob_sas_uri = BlobUtilities.GetServiceSASUriForBlob(sourceBlob, container.Name, null);

                    // Get a BlobClient representing the destination blob
                    BlobClient destBlob = destContainer.GetBlobClient(fileInfo.fileName);//destContainer.GetBlobClient(blob_sas_uri.ToString());

                    var dict = new Dictionary<string, string>();
                    foreach (var (tag, index) in fileInfo.tags.Split(",").WithIndex()) {
                        dict.Add($"tag{index}", tag.Trim());
                    }
                    dict.Add("source", fileInfo.source);
                    dict.Add("description", fileInfo.description);
                    var options = new BlobCopyFromUriOptions {
                        Metadata = dict
                    };
  
                    // Start the copy operation.
                    await destBlob.StartCopyFromUriAsync(blob_sas_uri, options);

                    // Get the destination blob's properties and display the copy status.
                    BlobProperties destProperties = await destBlob.GetPropertiesAsync();

                    // Update the source blob's properties.
                    sourceProperties = await sourceBlob.GetPropertiesAsync();

                    if (sourceProperties.LeaseState == LeaseState.Leased) {
                        // Break the lease on the source blob.
                        await lease.BreakAsync();
                        // Update the source blob's properties to check the lease state.
                        sourceProperties = await sourceBlob.GetPropertiesAsync();
                    }
                }
            }
            catch (RequestFailedException ex) {
                Console.WriteLine(ex.Message);
                Console.ReadLine();
                throw;
            }
        }
        public static IEnumerable<(T item, int index)> WithIndex<T>(this IEnumerable<T> source) {
            return source.Select((item, index) => (item, index));
        }
    }

    
}
