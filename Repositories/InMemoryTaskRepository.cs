namespace TaskTrackerApi.Services;

using TaskTrackerApi.Models;

public class InMemoryTaskRepository : ITaskRepository
{
    private readonly Dictionary<Guid, BaseTask> _tasks = new();
    private readonly object _lockObject = new();

    public InMemoryTaskRepository()
    {
        // Initialize with sample data
        InitializeSampleData();
    }

    private void InitializeSampleData()
    {
        lock (_lockObject)
        {
            var bug1 = new BugReportTask("Critical authentication bug", SeverityLevel.Critical);
            var bug2 = new BugReportTask("UI rendering issue", SeverityLevel.High);
            var bug3 = new BugReportTask("Minor typo in docs", SeverityLevel.Low);

            var feature1 = new FeatureRequestTask("Implement two-factor authentication", 16);
            var feature2 = new FeatureRequestTask("Add dark mode support", 8);
            var feature3 = new FeatureRequestTask("Dashboard improvements", 12);

            _tasks[bug1.Id] = bug1;
            _tasks[bug2.Id] = bug2;
            _tasks[bug3.Id] = bug3;
            _tasks[feature1.Id] = feature1;
            _tasks[feature2.Id] = feature2;
            _tasks[feature3.Id] = feature3;

            // Complete one task for demonstration
            feature2.CompleteTask();
        }
    }

    public Task<IEnumerable<BaseTask>> GetAllTasksAsync()
    {
        lock (_lockObject)
        {
            return Task.FromResult(_tasks.Values.AsEnumerable());
        }
    }

    public Task<BaseTask?> GetTaskByIdAsync(Guid id)
    {
        lock (_lockObject)
        {
            _ = _tasks.TryGetValue(id, out var task);
            return Task.FromResult(task);
        }
    }

    public Task<BaseTask> CreateTaskAsync(BaseTask task)
    {
        lock (_lockObject)
        {
            _tasks[task.Id] = task;
            return Task.FromResult(task);
        }
    }

    public Task<BaseTask?> UpdateTaskAsync(BaseTask task)
    {
        lock (_lockObject)
        {
            if (_tasks.ContainsKey(task.Id))
            {
                _tasks[task.Id] = task;
                return Task.FromResult<BaseTask?>(task);
            }

            return Task.FromResult<BaseTask?>(null);
        }
    }

    public Task<BaseTask?> CompleteTaskAsync(Guid id)
    {
        lock (_lockObject)
        {
            if (_tasks.TryGetValue(id, out var task))
            {
                task.CompleteTask();
                return Task.FromResult<BaseTask?>(task);
            }

            return Task.FromResult<BaseTask?>(null);
        }
    }

    public Task<bool> DeleteTaskAsync(Guid id)
    {
        lock (_lockObject)
        {
            return Task.FromResult(_tasks.Remove(id));
        }
    }
}
