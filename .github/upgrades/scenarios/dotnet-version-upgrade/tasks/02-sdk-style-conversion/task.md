# 02-sdk-style-conversion: Convert the application to SDK-style format

Convert `storage-queue-dotnet-popreceipt.csproj` from its classic format to SDK style while retaining .NET Framework 4.7.2, and migrate packages.config references into the project. Preserve source inclusion, content files, application configuration behavior, and the executable output shape so structural conversion is isolated from framework and API changes.

The assessment found nine NuGet dependencies, manual binding redirects, and legacy Azure packages. Inventory those references during conversion, but defer Azure SDK replacement, package upgrades, redirect removal, and source API changes to the framework-upgrade task.

**Done when**: The SDK-style project restores and builds on net472 with equivalent source/content inputs and no package reference loss.

## Research Findings

- The project is a classic executable project with root namespace and assembly name `PopreceiptSample`.
- It explicitly compiles `Program.cs` and `Properties\AssemblyInfo.cs`; SDK default compile inclusion can replace those entries, but generated assembly attributes must remain disabled.
- `App.config` must continue to be transformed into the executable configuration file.
- `packages.config` contains nine direct packages, all mirrored by assembly hint paths in the project.
- The source and configuration changes for Azure Track 2 are intentionally deferred to task `03-net10-azure-sdk`.
- Conversion will retain `net472`, `OutputType=Exe`, AnyCPU behavior, existing assembly metadata, and all nine package versions.
