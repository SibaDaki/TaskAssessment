using System.Data.Entity.Core.Metadata.Edm;
using TaskManagementAssesmentt.Entities;

namespace TaskManagementAssesmentt.Repositories.IRepository
{
    public interface ITeamMemberRepository : IRepository<TeamMemberEntity>
    {
        Task<TeamMemberEntity?> GetByEmailAsync(string email);
        Task<List<TeamMemberEntity>> GetActiveTeamMembersAsync();
        Task<List<TeamMemberEntity>> SearchTeamMembersAsync(string searchTerm);
        Task<TeamMemberEntity?> GetTeamMemberWithTasksAsync(int id);
    }
}
