# Package Analysis Feature Design

## Overview

Add a `--analyze` flag to CPMigrate that scans solutions/projects for package issues without modifying files. This provides a standalone analysis mode focused on package cleanup and health.

## Command Interface

**New flag:** `--analyze` (short: `-a`)

**Behavior:**
- Scans solution/projects for package issues without modifying anything
- Works standalone or combined with `-s` / `-p` options
- Outputs a formatted report to the console

**Usage examples:**
```bash
# Analyze current directory
cpmigrate --analyze

# Analyze specific solution
cpmigrate --analyze -s /path/to/solution

# Analyze single project
cpmigrate --analyze -p /path/to/project.csproj
```

**Mutual exclusivity:**
- `--analyze` cannot be combined with `--dry-run`, `--rollback`, or migration
- Can combine with `-s`, `-p` to specify targets

**Output format:**
```
╭─────────────────────────────────────────╮
│         CPMigrate Analysis              │
╰─────────────────────────────────────────╯

Scanned 12 projects, 47 package references

⚠ Version Inconsistencies (3 found)
┌──────────────────────┬──────────────────────────────────┐
│ Package              │ Versions                         │
├──────────────────────┼──────────────────────────────────┤
│ Newtonsoft.Json      │ 13.0.1 (ProjectA), 12.0.3 (Pro.. │
│ Serilog              │ 3.0.0 (ProjectB), 2.12.0 (Pro..  │
└──────────────────────┴──────────────────────────────────┘

✓ No duplicate packages found
✓ No redundant references found

Summary: 3 issues found
```

## Analysis Checks

### 1. Version Inconsistency Analyzer
- Groups all package references by name (case-insensitive)
- Flags packages where different projects use different versions
- Reports: package name, each version found, and which projects use it

### 2. Duplicate Package Analyzer
- Groups packages by lowercase name
- Flags when the same package appears with different casing
- Example: `Newtonsoft.Json` vs `newtonsoft.json` vs `NEWTONSOFT.JSON`
- Reports: the variations found and which projects have them

### 3. Redundant Reference Analyzer
- Scans each project individually
- Flags when the same package is referenced multiple times in one `.csproj`
- This can happen from merge conflicts or copy-paste errors
- Reports: project file and the duplicated package names

## Architecture

```
IAnalyzer (interface)
├── Task<AnalyzerResult> AnalyzeAsync(IEnumerable<ProjectInfo> projects)

AnalysisService
├── Runs all analyzers
├── Aggregates results
├── Returns combined AnalysisReport

MigrationService (existing)
├── New method: ExecuteAnalysisAsync(Options options)
├── Reuses existing project scanning logic
├── Calls AnalysisService instead of migration logic
```

## Data Models

```csharp
// Individual issue found by an analyzer
public record AnalysisIssue(
    string PackageName,
    string Description,
    IReadOnlyList<string> AffectedProjects
);

// Result from a single analyzer
public record AnalyzerResult(
    string AnalyzerName,        // e.g., "Version Inconsistencies"
    IReadOnlyList<AnalysisIssue> Issues
);

// Combined report from all analyzers
public record AnalysisReport(
    int ProjectsScanned,
    int TotalPackageReferences,
    IReadOnlyList<AnalyzerResult> Results
);
```

## Console Output

New methods added to `IConsoleService`:
```csharp
void WriteAnalysisHeader();
void WriteAnalyzerResult(AnalyzerResult result);
void WriteAnalysisSummary(AnalysisReport report);
```

## Exit Codes

- `0` - Analysis completed, no issues found
- `1` - Analysis completed, issues found (CI-friendly)
- Non-zero for errors (file not found, etc.)

## Testing Strategy

### Unit Tests

```csharp
// VersionInconsistencyAnalyzer tests
- DetectsPackagesWithDifferentVersions()
- ReturnsEmptyWhenAllVersionsMatch()
- GroupsPackageNamesCaseInsensitively()

// DuplicatePackageAnalyzer tests
- DetectsDifferentCasingOfSamePackage()
- ReturnsEmptyWhenNoDuplicates()
- ReportsAllCasingVariations()

// RedundantReferenceAnalyzer tests
- DetectsMultipleReferencesInSameProject()
- ReturnsEmptyWhenNoRedundantRefs()
- HandlesMultipleProjectsIndependently()
```

### Integration Tests

```csharp
- AnalyzeFlag_ScansProjectsAndReturnsReport()
- AnalyzeFlag_ReturnsExitCode1_WhenIssuesFound()
- AnalyzeFlag_ReturnsExitCode0_WhenNoIssues()
```

## Files to Create/Modify

| File | Action |
|------|--------|
| `CPMigrate/Options.cs` | Add `--analyze` flag with validation |
| `CPMigrate/Models/AnalysisIssue.cs` | Create |
| `CPMigrate/Models/AnalyzerResult.cs` | Create |
| `CPMigrate/Models/AnalysisReport.cs` | Create |
| `CPMigrate/Analyzers/IAnalyzer.cs` | Create interface |
| `CPMigrate/Analyzers/VersionInconsistencyAnalyzer.cs` | Create |
| `CPMigrate/Analyzers/DuplicatePackageAnalyzer.cs` | Create |
| `CPMigrate/Analyzers/RedundantReferenceAnalyzer.cs` | Create |
| `CPMigrate/Services/AnalysisService.cs` | Create |
| `CPMigrate/Services/IConsoleService.cs` | Add analysis output methods |
| `CPMigrate/Services/SpectreConsoleService.cs` | Implement analysis output |
| `CPMigrate/Services/MigrationService.cs` | Add ExecuteAnalysisAsync |
| `CPMigrate/Program.cs` | Route --analyze to analysis flow |
| `CPMigrate.Tests/Tests.cs` | Add analyzer and integration tests |
| `README.md` | Document --analyze flag |

## Future Enhancements

Once this foundation is in place, future versions could add:
- **Package Health:** Outdated package detection via NuGet API
- **Vulnerability Scanning:** Known CVE detection via NuGet vulnerability database
- **JSON/XML output:** Machine-readable reports for CI pipelines
