using TaskManagementAssesmentt.DTOs;
using TaskManagementAssesmentt.Entities;

namespace TaskManagementAssesmentt.Repositories.IRepository
{
    public interface ITaskRepository : IRepository<TaskEntity>
    {
        Task<List<TaskEntity>> GetTasksByStatusAsync(TaskStatusEnum status);
        Task<List<TaskEntity>> GetTasksByAssigneeAsync(int assigneeId);
        Task<List<TaskEntity>> GetTasksByPriorityAsync(TaskPriorityEnum priority);
        Task<List<TaskEntity>> GetOverdueTasksAsync();
        Task<List<TaskEntity>> SearchTasksAsync(string searchTerm);
        Task<List<TaskEntity>> FilterTasksAsync(TaskStatusEnum? status, TaskPriorityEnum? priority, int? assigneeId);

        Task<PaginatedResponse<TaskEntity>> GetPaginatedTasksAsync(
            int pageNumber,
            int pageSize,
            TaskStatusEnum? status,
            TaskPriorityEnum? priority,
            int? assigneeId
        );
    }
}
