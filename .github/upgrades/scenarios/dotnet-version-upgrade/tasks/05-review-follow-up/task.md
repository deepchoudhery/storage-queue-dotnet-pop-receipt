# 05-review-follow-up: Preserve Face landmarks and update run documentation

Apply the final review correction to preserve the legacy Face request's landmark behavior, and update the repository README for the SDK-style .NET 10 sample. Document the exact build path, configuration sources, preview Face SDK limitation, build-only validation boundary, and the destructive cleanup behavior of the demo without changing its storage workflow.

**Done when**: Face detection explicitly requests landmarks, the README accurately describes building and safely configuring the migrated sample, and the selected project builds with zero errors and warnings.

## Research Findings

- The migrated `DetectAsync` call already preserves `returnFaceId: false` and requests `FaceAttributeType.Age`, but omits the optional landmark argument; the legacy call explicitly passed `returnFaceLandmarks: true`.
- The repository README still describes the classic project, Azure Storage Emulator, `bin\Debug`, and Visual Studio F10 workflow.
- The SDK-style project targets `net10.0`; a Release build writes to `dotnet\storage-queue-dotnet-popreceipt\bin\Release\net10.0`.
- Runtime settings are read from `App.config` through `ConfigurationManager.AppSettings`: `StorageConnectionString`, `FaceAPIEndpoint`, and `FaceAPIKey`.
- The demo's cleanup path deletes the fixed resource names `samplequeue`, `samplecontainer`, and `sampletable`, including resources that existed before the run.
- Validation remains build-only because the solution contains no test project and live service execution is prohibited.
