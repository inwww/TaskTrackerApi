namespace TaskTrackerApi.Models;

/// <summary>
/// Feature request task with estimated hours
/// </summary>
public record FeatureRequestTask : BaseTask
{
    /// <summary>
    /// Estimated hours to complete the feature
    /// </summary>
    public decimal EstimatedHours { get; set; }

    /// <summary>
    /// Initialize a new feature request task
    /// </summary>
    public FeatureRequestTask()
    {
    }

    /// <summary>
    /// Initialize a new feature request task with parameters
    /// </summary>
    /// <param name="title">Feature title</param>
    /// <param name="estimatedHours">Estimated hours</param>
    public FeatureRequestTask(string title, decimal estimatedHours) : this()
    {
        Title = title;
        EstimatedHours = estimatedHours;
    }
}
