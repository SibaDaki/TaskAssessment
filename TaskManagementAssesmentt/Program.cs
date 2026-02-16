using Microsoft.EntityFrameworkCore;
using TaskManagementAssesmentt.Entities;

using TaskManagementAssesmentt.Repositories;
using TaskManagementAssesmentt.Repositories.Data;
using TaskManagementAssesmentt.Repositories.IRepository;
using TaskManagementAssesmentt.Services.CoreServices;
using TaskManagementAssesmentt.Services.IService;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<TaskDbContext>(options =>
    options.UseInMemoryDatabase("TaskDb"));

builder.Services.AddScoped<ITeamMemberRepository, TeamMemberRepository>();
builder.Services.AddScoped<ITaskRepository, TaskRepository>();
builder.Services.AddScoped<ITaskService, TaskService>();
builder.Services.AddScoped<ITeamMemberService, TeamMemberService>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<TaskDbContext>();

    db.Database.EnsureCreated();

    
    if (!db.TeamMembers.Any())
    {
        db.TeamMembers.AddRange(
            new TeamMemberEntity { Id = 1, Name = "Siba Daki", Email = "siba.d@test.com", Role = "Senior Developer", IsActive = true, CreatedAt = DateTime.UtcNow },
            new TeamMemberEntity { Id = 2, Name = "Joe Doe", Email = "joe.d@email.com", Role = "Junior Developer", IsActive = true, CreatedAt = DateTime.UtcNow }
        );
        db.SaveChanges();
    }
    if (!db.Tasks.Any())
    {
        db.Tasks.AddRange(
            new TaskEntity { Id = 1, Title = "Assessment", Description = "Complete the assessment.", Status = TaskStatusEnum.Completed, Priority = TaskPriorityEnum.High, AssignedToId = 1, CreatedAt = DateTime.UtcNow, DueDate = DateTime.UtcNow.AddDays(10) },
            new TaskEntity { Id = 2, Title = "Testing", Description = "API test.", Status = TaskStatusEnum.InProgress, Priority = TaskPriorityEnum.Critical, AssignedToId = 1, CreatedAt = DateTime.UtcNow, DueDate = DateTime.UtcNow.AddDays(12) },
            new TaskEntity { Id = 3, Title = "Submission", Description = "Submit the assessment on completion.", Status = TaskStatusEnum.InProgress, Priority = TaskPriorityEnum.High, AssignedToId = 2, CreatedAt = DateTime.UtcNow, DueDate = DateTime.UtcNow.AddDays(15) }
            
        );
        db.SaveChanges();
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
