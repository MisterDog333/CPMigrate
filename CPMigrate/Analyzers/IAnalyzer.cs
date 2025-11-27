using CPMigrate.Models;

namespace CPMigrate.Analyzers;

/// <summary>
/// Interface for package analyzers that detect specific types of issues.
/// </summary>
public interface IAnalyzer
{
    /// <summary>
    /// The display name for this analyzer (e.g., "Version Inconsistencies").
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Analyzes package references for issues.
    /// </summary>
    /// <param name="packageInfo">All package references discovered from projects.</param>
    /// <returns>Analysis result containing any issues found.</returns>
    AnalyzerResult Analyze(ProjectPackageInfo packageInfo);
}
