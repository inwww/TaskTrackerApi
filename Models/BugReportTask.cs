namespace TaskTrackerApi.Models;

public record BugReportTask(string Title, SeverityLevel SeverityLevel) : BaseTask(Title)
{
}
