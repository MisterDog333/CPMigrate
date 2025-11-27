namespace CPMigrate.Models;

/// <summary>
/// Combined report from all analyzers containing the complete analysis results.
/// </summary>
/// <param name="ProjectsScanned">Number of projects that were scanned.</param>
/// <param name="TotalPackageReferences">Total number of package references found across all projects.</param>
/// <param name="Results">Results from each analyzer.</param>
public record AnalysisReport(
    int ProjectsScanned,
    int TotalPackageReferences,
    IReadOnlyList<AnalyzerResult> Results
)
{
    /// <summary>
    /// Returns the total number of issues found across all analyzers.
    /// </summary>
    public int TotalIssues => Results.Sum(r => r.Issues.Count);

    /// <summary>
    /// Returns true if any analyzer found issues.
    /// </summary>
    public bool HasIssues => TotalIssues > 0;
}
