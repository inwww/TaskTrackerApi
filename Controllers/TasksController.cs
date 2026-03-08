namespace TaskTrackerApi.Controllers;

using Microsoft.AspNetCore.Mvc;
using TaskTrackerApi.Models;
using TaskTrackerApi.Services;

/// <summary>
/// API controller for task management operations
/// </summary>
[ApiController]
[Route("api/tasks")]
public class TasksController : ControllerBase
{
    private readonly ITaskRepository _taskRepository;
    private readonly ILogger<TasksController> _logger;

    public TasksController(ITaskRepository taskRepository, ILogger<TasksController> logger)
    {
        _taskRepository = taskRepository;
        _logger = logger;
    }

    /// <summary>
    /// Retrieve all tasks
    /// </summary>
    /// <returns>Collection of all tasks</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<TaskDto>>> GetAllTasks()
    {
        _logger.LogInformation("Retrieving all tasks");
        var tasks = await _taskRepository.GetAllTasksAsync();
        var taskDtos = tasks.Select(t => MapTaskToDto(t)).ToList();
        return Ok(taskDtos);
    }

    /// <summary>
    /// Get a specific task by ID
    /// </summary>
    /// <param name="id">Task ID</param>
    /// <returns>Task details</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TaskDto>> GetTaskById(Guid id)
    {
        _logger.LogInformation("Retrieving task {TaskId}", id);
        var task = await _taskRepository.GetTaskByIdAsync(id);

        if (task is null)
        {
            _logger.LogWarning("Task {TaskId} not found", id);
            return NotFound(new { message = $"Task with ID {id} not found" });
        }

        return Ok(MapTaskToDto(task));
    }

