## Copy Blobs using a Service Bus Queue Mediator
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
    }

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
    "FUNCTIONS_WORKER_RUNTIME": "dotnet",
    "outgoing_container": "outbox/",
    "incoming_container": "inbox/"
  }
}
</pre>

### How to test
Once running either locally or in Azure:
* Upload a blob to Origin Blob Storage. FileMoveRequester fires and creates a message on the all_files topic
* Message Receiver fires for each message and moves the file requested from origin to destination storage account. A Blob SAS token is created with a 1 hour ttl.



