using Microsoft.EntityFrameworkCore;
using TaskManagementAssesmentt.DTOs;
using TaskManagementAssesmentt.Entities;
using TaskManagementAssesmentt.Entities._enum;
using TaskManagementAssesmentt.Repositories.Data;
using TaskManagementAssesmentt.Repositories.IRepository;

namespace TaskManagementAssesmentt.Repositories
{

    public class TaskRepository : Repository<TaskEntity>, ITaskRepository
    {
        public TaskRepository(TaskDbContext context) : base(context) { }

        public override async Task<TaskEntity?> GetByIdAsync(int id)
        {
            return await Context.Tasks
                .Include(t => t.AssignedTo)
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public override async Task<List<TaskEntity>> GetAllAsync()
        {
            return await Context.Tasks
                .Include(t => t.AssignedTo)
                .OrderByDescending(t => t.Priority)
                .ThenBy(t => t.DueDate)
                .ToListAsync();
        }

        public async Task<List<TaskEntity>> GetTasksByStatusAsync(TaskStatusEnum status)
        {
            return await Context.Tasks
                .Where(t => t.Status == status)
                .Include(t => t.AssignedTo)
                .OrderByDescending(t => t.Priority)
                .ThenBy(t => t.DueDate)
                .ToListAsync();
        }

        public async Task<List<TaskEntity>> GetTasksByAssigneeAsync(int assigneeId)
        {
            return await Context.Tasks
                .Where(t => t.AssignedToId == assigneeId)
                .Include(t => t.AssignedTo)
                .OrderByDescending(t => t.Priority)
                .ThenBy(t => t.DueDate)
                .ToListAsync();
        }

        public async Task<List<TaskEntity>> GetTasksByPriorityAsync(TaskPriorityEnum priority)
        {
            return await Context.Tasks
                .Where(t => t.Priority == priority)
                .Include(t => t.AssignedTo)
                .OrderBy(t => t.DueDate)
                .ToListAsync();
        }

        public async Task<List<TaskEntity>> GetOverdueTasksAsync()
        {
            return await Context.Tasks
                .Where(t => t.DueDate < DateTime.UtcNow && t.Status != TaskStatusEnum.Completed)
                .Include(t => t.AssignedTo)
                .OrderByDescending(t => t.Priority)
                .ThenBy(t => t.DueDate)
                .ToListAsync();
        }

        public async Task<List<TaskEntity>> SearchTasksAsync(string searchTerm)
        {
            var lowerSearchTerm = searchTerm.ToLower();
            return await Context.Tasks
                .Where(t => t.Title.ToLower().Contains(lowerSearchTerm) ||
                            (t.Description != null && t.Description.ToLower().Contains(lowerSearchTerm)))
                .Include(t => t.AssignedTo)
                .OrderByDescending(t => t.Priority)
                .ThenBy(t => t.DueDate)
                .ToListAsync();
        }

        public async Task<List<TaskEntity>> FilterTasksAsync(TaskStatusEnum? status, TaskPriorityEnum? priority, int? assigneeId)
        {
            var query = Context.Tasks.Include(t => t.AssignedTo);

            if (status.HasValue)
                query = (Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<TaskEntity, TeamMemberEntity?>)query.Where(t => t.Status == status);

            if (priority.HasValue)
                query = (Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<TaskEntity, TeamMemberEntity?>)query.Where(t => t.Priority == priority);

            if (assigneeId.HasValue)
                query = (Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<TaskEntity, TeamMemberEntity?>)query.Where(t => t.AssignedToId == assigneeId);

            return await query
                .OrderByDescending(t => t.Priority)
                .ThenBy(t => t.DueDate)
                .ToListAsync();
        }

        public async Task<PaginatedResponse<TaskEntity>> GetPaginatedTasksAsync(
         int pageNumber, int pageSize,TaskStatusEnum? status,TaskPriorityEnum? priority,
        int? assigneeId)
        {
            var query = Context.Tasks.AsQueryable();

            
            if (status.HasValue)
                query = query.Where(t => t.Status == status.Value);

            if (priority.HasValue)
                query = query.Where(t => t.Priority == priority.Value);

            if (assigneeId.HasValue)
                query = query.Where(t => t.AssignedToId == assigneeId.Value);

            var totalItems = await query.CountAsync();

            var items = await query
                .Include(t => t.AssignedTo) 
                .OrderByDescending(t => t.Priority)
                .ThenBy(t => t.DueDate)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PaginatedResponse<TaskEntity>
            {
                Items = items,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalItems
            };
        }

    }
}

