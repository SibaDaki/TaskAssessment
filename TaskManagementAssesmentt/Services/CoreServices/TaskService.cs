using TaskManagementAssesmentt.DTOs;
using TaskManagementAssesmentt.Entities;
using TaskManagementAssesmentt.Helper;
using TaskManagementAssesmentt.Repositories.IRepository;
using TaskManagementAssesmentt.Services.IService;

namespace TaskManagementAssesmentt.Services.CoreServices
{
    public class TaskService : ITaskService
    {
        private readonly ITaskRepository _taskRepository;
        private readonly ITeamMemberRepository _teamMemberRepository;

        public TaskService(ITaskRepository taskRepository, ITeamMemberRepository teamMemberRepository)
        {
            _taskRepository = taskRepository;
            _teamMemberRepository = teamMemberRepository;
        }

        public async Task<TaskResponseDto> GetTaskByIdAsync(int id)
        {
            var task = await _taskRepository.GetByIdAsync(id)
                ?? throw new ResourceNotFoundException(nameof(Task), id);
            return MapToDto(task);
        }

        public async Task<List<TaskResponseDto>> GetAllTasksAsync()
        {
            var tasks = await _taskRepository.GetAllAsync();
            return tasks.Select(MapToDto).ToList();
        }

        public async Task<List<TaskResponseDto>> GetTasksByStatusAsync(int status)
        {
            if (!IsValidStatus(status))
                throw new ValidationException("Invalid task status.");

            var taskStatus = (TaskStatusEnum)status;
            var tasks = await _taskRepository.GetTasksByStatusAsync(taskStatus);
            return tasks.Select(MapToDto).ToList();
        }

        public async Task<List<TaskResponseDto>> GetTasksByAssigneeAsync(int assigneeId)
        {
            var assignee = await _teamMemberRepository.GetByIdAsync(assigneeId)
                ?? throw new ResourceNotFoundException(nameof(TeamMemberEntity), assigneeId);

            var tasks = await _taskRepository.GetTasksByAssigneeAsync(assigneeId);
            return tasks.Select(MapToDto).ToList();
        }

        public async Task<List<TaskResponseDto>> GetTasksByPriorityAsync(int priority)
        {
            if (!IsValidPriority(priority))
                throw new ValidationException("Invalid task priority.");

            var taskPriority = (TaskPriorityEnum)priority;
            var tasks = await _taskRepository.GetTasksByPriorityAsync(taskPriority);
            return tasks.Select(MapToDto).ToList();
        }

        public async Task<List<TaskResponseDto>> GetOverdueTasksAsync()
        {
            var tasks = await _taskRepository.GetOverdueTasksAsync();
            return tasks.Select(MapToDto).ToList();
        }

        public async Task<List<TaskResponseDto>> SearchTasksAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                throw new ValidationException("Search term cannot be empty.");

            var tasks = await _taskRepository.SearchTasksAsync(searchTerm);
            return tasks.Select(MapToDto).ToList();
        }

        public async Task<List<TaskResponseDto>> FilterTasksAsync(int? status, int? priority, int? assigneeId)
        {
            if (status.HasValue && !IsValidStatus(status.Value))
                throw new ValidationException("Invalid task status.");

            if (priority.HasValue && !IsValidPriority(priority.Value))
                throw new ValidationException("Invalid task priority.");

            if (assigneeId.HasValue)
            {
                var assignee = await _teamMemberRepository.GetByIdAsync(assigneeId.Value);
                if (assignee == null)
                    throw new ResourceNotFoundException(nameof(TeamMemberEntity), assigneeId.Value);
            }

            var taskStatus = status.HasValue ? (TaskStatusEnum?)status.Value : null;
            var taskPriority = priority.HasValue ? (TaskPriorityEnum?)priority.Value : null;

            var tasks = await _taskRepository.FilterTasksAsync(taskStatus, taskPriority, assigneeId);
            return tasks.Select(MapToDto).ToList();
        }

        public async Task<PaginatedResponse<TaskResponseDto>> GetPaginatedTasksAsync(int pageNumber, int pageSize, int? status, int? priority, int? assigneeId)
        {
            if (pageNumber < 1 || pageSize < 1 || pageSize > 100)
                throw new ValidationException("Invalid pagination parameters.");

            if (status.HasValue && !IsValidStatus(status.Value))
                throw new ValidationException("Invalid task status.");

            if (priority.HasValue && !IsValidPriority(priority.Value))
                throw new ValidationException("Invalid task priority.");

            var taskStatus = status.HasValue ? (TaskStatusEnum?)status.Value : null;
            var taskPriority = priority.HasValue ? (TaskPriorityEnum?)priority.Value : null;

            var result = await _taskRepository.GetPaginatedTasksAsync(pageNumber, pageSize, taskStatus, taskPriority, assigneeId);

            return new PaginatedResponse<TaskResponseDto>
            {
                Items = result.Items.Select(MapToDto).ToList(),
                TotalCount = result.TotalCount,
                PageNumber = result.PageNumber,
                PageSize = result.PageSize
            };
        }

        public async Task<TaskResponseDto> CreateTaskAsync(CreateTaskDto dto)
        {
            ValidateCreateTaskDto(dto);

            if (dto.AssignedToId.HasValue)
            {
                var assignee = await _teamMemberRepository.GetByIdAsync(dto.AssignedToId.Value);
                if (assignee == null)
                    throw new ResourceNotFoundException(nameof(TeamMemberEntity), dto.AssignedToId.Value);
            }

            var task = new TaskEntity
            {
                Title = dto.Title,
                Description = dto.Description,
                Priority = (TaskPriorityEnum)dto.Priority,
                AssignedToId = dto.AssignedToId,
                DueDate = dto.DueDate ?? DateTime.UtcNow.AddDays(7),
                CreatedAt = DateTime.UtcNow
            };

            await _taskRepository.AddAsync(task);
            return MapToDto(task);
        }

