﻿## Copy Blobs using a Service Bus Queue Mediator
These Azure Functions copy files between storage accounts using a Service Bus queue as a mediator

### Resources required 
1. Storage Account for Azure Functions: *jpodefault*
2. Origin Blob Storage:
	* Storage Account for origin blobs: *jpovirginia*
    * Container within this storage account for incoming blobs: *inbox*
	* Container within this storage account for outgoing blobs: *outbox*
3. Destination Blob Storage
	* Storage account for destination blobs: *jpoarizona*
    * Container within this storage account for incoming blobs: *inbox*
	* Container within this storage account for outgoing blobs: *outbox*
4. Destination Blob Storage
	* Storage account for destination blobs: *jpoiowa*
    * Container within this storage account for incoming blobs: *inbox*
	* Container within this storage account for outgoing blobs: *outbox*
5. Service Bus Namespace: *jposervicebus*
	* Topic: *all_files*
	* Subscription within this topic: *mysub*
5. Cosmos DB Account
    * Database name: jpo
    * Collection Name: logdb


### Payload
The payload for the message is a JSON file that provides the following controls:
<pre>
    public class JPOFileInfo {
        public string source { get; set; }  //Source Container Name
        public string destination { get; set; } //Destination Container name
        public string tags { get; set; }
        public string origin { get; set; }
        public string fileName { get; set; }
        public DateTime date { get; set; }
        public string description { get; set; }
        public string customerID { get; set; }
    }
</pre>
The tags can be used for managing flow. The JSON payload looks like:

"{"source":"outbox","destination":"inbox","tags":"tag1, tag2, tag3","origin":"Elvis","fileName":"win10-vs.rdp","date":"2021-03-17T09:55:46.9883908-04:00","description":"Return to sender"}"


### localsettings.json
Edit this localsettings.json file to replace your own. Replace <YOUR_KEY_HERE> with values from your deployed resources.
<pre>
{
    "IsEncrypted": false,
  "Values": {
    "AzureWebJobsStorage": "DefaultEndpointsProtocol=https;AccountName=jpodefault;AccountKey=<YOUR_KEY_HERE>;EndpointSuffix=core.usgovcloudapi.net", //DefaultEndpointsProtocol=https;AccountName=rcdev1;AccountKey=kaqcWkmMv+51mwlw19bnhu4+a7rk5YnzwcXHqvG1ambF3mF6jkzRQwUbvihoPh7+WL1p5V6YE15DoCKyk86IgQ==;BlobEndpoint=https://rcdev1.blob.core.windows.net/;TableEndpoint=https://rcdev1.table.core.windows.net/;QueueEndpoint=https://rcdev1.queue.core.windows.net/;FileEndpoint=https://rcdev1.file.core.windows.net/",
    "AccountMonitored": "DefaultEndpointsProtocol=https;AccountName=jpovirginia;AccountKey=<YOUR_KEY_HERE>;EndpointSuffix=core.usgovcloudapi.net",
    "JPOServiceBus": "Endpoint=sb://jposervicebus.servicebus.usgovcloudapi.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey<YOUR_KEY_HERE>",
    "receive_topic": "Endpoint=sb://jposervicebus.servicebus.usgovcloudapi.net/;SharedAccessKeyName=receive;SharedAccessKey=<YOUR_KEY_HERE>",
    "jpovirginia": "DefaultEndpointsProtocol=https;AccountName=jpovirginia;AccountKey=<YOUR_KEY_HERE>;EndpointSuffix=core.usgovcloudapi.net",
    "jpoarizona": "DefaultEndpointsProtocol=https;AccountName=jpoarizona;AccountKey=<YOUR_KEY_HERE>;EndpointSuffix=core.usgovcloudapi.net",
    "jpoiowa": "DefaultEndpointsProtocol=https;AccountName=jpoiowa;AccountKey=<YOUR_KEY_HERE>;EndpointSuffix=core.usgovcloudapi.net",
    "cosmosdb_log": "AccountEndpoint=<YOUR_KEY_HERE>",
    "FUNCTIONS_WORKER_RUNTIME": "dotnet",
    "outgoing_container": "outbox/",
    "incoming_container": "inbox/",
    "version": "0.2"
  }
}
</pre>

### How to test
Once running either locally or in Azure:
1. Upload one or more blobs to the **inbox** Container in any of the Storage Accounts
2. Use Postman or another tool configure a **PUT** request to the URL for the **MakeItSo** Function, and add the following JSON Payload:
<pre>
{
	"source":"jpoiowa",
	"destination":"jpovirginia",
	"tags":"tag1, tag2, tag3",
	"fileName":"Hello World - 3.txt",
	"date":"2021-03-17T09:55:46.9883908-04:00",
	"description":"file transfer",
	"customerID":"1"
}

</pre>
3. Modify the *source* and *destination* here to be one of the values in the local.settings.json file (here it is jpovirginia, jpoarizona or jpoiowa)
4. Send the Put request
5. Verify that the file from the *inbox* container of the source storage account has been moved to the *outbox* container of the destination storage account
6. Verify that the tags have been appended to the blobs's metadata. 
7. Verify that the blob metadata contains the 'source' tag

### Screenshots
## Flow
![alt text](https://raw.githubusercontent.com/marlinspike/BlobCopyBetweenStorageAccount/master/img/JPOMessageHandling.png)

## HTTP Put Request with JSON payload sent via Postman
![alt text](https://raw.githubusercontent.com/marlinspike/BlobCopyBetweenStorageAccount/master/img/JSONRequest.png)

## Storage Account
![alt text](https://raw.githubusercontent.com/marlinspike/BlobCopyBetweenStorageAccount/master/img/StorageAccount.png)

## Storage Account
Power BI Report
![alt text](https://raw.githubusercontent.com/marlinspike/BlobCopyBetweenStorageAccount/master/img/jpo_pbi.jpg)