using TaskManagementAssesmentt.Entities;

namespace TaskManagementAssesmentt.Entities
{
    public class TaskEntity : AuditEntity
    {
        public required string Title { get; set; }
        public string? Description { get; set; }
        public TaskStatusEnum Status { get; set; } = TaskStatusEnum.Todo;
        public TaskPriorityEnum Priority { get; set; } = TaskPriorityEnum.Medium;
        public int? AssignedToId { get; set; }
        

        // Navigation property
        public TeamMemberEntity? AssignedTo { get; set; }
    }
}
