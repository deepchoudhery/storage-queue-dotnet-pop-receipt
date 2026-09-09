# 03-net10-azure-sdk: Upgrade the application and Azure SDK dependencies

Upgrade the application in place from net472 to net10.0, resolve all seven incompatible dependencies, update the recommended Newtonsoft.Json dependency, and fix the reported source-incompatible API. Use the appropriate Azure SDK migration skills and compatibility guidance to replace legacy Azure dependencies where needed, especially the `WindowsAzure.Storage` queue API with the supported `Azure.Storage.Queues` client model, while preserving pop-receipt behavior and queue-message semantics.

Review and document the purpose of the existing Newtonsoft.Json and Microsoft.Azure.KeyVault.Core binding redirects before removing obsolete .NET Framework assembly-binding configuration. Research replacements for Microsoft.Azure.KeyVault.Core, Microsoft.ProjectOxford.Face, Microsoft.WindowsAzure.ConfigurationManager, and the legacy OData packages from their actual source usage rather than carrying incompatible packages forward.

**Done when**: The project targets net10.0, restores without dependency conflicts or vulnerable packages, uses supported Azure SDK APIs, contains no obsolete binding redirects, and builds with zero errors and warnings.

## Research Findings

- `WindowsAzure.Storage` is used for queues, blobs, and tables, so the actual Track 2 replacement set is `Azure.Storage.Queues`, `Azure.Storage.Blobs`, and `Azure.Data.Tables`; `Azure.Storage.Common` alone is not a client replacement.
- Queue behavior depends on Base64 encoding, a 900-second initial visibility delay, and deletion with the `MessageId` and `PopReceipt` returned by the send operation.
- Blob uploads must explicitly use overwrite behavior in the Track 2 API.
- Table writes use `InsertOrReplace`; the Track 2 equivalent is replace-mode upsert.
- Storage authentication uses a connection string and must remain connection-string based.
- `Microsoft.ProjectOxford.Face` is used directly for face detection and age attributes. The modern client requires both a subscription key and a service endpoint, so configuration must expose both without embedding credentials.
- `Microsoft.WindowsAzure.ConfigurationManager` is used only for app settings; modern .NET can use `System.Configuration.ConfigurationManager`.
- The legacy OData, Spatial, KeyVault Core, and Newtonsoft.Json references are indirect implementation dependencies of the old storage stack and have no direct source usage.
- Existing assembly binding redirects only support legacy package version resolution and are obsolete after the Track 2/net10.0 migration.

## Behavior-Preservation Checklist

- Queue payload encoding remains Base64.
- Queue messages remain initially invisible for 900 seconds.
- Queue deletion uses the send result's message ID and pop receipt and happens only after blob/table success.
- Blob upload overwrites an existing blob.
- Table upsert replaces the existing entity.
- Face detection and age extraction remain part of image processing.
