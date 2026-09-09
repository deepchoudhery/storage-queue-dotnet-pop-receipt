---
services: storage, cognitive-services
platforms: dotnet
author: seguler
---

# Azure Storage Queue Service - Quick Sample using Pop Receipt and Face API  

This sample demonstrates how to use the new Popreceipt functionality to coordinate updates across two non-transactional resources. In this case it allows a Blob to be published and metadata about the Blob to be written to a table entity. Prior to uploading the blob, a message is enqueued, the updates are performed to blob and table and if successful the message is deleted using its popreceipt. Any messages remaining in the queue need additional work due to the failures experienced, which can be consumed via a backend worker that is not demonstrated in this sample.    

## Sample App: Image Upload and Face Recognition using Face API from Azure Cognitive Services 

In this sample app, we assume that the user has a number of photos in a local folder that needs to be uploaded to Azure as blobs, and using Face API each person's age in the the photos are estimated and added to a table as an entity. We will be tracking the completion of this process in a queue and a backend worker would ideally later on pick the messages from queue to go through failed processes.

### Here is a quick walktrough of the sample:

1. Create the queue if not created already
2. Create the container if not created already
3. Create the table if not created already
4. Find JPG files in ‘testfolder’
5. For each photo, do steps 6-10:
6. Upload a queue message representing the processing of this photo.  If there is a failure while processing this photo, the existence of this queue message will signal that failure to a background cleanup process (not shown here.)
7. Call the Face API to estimate the age of each person in the photo.
8. Store the age information as an entity in the table.
9. Upload the image to a blob if at least one face is detected.
10. If both the blob and the table entity operation succeeded, delete the message from queue using the pop receipt.

Note: This sample uses asynchronous programming model with Task Parallel Library (TPL) to demonstrate how to call the Storage Service using the storage client libraries asynchronous API's. When used in real applications this approach enables you to improve the responsiveness of your application. Calls to the storage service are prefixed by the await keyword. 

## Running this sample

Install the .NET 10 SDK. From the repository root, restore and build only this sample with:

```powershell
dotnet build .\dotnet\storage-queue-dotnet-popreceipt\storage-queue-dotnet-popreceipt.csproj --configuration Release -warnaserror
```

The SDK-style project writes its Release output to:

```text
dotnet\storage-queue-dotnet-popreceipt\bin\Release\net10.0\
```

Before running, update these `appSettings` in
`dotnet\storage-queue-dotnet-popreceipt\App.config`:

- `StorageConnectionString`: a connection string for a dedicated disposable Azure Storage account, or `UseDevelopmentStorage=true;` for Azurite.
- `FaceAPIEndpoint`: the endpoint URI for the Azure Face resource.
- `FaceAPIKey`: the API key for that Face resource.

The migrated sample reads all three values from `App.config`. In particular, the Face key is no longer read from an environment variable. Do not commit real account keys or connection strings.

Create a `testfolder` under the process working directory and add the JPG files to process. If the application is launched from the Release output directory, that folder is:

```text
dotnet\storage-queue-dotnet-popreceipt\bin\Release\net10.0\testfolder
```

> [!WARNING]
> Running this demo creates and then deletes `samplequeue`, `samplecontainer`, and `sampletable` in the configured Storage account. Cleanup also deletes pre-existing resources with those names. Use only a dedicated disposable account or emulator; never point the sample at production.

Azurite emulates the Storage services only. It does not emulate the Face service, so the sample still needs a reachable Azure Face endpoint and valid Face credentials. The Face client is pinned to the preview package `Azure.AI.Vision.Face` 1.0.0-beta.2 because no stable release is currently available. Face detection, landmarks, and the age attribute depend on the capabilities and access level of the configured Face resource; a successful local build does not prove that live Face requests are available.

Validation for the .NET 10 migration was build-only: the selected project restores and builds with zero warnings and errors, but this repository has no test project and no live Storage or Face integration was exercised.


## More information

[What is a Storage Account](https://docs.microsoft.com/en-us/azure/storage/common/storage-create-storage-account)

[Cognitive Services Face API](https://azure.microsoft.com/en-us/services/cognitive-services/face/)

[Get started with Azure Queue storage using .NET](https://docs.microsoft.com/en-us/azure/storage/storage-dotnet-how-to-use-queues)

[Queue Service REST API Reference](https://docs.microsoft.com/en-us/rest/api/storageservices/fileservices/queue-service-rest-api)

[Azure Storage Queue Service Samples](https://azure.microsoft.com/en-us/resources/samples/?service=storage&term=queue)
