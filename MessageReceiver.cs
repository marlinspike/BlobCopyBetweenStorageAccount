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

namespace Funcs_DataMovement
{
    public static class MessageReceiver {
        [FunctionName("MessageReceiver")]
        public static void Run([ServiceBusTrigger("all_files", "mysub", Connection = "JPOServiceBus")]string sbmsg, ILogger log){
            JPOFileInfo fileInfo = JsonConvert.DeserializeObject<JPOFileInfo>(sbmsg);
            var sourceBlobAccount = Environment.GetEnvironmentVariable("source_blob_account"); //Dest Blob Account
            var destBlobAccount = Environment.GetEnvironmentVariable("dest_blob_account"); //Dest Blob Account
            var sourceClient = new BlobServiceClient(sourceBlobAccount);
            var destClient = new BlobServiceClient(destBlobAccount);

            //Get rererence to Source Blob
            var sourceContainer = sourceClient.GetBlobContainerClient(fileInfo.source);
            var sourceBlob = sourceContainer.GetBlobClient(fileInfo.fileName);

            //Get or Create a reference to destination Blob Container and Blob
            var destContainer = destClient.GetBlobContainerClient(fileInfo.destination);
            var destBlob = destContainer.GetBlobClient(fileInfo.fileName);

            CopyBlobAsync(sourceContainer, destContainer, fileInfo.fileName).GetAwaiter().GetResult();

            log.LogInformation($"---- Received message: {JsonConvert.SerializeObject(fileInfo)}");
            log.LogInformation("Got message");
        }


        private static async Task CopyBlobAsync(BlobContainerClient container, BlobContainerClient destContainer, string sourceBlobName) {
            try {
                // Get the name of the first blob in the container to use as the source.
                string blobName = sourceBlobName;

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
                    BlobClient destBlob = destContainer.GetBlobClient(sourceBlobName);//destContainer.GetBlobClient(blob_sas_uri.ToString());

                    // Start the copy operation.
                    await destBlob.StartCopyFromUriAsync(blob_sas_uri);

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
    }

    
}
