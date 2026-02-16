using Microsoft.EntityFrameworkCore;
using TaskManagementAssesmentt.Entities;
using TaskManagementAssesmentt.Repositories.Data;
using TaskManagementAssesmentt.Repositories.IRepository;

namespace TaskManagementAssesmentt.Repositories
{
    public class TeamMemberRepository : Repository<TeamMemberEntity>, ITeamMemberRepository
    {
        public TeamMemberRepository(TaskDbContext context) : base(context) { }

        public override async Task<TeamMemberEntity?> GetByIdAsync(int id)
        {
            return await Context.TeamMembers
            .Include(tm => tm.AssignedTasks)
                .FirstOrDefaultAsync(tm => tm.Id == id);
        }

        public override async Task<List<TeamMemberEntity>> GetAllAsync()
        {
            return await Context.TeamMembers
                .Include(tm => tm.AssignedTasks)
            .OrderBy(tm => tm.Name)
                .ToListAsync();
        }

        public async Task<TeamMemberEntity?> GetByEmailAsync(string email)
        {
            return await Context.TeamMembers
                .Include(tm => tm.AssignedTasks)
                .FirstOrDefaultAsync(tm => tm.Email == email);
        }

        public async Task<List<TeamMemberEntity>> GetActiveTeamMembersAsync()
        {
            return await Context.TeamMembers
                .Where(tm => tm.IsActive)
                .Include(tm => tm.AssignedTasks)
            .OrderBy(tm => tm.Name)
                .ToListAsync();
        }

        public async Task<List<TeamMemberEntity>> SearchTeamMembersAsync(string searchTerm)
        {
            var lowerSearchTerm = searchTerm.ToLower();
            return await Context.TeamMembers
                .Where(tm => tm.Name.ToLower().Contains(lowerSearchTerm) ||
                             tm.Email.ToLower().Contains(lowerSearchTerm) ||
                             (tm.Role != null && tm.Role.ToLower().Contains(lowerSearchTerm)))
                .Include(tm => tm.AssignedTasks)
            .OrderBy(tm => tm.Name)
                .ToListAsync();
        }

        public async Task<TeamMemberEntity?> GetTeamMemberWithTasksAsync(int id)
        {
            return await Context.TeamMembers
                .Include(tm => tm.AssignedTasks.Where(t => t.Status != TaskStatusEnum.Completed))
                .FirstOrDefaultAsync(tm => tm.Id == id);
        }
    }
}
