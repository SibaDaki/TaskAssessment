using Microsoft.AspNetCore.Mvc;

using TaskManagementAssesmentt.DTOs;
using TaskManagementAssesmentt.Services.IService;

namespace TaskManagementAPI.Controllers;

/// <summary>
/// Controller for managing team members
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class TeamMembersController : ControllerBase
{
    private readonly ITeamMemberService _teamMemberService;
    private readonly ILogger<TeamMembersController> _logger;

    public TeamMembersController(ITeamMemberService teamMemberService, ILogger<TeamMembersController> logger)
    {
        _teamMemberService = teamMemberService;
        _logger = logger;
    }

    /// <summary>
    /// Get all team members
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(List<TeamMemberDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllTeamMembers()
    {
        _logger.LogInformation("Getting all team members");
        var members = await _teamMemberService.GetAllTeamMembersAsync();
        return Ok(members);
    }

    /// <summary>
    /// Get all active team members
    /// </summary>
    [HttpGet("active")]
    [ProducesResponseType(typeof(List<TeamMemberDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetActiveTeamMembers()
    {
        _logger.LogInformation("Getting active team members");
        var members = await _teamMemberService.GetActiveTeamMembersAsync();
        return Ok(members);
    }

    /// <summary>
    /// Search team members
    /// </summary>
    [HttpGet("search")]
    [ProducesResponseType(typeof(List<TeamMemberDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> SearchTeamMembers([FromQuery] string searchTerm)
    {
        _logger.LogInformation("Searching team members with term: {SearchTerm}", searchTerm);
        var members = await _teamMemberService.SearchTeamMembersAsync(searchTerm);
        return Ok(members);
    }

    /// <summary>
    /// Get a specific team member by ID
    /// </summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(TeamMemberDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetTeamMemberById(int id)
    {
        _logger.LogInformation("Getting team member with ID: {MemberId}", id);
        var member = await _teamMemberService.GetTeamMemberDetailAsync(id);
        return Ok(member);
    }

    /// <summary>
    /// Create a new team member
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(TeamMemberDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateTeamMember([FromBody] CreateTeamMemberDto dto)
    {
        _logger.LogInformation("Creating new team member: {Name} ({Email})", dto.Name, dto.Email);
        var member = await _teamMemberService.CreateTeamMemberAsync(dto);
        return CreatedAtAction(nameof(GetTeamMemberById), new { id = member.Id }, member);
    }

    /// <summary>
    /// Update an existing team member
    /// </summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(TeamMemberDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateTeamMember(int id, [FromBody] UpdateTeamMemberDto dto)
    {
        _logger.LogInformation("Updating team member with ID: {MemberId}", id);
        var member = await _teamMemberService.UpdateTeamMemberAsync(id, dto);
        return Ok(member);
    }

    /// <summary>
    /// Deactivate a team member (soft delete - unassigns all active tasks)
    /// </summary>
    [HttpPatch("{id:int}/deactivate")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeactivateTeamMember(int id)
    {
        _logger.LogInformation("Deactivating team member with ID: {MemberId}", id);
        await _teamMemberService.DeactivateTeamMemberAsync(id);
        return NoContent();
    }

    /// <summary>
    /// Delete a team member (only if no active tasks assigned)
    /// </summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteTeamMember(int id)
    {
        _logger.LogInformation("Deleting team member with ID: {MemberId}", id);
        await _teamMemberService.DeleteTeamMemberAsync(id);
        return NoContent();
    }
}