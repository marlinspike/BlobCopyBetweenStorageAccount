using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using System;
using System.Collections.Generic;
using System.Text;

namespace Funcs_DataMovement.Utils {
    class BlobUtilities {
        public static Uri GetServiceSASUriForContainer(BlobContainerClient containerClient, string storedPolicyName = null) {
            // Check whether this BlobContainerClient object has been authorized with Shared Key.
            if (containerClient.CanGenerateSasUri) {
                // Create a SAS token that's valid for one hour.
                BlobSasBuilder sasBuilder = new BlobSasBuilder() {
                    BlobContainerName = containerClient.Name,
                    Resource = "c"
                };

                if (storedPolicyName == null) {
                    sasBuilder.ExpiresOn = DateTimeOffset.UtcNow.AddHours(1);
                    sasBuilder.SetPermissions(BlobContainerSasPermissions.Read);
                }
                else {
                    sasBuilder.Identifier = storedPolicyName;
                }

                Uri sasUri = containerClient.GenerateSasUri(sasBuilder);
                Console.WriteLine("SAS URI for blob container is: {0}", sasUri);
                Console.WriteLine();

                return sasUri;
            }
            else {
                Console.WriteLine(@"BlobContainerClient must be authorized with Shared Key 
                          credentials to create a service SAS.");
                return null;
            }
        }

        public static Uri GetServiceSASUriForBlob(BlobClient blobClient, string containerName, string storedPolicyName = null) {
            // Check whether this BlobClient object has been authorized with Shared Key.
            if (blobClient.CanGenerateSasUri) {
                // Create a SAS token that's valid for one hour.
                BlobSasBuilder sasBuilder = new BlobSasBuilder() {
                    BlobContainerName = containerName,
                    BlobName = blobClient.Name,
                    Resource = "b"
                };

                if (storedPolicyName == null) {
                    sasBuilder.ExpiresOn = DateTimeOffset.UtcNow.AddHours(1);
                    sasBuilder.SetPermissions(BlobSasPermissions.Read |
                        BlobSasPermissions.Write);
                }
                else {
                    sasBuilder.Identifier = storedPolicyName;
                }

                Uri sasUri = blobClient.GenerateSasUri(sasBuilder);
                Console.WriteLine("SAS URI for blob is: {0}", sasUri);
                Console.WriteLine();

                return sasUri;
            }
            else {
                Console.WriteLine(@"BlobClient must be authorized with Shared Key 
                          credentials to create a service SAS.");
                return null;
            }
        }
    }
}
