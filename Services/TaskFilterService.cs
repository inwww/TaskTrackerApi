namespace TaskTrackerApi.Services;

using TaskTrackerApi.Models;

/// <summary>
/// Static service for filtering and analyzing tasks
/// </summary>
public static class TaskFilterService
{
    /// <summary>
    /// Get high severity incomplete bug reports sorted by creation date (newest first)
    /// </summary>
    /// <param name="tasks">Collection of all tasks</param>
    /// <returns>Filtered and sorted list of bug reports</returns>
    public static List<BugReportTask> GetHighSeverityIncompleteBugs(IEnumerable<BaseTask> tasks)
    {
        return tasks
            .OfType<BugReportTask>()
            .Where(bug => !bug.IsCompleted && bug.SeverityLevel >= SeverityLevel.High)
            .OrderByDescending(bug => bug.CreatedAt)
            .ToList();
    }

    /// <summary>
    /// Calculate total estimated hours for incomplete feature requests
    /// </summary>
    /// <param name="tasks">Collection of all tasks</param>
    /// <returns>Sum of estimated hours</returns>
    public static decimal GetTotalEstimatedHours(IEnumerable<BaseTask> tasks)
    {
        return tasks
            .OfType<FeatureRequestTask>()
            .Where(feature => !feature.IsCompleted)
            .Sum(feature => feature.EstimatedHours);
    }

    /// <summary>
    /// Get comprehensive task analysis combining both high severity bugs and feature hours
    /// </summary>
    /// <param name="tasks">Collection of all tasks</param>
    /// <returns>Tuple containing high severity bugs and total estimated hours</returns>
    public static (List<BugReportTask> HighSeverityBugs, decimal TotalEstimatedHours) GetTaskAnalysis(
        IEnumerable<BaseTask> tasks)
    {
        var taskList = tasks.ToList();
        return (GetHighSeverityIncompleteBugs(taskList), GetTotalEstimatedHours(taskList));
    }
}
