# 04-final-validation: Validate the upgraded solution

Validate the complete solution after the atomic upgrade, including a clean restore, warning-free build, and all existing automated tests or repository-provided executable checks. Confirm the upgraded queue sample preserves message insertion, retrieval, update, pop-receipt renewal, and deletion behavior without relying on legacy configuration or storage APIs.

Document any genuinely deferred recommendations without weakening the build or suppressing warnings. Validate that no packages.config files, stale assembly references, or dependency conflicts remain.

**Done when**: The full solution restores and builds with zero errors and warnings, all existing tests/checks pass, and no unresolved dependency conflicts remain.

## Research Findings

- The solution contains one `net10.0` executable project and no test projects.
- Validation must cover the full solution rather than application execution.
- The required checks are a clean restore/build with warnings as errors, package vulnerability/deprecation inspection, and a repository-wide legacy-reference scan.
- Live Storage and Face behavior cannot be exercised in this build-only trial; the static behavior audit is recorded in task `03-net10-azure-sdk`.
