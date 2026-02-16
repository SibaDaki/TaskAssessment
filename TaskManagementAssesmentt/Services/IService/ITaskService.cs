using TaskManagementAssesmentt.DTOs;

namespace TaskManagementAssesmentt.Services.IService
{
    public interface ITaskService
    {
        Task<TaskResponseDto> GetTaskByIdAsync(int id);
        Task<List<TaskResponseDto>> GetAllTasksAsync();
        Task<List<TaskResponseDto>> GetTasksByStatusAsync(int status);
        Task<List<TaskResponseDto>> GetTasksByAssigneeAsync(int assigneeId);
        Task<List<TaskResponseDto>> GetTasksByPriorityAsync(int priority);
        Task<List<TaskResponseDto>> GetOverdueTasksAsync();
        Task<List<TaskResponseDto>> SearchTasksAsync(string searchTerm);
        Task<List<TaskResponseDto>> FilterTasksAsync(int? status, int? priority, int? assigneeId);
        Task<PaginatedResponse<TaskResponseDto>> GetPaginatedTasksAsync(int pageNumber, int pageSize, int? status, int? priority, int? assigneeId);
        Task<TaskResponseDto> CreateTaskAsync(CreateTaskDto dto);
        Task<TaskResponseDto> UpdateTaskAsync(int id, UpdateTaskDto dto);
        Task<bool> DeleteTaskAsync(int id);
        Task<TaskResponseDto> AssignTaskAsync(int taskId, int teamMemberId);
        Task<TaskResponseDto> UpdateTaskStatusAsync(int taskId, int status);
        Task<TaskResponseDto> UpdateTaskPriorityAsync(int taskId, int priority);
    }
}
