namespace CPMigrate.Models;

/// <summary>
/// Result from a single analyzer containing all issues it found.
/// </summary>
/// <param name="AnalyzerName">The display name of the analyzer (e.g., "Version Inconsistencies").</param>
/// <param name="Issues">List of issues found by this analyzer.</param>
public record AnalyzerResult(
    string AnalyzerName,
    IReadOnlyList<AnalysisIssue> Issues
)
{
    /// <summary>
    /// Returns true if this analyzer found any issues.
    /// </summary>
    public bool HasIssues => Issues.Count > 0;
}
