namespace TaskTrackerApi.Services;

using TaskTrackerApi.Models;

public interface ITaskRepository
{
    Task<IEnumerable<BaseTask>> GetAllTasksAsync();

    Task<BaseTask?> GetTaskByIdAsync(Guid id);

    Task<BaseTask> CreateTaskAsync(BaseTask task);

    Task<BaseTask?> UpdateTaskAsync(BaseTask task);

    Task<BaseTask?> CompleteTaskAsync(Guid id);

    Task<bool> DeleteTaskAsync(Guid id);
}
