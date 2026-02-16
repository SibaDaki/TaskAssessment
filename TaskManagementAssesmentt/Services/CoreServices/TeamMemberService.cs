using TaskManagementAssesmentt.DTOs;
using TaskManagementAssesmentt.Entities;
using TaskManagementAssesmentt.Entities._enum;
using TaskManagementAssesmentt.Helper;
using TaskManagementAssesmentt.Repositories.IRepository;
using TaskManagementAssesmentt.Services.IService;
using InvalidOperationException = TaskManagementAssesmentt.Helper.InvalidOperationException;

namespace TaskManagementAssesmentt.Services.CoreServices
{
   

    public class TeamMemberService : ITeamMemberService
    {
        private readonly ITeamMemberRepository _teamMemberRepository;
        private readonly ITaskRepository _taskRepository;

        public TeamMemberService(ITeamMemberRepository teamMemberRepository, ITaskRepository taskRepository)
        {
            _teamMemberRepository = teamMemberRepository;
            _taskRepository = taskRepository;
        }

        public async Task<TeamMemberDto> GetTeamMemberByIdAsync(int id)
        {
            var member = await _teamMemberRepository.GetByIdAsync(id)
                ?? throw new ResourceNotFoundException(nameof(TeamMemberEntity), id);
            return MapToDto(member);
        }

        public async Task<TeamMemberDetailDto> GetTeamMemberDetailAsync(int id)
        {
            var member = await _teamMemberRepository.GetTeamMemberWithTasksAsync(id)
                ?? throw new ResourceNotFoundException(nameof(TeamMemberEntity), id);

            var detail = new TeamMemberDetailDto
            {
                Id = member.Id,
                Name = member.Name,
                Email = member.Email,
                Role = member.Role,
                IsActive = member.IsActive,
                TaskCount = member.AssignedTasks.Count
            };

            return detail;
        }

        public async Task<List<TeamMemberDto>> GetAllTeamMembersAsync()
        {
            var members = await _teamMemberRepository.GetAllAsync();
            return members.Select(MapToDto).ToList();
        }

        public async Task<List<TeamMemberDto>> GetActiveTeamMembersAsync()
        {
            var members = await _teamMemberRepository.GetActiveTeamMembersAsync();
            return members.Select(MapToDto).ToList();
        }

        public async Task<List<TeamMemberDto>> SearchTeamMembersAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                throw new ValidationException("Search term cannot be empty.");

            var members = await _teamMemberRepository.SearchTeamMembersAsync(searchTerm);
            return members.Select(MapToDto).ToList();
        }

        public async Task<TeamMemberDto> CreateTeamMemberAsync(CreateTeamMemberDto dto)
        {
            ValidateCreateTeamMemberDto(dto);

            var existingMember = await _teamMemberRepository.GetByEmailAsync(dto.Email);
            if (existingMember != null)
                throw new ValidationException("A team member with this email already exists.");

            var member = new TeamMemberEntity
            {
                Name = dto.Name,
                Email = dto.Email,
                Role = dto.Role,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            await _teamMemberRepository.AddAsync(member);
            return MapToDto(member);
        }

        public async Task<TeamMemberDto> UpdateTeamMemberAsync(int id, UpdateTeamMemberDto dto)
        {
            var member = await _teamMemberRepository.GetByIdAsync(id)
                ?? throw new ResourceNotFoundException(nameof(TeamMemberEntity), id);

            if (!string.IsNullOrWhiteSpace(dto.Name))
            {
                if (dto.Name.Length > 100)
                    throw new ValidationException("Name cannot exceed 100 characters.");
                member.Name = dto.Name;
            }

            if (!string.IsNullOrWhiteSpace(dto.Email))
            {
                if (dto.Email.Length > 255)
                    throw new ValidationException("Email cannot exceed 255 characters.");

                var existingMember = await _teamMemberRepository.GetByEmailAsync(dto.Email);
                if (existingMember != null && existingMember.Id != id)
                    throw new ValidationException("A team member with this email already exists.");

                member.Email = dto.Email;
            }

            if (dto.Role != null)
            {
                if (dto.Role.Length > 100)
                    throw new ValidationException("Role cannot exceed 100 characters.");
                member.Role = dto.Role;
            }

            if (dto.IsActive.HasValue)
                member.IsActive = dto.IsActive.Value;

            member.UpdatedAt = DateTime.UtcNow;
            await _teamMemberRepository.UpdateAsync(member);
            return MapToDto(member);
        }

        public async Task<bool> DeactivateTeamMemberAsync(int id)
        {
            var member = await _teamMemberRepository.GetByIdAsync(id)
                ?? throw new ResourceNotFoundException(nameof(TeamMemberEntity), id);

            
            var activeTasks = await _taskRepository.GetTasksByAssigneeAsync(id);
            foreach (var task in activeTasks.Where(t => t.Status != TaskStatusEnum.Completed))
            {
                task.AssignedToId = null;
                task.UpdatedAt = DateTime.UtcNow;
                await _taskRepository.UpdateAsync(task);
            }

            member.IsActive = false;
            member.UpdatedAt = DateTime.UtcNow;
            await _teamMemberRepository.UpdateAsync(member);
            return true;
        }

        public async Task<bool> DeleteTeamMemberAsync(int id)
        {
            var member = await _teamMemberRepository.GetByIdAsync(id)
                ?? throw new ResourceNotFoundException(nameof(TeamMemberEntity), id);

            var assignedTasks = await _taskRepository.GetTasksByAssigneeAsync(id);
            if (assignedTasks.Any(t => t.Status != TaskStatusEnum.Completed))
                throw new InvalidOperationException("Cannot delete a team member with active assigned tasks. Reassign or complete the tasks first.");

            return await _teamMemberRepository.DeleteAsync(id);
        }

        private static TeamMemberDto MapToDto(TeamMemberEntity member)
        {
            return new TeamMemberDto
            {
                Id = member.Id,
                Name = member.Name,
                Email = member.Email,
                Role = member.Role,
                IsActive = member.IsActive
            };
        }

        private static void ValidateCreateTeamMemberDto(CreateTeamMemberDto dto)
        {
            var errors = new Dictionary<string, string[]>();

            if (string.IsNullOrWhiteSpace(dto.Name))
                errors["Name"] = new[] { "Name is required." };
            else if (dto.Name.Length > 100)
                errors["Name"] = new[] { "Name cannot exceed 100 characters." };

            if (string.IsNullOrWhiteSpace(dto.Email))
                errors["Email"] = new[] { "Email is required." };
            else if (dto.Email.Length > 255 || !IsValidEmail(dto.Email))
                errors["Email"] = new[] { "Email format is invalid." };

            if (errors.Any())
                throw new ValidationException("Team member validation failed.", errors);
        }

        private static bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
    }
}
