namespace TaskManagementAssesmentt.DTOs
{
    public class CreateTeamMemberDto
    {
        public required string Name { get; set; }
        public required string Email { get; set; }
        public string? Role { get; set; }
    }

    public class UpdateTeamMemberDto
    {
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Role { get; set; }
        public bool? IsActive { get; set; }
    }

    public class TeamMemberDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Role { get; set; }
        public bool IsActive { get; set; }
    }

    public class TeamMemberDetailDto : TeamMemberDto
    {
        public int TaskCount { get; set; }
    }
}
