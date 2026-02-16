namespace TaskManagementAssesmentt.Entities
{
    public class TeamMemberEntity : AuditEntity
    {
        public required string Name { get; set; }
        public required string Email { get; set; }
        public string? Role { get; set; }

        // Navigation property
        public ICollection<TaskEntity> AssignedTasks { get; set; } = new List<TaskEntity>();
    }
}
