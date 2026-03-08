namespace TaskTrackerApi.Models;

public record FeatureRequestTask(string Title, decimal EstimatedHours) : BaseTask(Title)
{
}
