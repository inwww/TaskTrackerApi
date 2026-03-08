namespace TaskTrackerApi.Models;

public record FeatureRequestTask : BaseTask
{
    public decimal EstimatedHours { get; set; }

    public FeatureRequestTask()
    {
    }

    public FeatureRequestTask(string title, decimal estimatedHours) : this()
    {
        Title = title;
        EstimatedHours = estimatedHours;
    }
}