        public async Task<TaskResponseDto> UpdateTaskAsync(int id, UpdateTaskDto dto)
        {
            var task = await _taskRepository.GetByIdAsync(id)
                ?? throw new ResourceNotFoundException(nameof(Task), id);

            if (!string.IsNullOrWhiteSpace(dto.Title))
                task.Title = dto.Title;

            if (dto.Description != null)
                task.Description = dto.Description;

            if (dto.Status.HasValue)
            {
                if (!IsValidStatus(dto.Status.Value))
                    throw new ValidationException("Invalid task status.");
                task.Status = (TaskStatusEnum)dto.Status.Value;
                if (task.Status == TaskStatusEnum.Completed && !task.CompletedAt.HasValue)
                    task.CompletedAt = DateTime.UtcNow;
            }

            if (dto.Priority.HasValue)
            {
                if (!IsValidPriority(dto.Priority.Value))
                    throw new ValidationException("Invalid task priority.");
                task.Priority = (TaskPriorityEnum)dto.Priority.Value;
            }

            if (dto.AssignedToId.HasValue)
            {
                var assignee = await _teamMemberRepository.GetByIdAsync(dto.AssignedToId.Value);
                if (assignee == null)
                    throw new ResourceNotFoundException(nameof(TeamMemberEntity), dto.AssignedToId.Value);
                task.AssignedToId = dto.AssignedToId.Value;
            }

            if (dto.DueDate.HasValue)
                task.DueDate = dto.DueDate.Value;

            task.UpdatedAt = DateTime.UtcNow;
            await _taskRepository.UpdateAsync(task);
            return MapToDto(task);
        }

        public async Task<bool> DeleteTaskAsync(int id)
        {
            var exists = await _taskRepository.GetByIdAsync(id) != null;
            if (!exists)
                throw new ResourceNotFoundException(nameof(TaskEntity), id);

            return await _taskRepository.DeleteAsync(id);
        }

        public async Task<TaskResponseDto> AssignTaskAsync(int taskId, int teamMemberId)
        {
            var task = await _taskRepository.GetByIdAsync(taskId)
                ?? throw new ResourceNotFoundException(nameof(TaskEntity), taskId);

            var teamMember = await _teamMemberRepository.GetByIdAsync(teamMemberId)
                ?? throw new ResourceNotFoundException(nameof(TeamMemberEntity), teamMemberId);

            if (!teamMember.IsActive)
                throw new System.InvalidOperationException("Cannot assign task to an inactive team member.");

            task.AssignedToId = teamMemberId;
            task.UpdatedAt = DateTime.UtcNow;
            await _taskRepository.UpdateAsync(task);
            return MapToDto(task);
        }

        public async Task<TaskResponseDto> UpdateTaskStatusAsync(int taskId, int status)
        {
            if (!IsValidStatus(status))
                throw new ValidationException("Invalid task status.");

            var task = await _taskRepository.GetByIdAsync(taskId)
                ?? throw new ResourceNotFoundException(nameof(TaskEntity), taskId);

            task.Status = (TaskStatusEnum)status;
            if (task.Status == TaskStatusEnum.Completed && !task.CompletedAt.HasValue)
                task.CompletedAt = DateTime.UtcNow;

            task.UpdatedAt = DateTime.UtcNow;
            await _taskRepository.UpdateAsync(task);
            return MapToDto(task);
        }

        public async Task<TaskResponseDto> UpdateTaskPriorityAsync(int taskId, int priority)
        {
            if (!IsValidPriority(priority))
                throw new ValidationException("Invalid task priority.");

            var task = await _taskRepository.GetByIdAsync(taskId)
                ?? throw new ResourceNotFoundException(nameof(TaskEntity), taskId);

            task.Priority = (TaskPriorityEnum)priority;
            task.UpdatedAt = DateTime.UtcNow;
            await _taskRepository.UpdateAsync(task);
            return MapToDto(task);
        }

        private static TaskResponseDto MapToDto(TaskEntity task)
        {
            return new TaskResponseDto
            {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description,
                Status = task.Status.ToString(),
                Priority = task.Priority.ToString(),
                AssignedTo = task.AssignedTo != null ? new TeamMemberDto
                {
                    Id = task.AssignedTo.Id,
                    Name = task.AssignedTo.Name,
                    Email = task.AssignedTo.Email,
                    Role = task.AssignedTo.Role,
                    IsActive = task.AssignedTo.IsActive
                } : null,
                DueDate = task.DueDate,
                CreatedAt = task.CreatedAt,
                UpdatedAt = task.UpdatedAt,
                CompletedAt = task.CompletedAt
            };
        }

        private static void ValidateCreateTaskDto(CreateTaskDto dto)
        {
            var errors = new Dictionary<string, string[]>();

            if (string.IsNullOrWhiteSpace(dto.Title))
                errors["Title"] = new[] { "Title is required." };
            else if (dto.Title.Length > 200)
                errors["Title"] = new[] { "Title cannot exceed 200 characters." };

            if (!IsValidPriority(dto.Priority))
                errors["Priority"] = new[] { "Invalid priority value." };

            if (errors.Any())
                throw new ValidationException("Task validation failed.", errors);
        }

        private static bool IsValidStatus(int status) =>
            Enum.IsDefined(typeof(TaskStatus), status);

        private static bool IsValidPriority(int priority) =>
            Enum.IsDefined(typeof(TaskPriorityEnum), priority);
    }
}
