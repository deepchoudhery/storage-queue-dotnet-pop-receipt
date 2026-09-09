# 01-prerequisites: Verify the .NET 10 upgrade toolchain

Verify that the repository can use the .NET 10 SDK and identify any solution-level SDK constraints before project conversion begins. Scope is the solution and its single application project; there is no dependency graph, but the current classic project format and packages.config workflow must be accounted for.

Research existing build entry points, installed SDK constraints, and generated artifacts that should remain excluded from source control. Do not change the target framework in this task.

**Done when**: The .NET 10 SDK and solution build entry point are confirmed, and no repository-level SDK constraint blocks the upgrade.

## Research Findings

- The active branch is `deepchoudhery-queue-receipt-net10-trial`, as required for this trial.
- Installed .NET 10 SDKs include `10.0.203` and `10.0.303`.
- No `global.json` constrains SDK selection.
- The build entry point is `dotnet\storage-queue-dotnet-popreceipt.sln`.
- The solution contains one classic .NET Framework 4.7.2 project using `packages.config`.
- Generated `bin`, `obj`, package, log, and IDE outputs are covered by the repository `.gitignore`.
- No repository-level SDK constraint blocks the planned .NET 10 upgrade.
