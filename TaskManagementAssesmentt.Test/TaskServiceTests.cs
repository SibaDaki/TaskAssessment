using Moq;
using TaskManagementAssesmentt.Services.CoreServices;
using TaskManagementAssesmentt.Repositories.IRepository;
using TaskManagementAssesmentt.DTOs;
using TaskManagementAssesmentt.Entities;
using TaskManagementAssesmentt.Helper;

public class TaskServiceTests
{
    private readonly Mock<ITaskRepository> _taskRepositoryMock;
    private readonly Mock<ITeamMemberRepository> _teamMemberRepositoryMock;
    private readonly TaskService _taskService;

    public TaskServiceTests()
    {
        _taskRepositoryMock = new Mock<ITaskRepository>();
        _teamMemberRepositoryMock = new Mock<ITeamMemberRepository>();
        _taskService = new TaskService(
            _taskRepositoryMock.Object,
            _teamMemberRepositoryMock.Object);
    }

    [Fact]
    public async Task GetTaskByIdAsync_ShouldReturnTask_WhenTaskExists()
    {
        // Arrange
        var taskEntity = new TaskEntity
        {
            Id = 1,
            Title = "Test Task",
            Priority = TaskPriorityEnum.High,
            Status = TaskStatusEnum.Todo,
            CreatedAt = DateTime.UtcNow
        };

        _taskRepositoryMock
            .Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(taskEntity);

        // Act
        var result = await _taskService.GetTaskByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Test Task", result.Title);
        Assert.Equal("High", result.Priority);
    }

    [Fact]
    public async Task GetTaskByIdAsync_ShouldThrowResourceNotFound_WhenTaskDoesNotExist()
    {
        // Arrange
        _taskRepositoryMock
            .Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync((TaskEntity)null);

        // Act & Assert
        await Assert.ThrowsAsync<ResourceNotFoundException>(
            () => _taskService.GetTaskByIdAsync(1));
    }

    [Fact]
    public async Task CreateTaskAsync_ShouldThrowValidationException_WhenTitleIsEmpty()
    {
        // Arrange
        var dto = new CreateTaskDto
        {
            Title = "",
            Priority = 1
        };

        // Act & Assert
        await Assert.ThrowsAsync<ValidationException>(
            () => _taskService.CreateTaskAsync(dto));
    }
}