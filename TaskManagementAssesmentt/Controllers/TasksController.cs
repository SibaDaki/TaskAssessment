using Microsoft.AspNetCore.Mvc;
using TaskManagementAssesmentt.DTOs;
using TaskManagementAssesmentt.Services.IService;

namespace TaskManagementAPI.Controllers;

/// <summary>
/// Controller for managing tasks
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class TasksController : ControllerBase
{
    private readonly ITaskService _taskService;
    private readonly ILogger<TasksController> _logger;

    public TasksController(ITaskService taskService, ILogger<TasksController> logger)
    {
        _taskService = taskService;
        _logger = logger;
    }

    /// <summary>
    /// Get all tasks with optional filtering
    /// </summary>
    /// <param name="status">Filter by task status (0=Todo, 1=InProgress, 2=Review, 3=Completed, 4=Blocked)</param>
    /// <param name="priority">Filter by priority (0=Low, 1=Medium, 2=High, 3=Critical)</param>
    /// <param name="assigneeId">Filter by assigned team member</param>
    /// <param name="pageNumber">Page number for pagination (default: 1)</param>
    /// <param name="pageSize">Items per page (default: 10, max: 100)</param>
    [HttpGet]
    [ProducesResponseType(typeof(List<TaskResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetTasks(
        [FromQuery] int? status = null,
        [FromQuery] int? priority = null,
        [FromQuery] int? assigneeId = null,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        _logger.LogInformation("Getting tasks with filters - Status: {Status}, Priority: {Priority}, AssigneeId: {AssigneeId}", status, priority, assigneeId);

        if (pageNumber > 0 && pageSize > 0)
        {
            var paginatedResult = await _taskService.GetPaginatedTasksAsync(pageNumber, pageSize, status, priority, assigneeId);
            return Ok(paginatedResult);
        }

        var tasks = await _taskService.FilterTasksAsync(status, priority, assigneeId);
        return Ok(tasks);
    }

    /// <summary>
    /// Search tasks by title or description
    /// </summary>
    [HttpGet("search")]
    [ProducesResponseType(typeof(List<TaskResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> SearchTasks([FromQuery] string searchTerm)
    {
        _logger.LogInformation("Searching tasks with term: {SearchTerm}", searchTerm);
        var tasks = await _taskService.SearchTasksAsync(searchTerm);
        return Ok(tasks);
    }

    /// <summary>
    /// Get a specific task by ID
    /// </summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(TaskResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetTaskById(int id)
    {
        _logger.LogInformation("Getting task with ID: {TaskId}", id);
        var task = await _taskService.GetTaskByIdAsync(id);
        return Ok(task);
    }

    /// <summary>
    /// Create a new task
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(TaskResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateTask([FromBody] CreateTaskDto dto)
    {
        _logger.LogInformation("Creating new task: {Title}", dto.Title);
        var task = await _taskService.CreateTaskAsync(dto);
        return CreatedAtAction(nameof(GetTaskById), new { id = task.Id }, task);
    }

    /// <summary>
    /// Update an existing task
    /// </summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(TaskResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateTask(int id, [FromBody] UpdateTaskDto dto)
    {
        _logger.LogInformation("Updating task with ID: {TaskId}", id);
        var task = await _taskService.UpdateTaskAsync(id, dto);
        return Ok(task);
    }

    /// <summary>
    /// Delete a task
    /// </summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteTask(int id)
    {
        _logger.LogInformation("Deleting task with ID: {TaskId}", id);
        await _taskService.DeleteTaskAsync(id);
        return NoContent();
    }

    /// <summary>
    /// Assign a task to a team member
    /// </summary>
    [HttpPatch("{taskId:int}/assign/{teamMemberId:int}")]
    [ProducesResponseType(typeof(TaskResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AssignTask(int taskId, int teamMemberId)
    {
        _logger.LogInformation("Assigning task {TaskId} to team member {TeamMemberId}", taskId, teamMemberId);
        var task = await _taskService.AssignTaskAsync(taskId, teamMemberId);
        return Ok(task);
    }

    /// <summary>
    /// Update task status
    /// </summary>
    [HttpPatch("{id:int}/status")]
    [ProducesResponseType(typeof(TaskResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateTaskStatus(int id, [FromBody] StatusUpdateDto dto)
    {
        _logger.LogInformation("Updating task {TaskId} status to {Status}", id, dto.Status);
        var task = await _taskService.UpdateTaskStatusAsync(id, dto.Status);
        return Ok(task);
    }

    /// <summary>
    /// Update task priority
    /// </summary>
    [HttpPatch("{id:int}/priority")]
    [ProducesResponseType(typeof(TaskResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateTaskPriority(int id, [FromBody] PriorityUpdateDto dto)
    {
        _logger.LogInformation("Updating task {TaskId} priority to {Priority}", id, dto.Priority);
        var task = await _taskService.UpdateTaskPriorityAsync(id, dto.Priority);
        return Ok(task);
    }
}

