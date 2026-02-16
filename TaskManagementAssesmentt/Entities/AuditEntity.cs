namespace TaskManagementAssesmentt.Entities
{
    public class AuditEntity
    {
        public int Id { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public bool IsActive { get; set; } = true;

    }
}
