namespace TaskTrackerApi.Models;

/// <summary>
/// Delegate for task completion event
/// </summary>
public delegate void TaskCompletedEventHandler(object sender, TaskCompletedEventArgs e);

/// <summary>
/// Event arguments for task completion
/// </summary>
public class TaskCompletedEventArgs : EventArgs
{
    public Guid TaskId { get; set; }
    public string Title { get; set; } = string.Empty;
    public DateTime CompletedAt { get; set; }
}

/// <summary>
/// Abstract base class for all task types
/// </summary>
public abstract record BaseTask
{
    /// <summary>
    /// Event triggered when a task is completed
    /// </summary>
    public event TaskCompletedEventHandler? OnTaskCompleted;

    /// <summary>
    /// Unique identifier - set only during object creation
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// Task title
    /// </summary>
    public required string Title { get; set; }

    /// <summary>
    /// Task creation timestamp - set only during object creation
    /// </summary>
    public DateTime CreatedAt { get; init; }

    /// <summary>
    /// Task completion status
    /// </summary>
    public bool IsCompleted { get; protected set; }

    /// <summary>
    /// Initialize a new task
    /// </summary>
    protected BaseTask()
    {
        Id = Guid.NewGuid();
        CreatedAt = DateTime.UtcNow;
        IsCompleted = false;
    }

    /// <summary>
    /// Mark task as completed and trigger the event
    /// </summary>
    public virtual void CompleteTask()
    {
        if (!IsCompleted)
        {
            IsCompleted = true;
            OnTaskCompleted?.Invoke(this, new TaskCompletedEventArgs
            {
                TaskId = Id,
                Title = Title,
                CompletedAt = DateTime.UtcNow
            });
        }
    }
}
