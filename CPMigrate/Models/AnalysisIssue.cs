namespace CPMigrate.Models;

/// <summary>
/// Represents a single issue found by an analyzer.
/// </summary>
/// <param name="PackageName">The name of the package with the issue.</param>
/// <param name="Description">A description of the issue found.</param>
/// <param name="AffectedProjects">List of project names/paths affected by this issue.</param>
public record AnalysisIssue(
    string PackageName,
    string Description,
    IReadOnlyList<string> AffectedProjects
);
