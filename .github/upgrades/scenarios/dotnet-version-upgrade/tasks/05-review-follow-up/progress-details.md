# Progress Details

Closed the two final review gaps without changing the migrated Storage workflow.

- Updated the Face `DetectAsync` call to explicitly preserve `returnFaceLandmarks: true`
- Kept `returnFaceId: false` and the requested `FaceAttributeType.Age`
- Replaced legacy README run instructions with the .NET 10 SDK requirement, scoped Release build command, and current output layout
- Documented `StorageConnectionString`, `FaceAPIEndpoint`, and `FaceAPIKey` as `App.config` settings, including the changed Face credential source
- Documented the preview Face SDK and live-service capability limitations
- Documented that Azurite does not emulate Face
- Added an explicit warning that cleanup deletes `samplequeue`, `samplecontainer`, and `sampletable`, including pre-existing resources with those names
- Recorded that validation is build-only and that the repository has no test project

Validation: the selected project built in Release with warnings treated as errors and completed with zero warnings and zero errors. The application and all live services were not run.
