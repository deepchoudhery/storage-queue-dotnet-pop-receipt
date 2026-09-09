# .NET Version Upgrade

## Preferences
- **Flow Mode**: Automatic
- **Target Framework**: .NET 10 (LTS)

### Technical Preferences
- Use the appropriate Azure SDK upgrade skills and compatibility guidance when updating Azure SDK dependencies.

## Source Control
- **Source Branch**: deepchoudhery-queue-receipt-net10-trial
- **Working Branch**: upgrade-dotnet-10
- **Commit Strategy**: Single Commit at End
- **Branch Sync**: Auto (Merge)

## Upgrade Options

### Strategy
- Upgrade Strategy: All-at-Once

### Project Structure
- Project Approach: In-place

### Compatibility
- Unsupported Packages: Resolve Inline (7 incompatible packages)
- Unsupported API Handling: Fix Inline

### Modernization
- Assembly Binding Redirects: Document and Review Before Removing

### Reliability
- Test Coverage: Skip

## Strategy
**Selected**: All-at-Once
**Rationale**: The solution contains one standalone project with no project dependency graph.

### Execution Constraints
- Treat the application upgrade as one atomic change set
- Convert the classic project to SDK style on its current framework before changing the TFM
- Upgrade the TFM, Azure SDK dependencies, other packages, and affected APIs together
- Validate the full solution after the atomic upgrade and require a warning-free build
- Run existing tests after the solution builds successfully
