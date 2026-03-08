namespace TaskTrackerApi.Models;

/// <summary>
/// Bug report task with severity level
/// </summary>
public record BugReportTask : BaseTask
{
    /// <summary>
    /// Severity level of the bug
    /// </summary>
    public SeverityLevel SeverityLevel { get; set; }

    /// <summary>
    /// Initialize a new bug report task
    /// </summary>
    public BugReportTask()
    {
    }

    /// <summary>
    /// Initialize a new bug report task with parameters
    /// </summary>
    /// <param name="title">Bug title</param>
    /// <param name="severityLevel">Severity level</param>
    public BugReportTask(string title, SeverityLevel severityLevel) : this()
    {
        Title = title;
        SeverityLevel = severityLevel;
    }
}
