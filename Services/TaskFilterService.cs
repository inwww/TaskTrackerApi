namespace TaskTrackerApi.Services;

using TaskTrackerApi.Models;

public static class TaskFilterService
{
    public static List<BugReportTask> GetHighSeverityIncompleteBugs(IEnumerable<BaseTask> tasks)
    {
        return tasks
            .OfType<BugReportTask>()
            .Where(bug => !bug.IsCompleted && bug.SeverityLevel >= SeverityLevel.High)
            .OrderByDescending(bug => bug.CreatedAt)
            .ToList();
    }

    public static decimal GetTotalEstimatedHours(IEnumerable<BaseTask> tasks)
    {
        return tasks
            .OfType<FeatureRequestTask>()
            .Where(feature => !feature.IsCompleted)
            .Sum(feature => feature.EstimatedHours);
    }

    public static (List<BugReportTask> HighSeverityBugs, decimal TotalEstimatedHours) GetTaskAnalysis(
        IEnumerable<BaseTask> tasks)
    {
        var taskList = tasks.ToList();
        return (GetHighSeverityIncompleteBugs(taskList), GetTotalEstimatedHours(taskList));
    }
}
