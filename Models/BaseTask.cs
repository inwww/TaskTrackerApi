namespace TaskTrackerApi.Models;

public delegate void TaskCompletedEventHandler(object sender, TaskCompletedEventArgs e);

public class TaskCompletedEventArgs : EventArgs
{
    public Guid TaskId { get; set; }
    public string Title { get; set; } = string.Empty;
    public DateTime CompletedAt { get; set; }
}

public abstract record BaseTask(required string Title)
{
    public event TaskCompletedEventHandler? OnTaskCompleted;

    public Guid Id { get; init; } = Guid.NewGuid();

    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;

    public bool IsCompleted { get; protected set; } = false;

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
