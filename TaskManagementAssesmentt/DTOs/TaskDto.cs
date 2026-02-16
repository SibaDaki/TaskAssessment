namespace TaskManagementAssesmentt.DTOs
{
    public class CreateTaskDto
    {
        public required string Title { get; set; }
        public string? Description { get; set; }
        public int Priority { get; set; } = 1; 
        public int? AssignedToId { get; set; }
        public DateTime? DueDate { get; set; }
    }

    public class UpdateTaskDto
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public int? Status { get; set; }
        public int? Priority { get; set; }
        public int? AssignedToId { get; set; }
        public DateTime? DueDate { get; set; }
    }

    public class TaskResponseDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Status { get; set; } = string.Empty;
        public string Priority { get; set; } = string.Empty;
        public TeamMemberDto? AssignedTo { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
    }

    public class StatusUpdateDto
    {
        public int Status { get; set; }
    }

    public class PriorityUpdateDto
    {
        public int Priority { get; set; }
    }
}