    /// <summary>
    /// Create a new bug report task
    /// </summary>
    /// <param name="request">Bug report creation request</param>
    /// <returns>Created bug report task</returns>
    [HttpPost("bug")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<TaskDto>> CreateBugReport([FromBody] CreateBugReportRequest request)
    {
        _logger.LogInformation("Creating new bug report: {Title}", request.Title);

        if (string.IsNullOrWhiteSpace(request.Title))
        {
            return BadRequest(new { message = "Title is required" });
        }

        var bugTask = new BugReportTask(request.Title, request.SeverityLevel);

        // Subscribe to completion event for logging
        bugTask.OnTaskCompleted += (sender, args) =>
        {
            _logger.LogInformation(
                "Bug task completed - TaskId: {TaskId}, Title: {Title}, CompletedAt: {CompletedAt}",
                args.TaskId, args.Title, args.CompletedAt);
        };

        var createdTask = await _taskRepository.CreateTaskAsync(bugTask);
        var taskDto = MapTaskToDto(createdTask);

        return CreatedAtAction(nameof(GetTaskById), new { id = createdTask.Id }, taskDto);
    }

    /// <summary>
    /// Create a new feature request task
    /// </summary>
    /// <param name="request">Feature request creation request</param>
    /// <returns>Created feature request task</returns>
    [HttpPost("feature")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<TaskDto>> CreateFeatureRequest([FromBody] CreateFeatureRequestRequest request)
    {
        _logger.LogInformation("Creating new feature request: {Title}", request.Title);

        if (string.IsNullOrWhiteSpace(request.Title))
        {
            return BadRequest(new { message = "Title is required" });
        }

        if (request.EstimatedHours <= 0)
        {
            return BadRequest(new { message = "Estimated hours must be greater than 0" });
        }

        var featureTask = new FeatureRequestTask(request.Title, request.EstimatedHours);

        // Subscribe to completion event for logging
        featureTask.OnTaskCompleted += (sender, args) =>
        {
            _logger.LogInformation(
                "Feature task completed - TaskId: {TaskId}, Title: {Title}, CompletedAt: {CompletedAt}",
                args.TaskId, args.Title, args.CompletedAt);
        };

        var createdTask = await _taskRepository.CreateTaskAsync(featureTask);
        var taskDto = MapTaskToDto(createdTask);

        return CreatedAtAction(nameof(GetTaskById), new { id = createdTask.Id }, taskDto);
    }

    /// <summary>
    /// Mark a task as completed
    /// </summary>
    /// <param name="id">Task ID to complete</param>
    /// <returns>Updated task details</returns>
    [HttpPut("{id}/complete")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TaskDto>> CompleteTask(Guid id)
    {
        _logger.LogInformation("Completing task {TaskId}", id);
        var task = await _taskRepository.CompleteTaskAsync(id);

        if (task is null)
        {
            _logger.LogWarning("Task {TaskId} not found for completion", id);
            return NotFound(new { message = $"Task with ID {id} not found" });
        }

        var taskDto = MapTaskToDto(task);
        return Ok(taskDto);
    }

    /// <summary>
    /// Get task analysis: high severity incomplete bugs and total estimated hours
    /// </summary>
    /// <returns>Task analysis results</returns>
    [HttpGet("analysis/summary")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<TaskAnalysisDto>> GetTaskAnalysis()
    {
        _logger.LogInformation("Retrieving task analysis");
        var tasks = await _taskRepository.GetAllTasksAsync();
        var (highSeverityBugs, totalEstimatedHours) = TaskFilterService.GetTaskAnalysis(tasks);

        var analysis = new TaskAnalysisDto
        {
            HighSeverityBugs = highSeverityBugs.Select(b => MapTaskToDto(b)).ToList(),
            TotalEstimatedHours = totalEstimatedHours,
            HighSeverityBugCount = highSeverityBugs.Count
        };

        return Ok(analysis);
    }

    /// <summary>
    /// Delete a task by ID
    /// </summary>
    /// <param name="id">Task ID to delete</param>
    /// <returns>No content</returns>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteTask(Guid id)
    {
        _logger.LogInformation("Deleting task {TaskId}", id);
        var deleted = await _taskRepository.DeleteTaskAsync(id);

        if (!deleted)
        {
            _logger.LogWarning("Task {TaskId} not found for deletion", id);
            return NotFound(new { message = $"Task with ID {id} not found" });
        }

        return NoContent();
    }

    /// <summary>
    /// Helper method to map BaseTask to DTO
    /// </summary>
    private static TaskDto MapTaskToDto(BaseTask task)
    {
        return task switch
        {
            BugReportTask bug => new BugReportTaskDto
            {
                Id = bug.Id,
                Title = bug.Title,
                CreatedAt = bug.CreatedAt,
                IsCompleted = bug.IsCompleted,
                SeverityLevel = bug.SeverityLevel,
                TaskType = "BugReport"
            },
            FeatureRequestTask feature => new FeatureRequestTaskDto
            {
                Id = feature.Id,
                Title = feature.Title,
                CreatedAt = feature.CreatedAt,
                IsCompleted = feature.IsCompleted,
                EstimatedHours = feature.EstimatedHours,
                TaskType = "FeatureRequest"
            },
            _ => new TaskDto
            {
                Id = task.Id,
                Title = task.Title,
                CreatedAt = task.CreatedAt,
                IsCompleted = task.IsCompleted,
                TaskType = "Unknown"
            }
        };
    }
}

/// <summary>
/// Base DTO for task responses
/// </summary>
public record TaskDto
{
    public Guid Id { get; set; }
    public required string Title { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsCompleted { get; set; }
    public string TaskType { get; set; } = "Unknown";
}

/// <summary>
/// DTO for bug report tasks
/// </summary>
public record BugReportTaskDto : TaskDto
{
    public SeverityLevel SeverityLevel { get; set; }
}

/// <summary>
/// DTO for feature request tasks
/// </summary>
public record FeatureRequestTaskDto : TaskDto
{
    public decimal EstimatedHours { get; set; }
}

/// <summary>
/// Task analysis results
/// </summary>
public record TaskAnalysisDto
{
    public List<TaskDto> HighSeverityBugs { get; set; } = new();
    public decimal TotalEstimatedHours { get; set; }
    public int HighSeverityBugCount { get; set; }
}

/// <summary>
/// Request model for creating a bug report
/// </summary>
public record CreateBugReportRequest
{
    public required string Title { get; set; }
    public SeverityLevel SeverityLevel { get; set; }
}

/// <summary>
/// Request model for creating a feature request
/// </summary>
public record CreateFeatureRequestRequest
{
    public required string Title { get; set; }
    public decimal EstimatedHours { get; set; }
}
