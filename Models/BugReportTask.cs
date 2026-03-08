namespace TaskTrackerApi.Models;

public record BugReportTask : BaseTask
{
    public SeverityLevel SeverityLevel { get; set; }

    public BugReportTask()
    {
    }

    public BugReportTask(string title, SeverityLevel severityLevel) : this()
    {
        Title = title;
        SeverityLevel = severityLevel;
    }
}
