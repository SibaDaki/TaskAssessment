using TaskManagementAssesmentt.DTOs;

namespace TaskManagementAssesmentt.Services.IService
{
    public interface ITeamMemberService
    {
        Task<TeamMemberDto> GetTeamMemberByIdAsync(int id);
        Task<TeamMemberDetailDto> GetTeamMemberDetailAsync(int id);
        Task<List<TeamMemberDto>> GetAllTeamMembersAsync();
        Task<List<TeamMemberDto>> GetActiveTeamMembersAsync();
        Task<List<TeamMemberDto>> SearchTeamMembersAsync(string searchTerm);
        Task<TeamMemberDto> CreateTeamMemberAsync(CreateTeamMemberDto dto);
        Task<TeamMemberDto> UpdateTeamMemberAsync(int id, UpdateTeamMemberDto dto);
        Task<bool> DeactivateTeamMemberAsync(int id);
        Task<bool> DeleteTeamMemberAsync(int id);
    }
}
