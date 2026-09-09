# Progress Details

Upgraded the application to `net10.0` and replaced the legacy Azure Storage stack with current data-plane clients.

## Skill Guidance Applied

Successfully read and applied the installed `migrating-azure-sdk-to-track2` skill and these complete references:

- `ref\track2-contract.md`
- `ref\package-catalog.md`
- `ref\authentication.md`
- `ref\behavior-audit.md`

Management-plane guidance was excluded because this sample uses Storage and Face data-plane APIs only.

## Changes

- Replaced `WindowsAzure.Storage` with `Azure.Storage.Queues` 12.27.1, `Azure.Storage.Blobs` 12.29.2, and `Azure.Data.Tables` 12.12.0
- Replaced `Microsoft.WindowsAzure.ConfigurationManager` with `System.Configuration.ConfigurationManager` 10.0.12
- Replaced `Microsoft.ProjectOxford.Face` with `Azure.AI.Vision.Face` 1.0.0-beta.2
- Removed unused legacy OData, Spatial, KeyVault Core, and Newtonsoft.Json dependencies
- Removed obsolete .NET Framework binding redirects
- Added the Face service endpoint as a non-secret application setting
- Retained connection-string authentication for Storage

`Azure.AI.Vision.Face` has no stable NuGet release; beta.2 is the available modern SDK required to retain the Face detection feature. This prerelease limitation was not hidden or replaced with a live-service workaround.

## Behavior Audit

| Package or type | Shape | Outcome | Evidence | Action taken |
|---|---|---|---|---|
| `QueueClient` | External contract | Preserved | `QueueClientOptions.MessageEncoding` is explicitly `Base64` | Prevented the Track 2 default from changing message bytes |
| `QueueClient` | Operation contract | Preserved | `SendMessageAsync` retains the 900-second visibility timeout and its `SendReceipt` supplies deletion tokens | Deletion uses `sendReceipt.MessageId` and `sendReceipt.PopReceipt` only after blob/table verification |
| `QueueClient` | Boundary and topology | Preserved | The client remains bound to `samplequeue` | Replaced the account/client/reference chain with one resource-bound client |
| `QueueClient` | Operational | Preserved | The existing Storage connection string is passed to the client and the client is reused | Authentication mode and client lifetime remain equivalent |
| `BlobClient` | External contract | Preserved | The same source file stream/path is uploaded without application serialization | Blob bytes remain unchanged |
| `BlobClient` | Operation contract | Preserved | `UploadAsync(currentFile, overwrite: true)` | Existing blobs are overwritten explicitly |
| `BlobClient` | Boundary and topology | Preserved | The container is `samplecontainer` and each blob client is bound to the original file name | Resource binding remains equivalent |
| `BlobClient` | Operational | Preserved | The connection-string container client is created once and reused | Authentication and lifetime remain equivalent |
| `TableClient` | External contract | Preserved | Partition key `FaceImages`, row key, and `personN` age string properties are retained | Persisted entity shape remains equivalent |
| `TableClient` | Operation contract | Preserved | `UpsertEntityAsync(..., TableUpdateMode.Replace)` | Preserved legacy `InsertOrReplace` replacement semantics |
| `TableClient` | Boundary and topology | Preserved | The client remains bound to `sampletable` | Replaced the account/client/reference chain with one resource-bound client |
| `TableClient` | Operational | Preserved | The existing Storage connection string is used and the client is reused | Authentication mode and lifetime remain equivalent |
| `FaceClient` | External contract | Preserved | Face count and age attributes still populate the same table properties | Retained face detection and age extraction |
| `FaceClient` | Operation contract | Preserved | Detection requests `FaceAttributeType.Age` using the modern async API | Retained the feature with explicit detection/recognition models |
| `FaceClient` | Boundary and topology | Changed deliberately | Modern Face clients bind to a service endpoint at construction | Added `FaceAPIEndpoint` configuration alongside the existing key |
| `FaceClient` | Operational | Unchecked | The modern package is prerelease and no live service call was permitted | Build validated only; production Face availability and credentials were not exercised |

## Validation

- Project build: succeeded with zero errors and zero warnings
- Vulnerable packages: none detected
- Deprecated packages: none detected
- Repository-wide legacy Azure reference audit: no active application or build references remain
- Tests: none exist in the solution
- Application and live Azure/Face services: not run
