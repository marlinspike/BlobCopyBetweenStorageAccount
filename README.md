## Copy Blobs using a Service Bus Queue Mediator
These Azure Functions copy files between storage accounts using a Service Bus queue as a mediator

### Resources required 
1. Storage Account for Azure Functions: jpodefault
2. Origin Blob Storage:
	* Storage Account for origin blobs: jpovirginia
	* Container within this storage account for outgoing blobs: outbox
3. Destination Blob Storage
	* Storage account for destination blobs: deststorage
	* Container within this storage account for incoming blobs: inbox
4. Service Bus Namespace: jposervicebus
	* Topic: all_files
	* Subscription within this topic: mysub


### localsettings.json
Edit this localsettings.json file to replace your own. Replace <YOUR_KEY_HERE> with values from your deployed resources.
<pre>
{
    "IsEncrypted": false,
  "Values": {
    "AzureWebJobsStorage": "DefaultEndpointsProtocol=https;AccountName=jpodefault;AccountKey=<YOUR_KEY_HERE>;EndpointSuffix=core.usgovcloudapi.net", //DefaultEndpointsProtocol=https;AccountName=rcdev1;AccountKey=kaqcWkmMv+51mwlw19bnhu4+a7rk5YnzwcXHqvG1ambF3mF6jkzRQwUbvihoPh7+WL1p5V6YE15DoCKyk86IgQ==;BlobEndpoint=https://rcdev1.blob.core.windows.net/;TableEndpoint=https://rcdev1.table.core.windows.net/;QueueEndpoint=https://rcdev1.queue.core.windows.net/;FileEndpoint=https://rcdev1.file.core.windows.net/",
    "AccountMonitored": "DefaultEndpointsProtocol=https;AccountName=jpovirginia;AccountKey=<YOUR_KEY_HERE>;EndpointSuffix=core.usgovcloudapi.net",
    "JPOServiceBus": "Endpoint=sb://jposervicebus.servicebus.usgovcloudapi.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=<YOUR_KEY_HERE>",
    "receive_topic": "Endpoint=sb://jposervicebus.servicebus.usgovcloudapi.net/;SharedAccessKeyName=receive;SharedAccessKey=<YOUR_KEY_HERE>",
    "source_blob_account": "DefaultEndpointsProtocol=https;AccountName=jpovirginia;AccountKey=<YOUR_KEY_HERE>;EndpointSuffix=core.usgovcloudapi.net",
    "dest_blob_account": "DefaultEndpointsProtocol=https;AccountName=deststorage;AccountKey=<YOUR_KEY_HERE>;EndpointSuffix=core.usgovcloudapi.net",
    "FUNCTIONS_WORKER_RUNTIME": "dotnet"
  }
}
</pre>

### How to test
Once running either locally or in Azure:
* Upload a blob to Origin Blob Storage. FileMoveRequester fires and creates a message on the all_files topic
* Message Receiver fires for each message and moves the file requested from origin to destination storage account. A Blob SAS token is created with a 1 hour ttl.



