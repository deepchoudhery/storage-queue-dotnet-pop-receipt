# .NET 10 Upgrade Plan

### Selected Strategy
**All-at-Once** — All projects upgraded simultaneously in a single operation.
**Rationale**: One standalone .NET Framework 4.7.2 project with no project dependencies.

## Projects

- `dotnet\storage-queue-dotnet-popreceipt\storage-queue-dotnet-popreceipt.csproj`

## Upgrade Options

| Option | Selected | Why |
|--------|----------|-----|
| Upgrade Strategy | All-at-Once | The solution contains one standalone project with no dependency graph to manage. |
| Project Approach | In-place | The classic application has no dependants, so it can replace net472 directly. |
| Unsupported Packages | Resolve Inline | Seven incompatible packages require compatible replacements, including Azure SDK modernization. |
| Unsupported API Handling | Fix Inline | The assessment reports one source-incompatible API, making direct resolution practical. |
| Assembly Binding Redirects | Document and Review Before Removing | Existing redirects include mandatory version conflicts whose intent must be recorded before removal. |
| Test Coverage | Skip | Test generation was not requested; existing validation will be used. |

### 01-prerequisites: Verify the .NET 10 upgrade toolchain

Verify that the repository can use the .NET 10 SDK and identify any solution-level SDK constraints before project conversion begins. Scope is the solution and its single application project; there is no dependency graph, but the current classic project format and packages.config workflow must be accounted for.

Research existing build entry points, installed SDK constraints, and generated artifacts that should remain excluded from source control. Do not change the target framework in this task.

**Done when**: The .NET 10 SDK and solution build entry point are confirmed, and no repository-level SDK constraint blocks the upgrade.

### 02-sdk-style-conversion: Convert the application to SDK-style format

Convert `storage-queue-dotnet-popreceipt.csproj` from its classic format to SDK style while retaining .NET Framework 4.7.2, and migrate packages.config references into the project. Preserve source inclusion, content files, application configuration behavior, and the executable output shape so structural conversion is isolated from framework and API changes.

The assessment found nine NuGet dependencies, manual binding redirects, and legacy Azure packages. Inventory those references during conversion, but defer Azure SDK replacement, package upgrades, redirect removal, and source API changes to the framework-upgrade task.

**Done when**: The SDK-style project restores and builds on net472 with equivalent source/content inputs and no package reference loss.

### 03-net10-azure-sdk: Upgrade the application and Azure SDK dependencies

Upgrade the application in place from net472 to net10.0, resolve all seven incompatible dependencies, update the recommended Newtonsoft.Json dependency, and fix the reported source-incompatible API. Use the appropriate Azure SDK migration skills and compatibility guidance to replace legacy Azure dependencies where needed, especially the `WindowsAzure.Storage` queue API with the supported `Azure.Storage.Queues` client model, while preserving pop-receipt behavior and queue-message semantics.

Review and document the purpose of the existing Newtonsoft.Json and Microsoft.Azure.KeyVault.Core binding redirects before removing obsolete .NET Framework assembly-binding configuration. Research replacements for Microsoft.Azure.KeyVault.Core, Microsoft.ProjectOxford.Face, Microsoft.WindowsAzure.ConfigurationManager, and the legacy OData packages from their actual source usage rather than carrying incompatible packages forward.

**Done when**: The project targets net10.0, restores without dependency conflicts or vulnerable packages, uses supported Azure SDK APIs, contains no obsolete binding redirects, and builds with zero errors and warnings.

### 04-final-validation: Validate the upgraded solution

Validate the complete solution after the atomic upgrade, including a clean restore, warning-free build, and all existing automated tests or repository-provided executable checks. Confirm the upgraded queue sample preserves message insertion, retrieval, update, pop-receipt renewal, and deletion behavior without relying on legacy configuration or storage APIs.

Document any genuinely deferred recommendations without weakening the build or suppressing warnings. Validate that no packages.config files, stale assembly references, or dependency conflicts remain.

**Done when**: The full solution restores and builds with zero errors and warnings, all existing tests/checks pass, and no unresolved dependency conflicts remain.

### 05-review-follow-up: Preserve Face landmarks and update run documentation

Apply the final review correction to preserve the legacy Face request's landmark behavior, and update the repository README for the SDK-style .NET 10 sample. Document the exact build path, configuration sources, preview Face SDK limitation, build-only validation boundary, and the destructive cleanup behavior of the demo without changing its storage workflow.

**Done when**: Face detection explicitly requests landmarks, the README accurately describes building and safely configuring the migrated sample, and the selected project builds with zero errors and warnings.
