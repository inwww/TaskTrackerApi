namespace TaskTrackerApi.Services;

using TaskTrackerApi.Models;

/// <summary>
/// Interface for task repository operations
/// </summary>
public interface ITaskRepository
{
    /// <summary>
    /// Get all tasks
    /// </summary>
    Task<IEnumerable<BaseTask>> GetAllTasksAsync();

    /// <summary>
    /// Get a specific task by ID
    /// </summary>
    Task<BaseTask?> GetTaskByIdAsync(Guid id);

    /// <summary>
    /// Create a new task
    /// </summary>
    Task<BaseTask> CreateTaskAsync(BaseTask task);

    /// <summary>
    /// Update an existing task
    /// </summary>
    Task<BaseTask?> UpdateTaskAsync(BaseTask task);

    /// <summary>
    /// Complete a task by ID
    /// </summary>
    Task<BaseTask?> CompleteTaskAsync(Guid id);

    /// <summary>
    /// Delete a task by ID
    /// </summary>
    Task<bool> DeleteTaskAsync(Guid id);
}
