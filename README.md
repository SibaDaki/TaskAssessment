
# Task Management API Assessment

RESTful API for managing tasks and team members. Built with ASP.NET Core 8, Entity Framework Core, and following Clean Architecture principles.

## Features

### Task Management
-  View and search tasks with full-text search
-  Filter tasks by status, priority, and assignee
-  Create, update, and delete tasks
-  Assign tasks to team members
-  Set and update task priorities
-  Track task status (Todo, InProgress, Review, Completed, Blocked)
- Pagination support with configurable page size

### Team Member Management
-  Create and manage team members
-  Deactivate/activate team members
-  Search team members
-  View detailed member information 


### Developer Features
-  Comprehensive error handling with custom exceptions
-  Global exception middleware
-  Structured logging
-  Swagger/OpenAPI documentation
-  DTOs for API contracts
-  Repository pattern for data access
-  Service layer for business logic
-  In-memory database with seed data
-  Clean Architecture

## Technology Stack

- **Framework**: ASP.NET Core 8
- **ORM**: Entity Framework Core 8
- **Database**: In-Memory (EF Core InMemory Provider)
- **Documentation**: Swagger/OpenAPI
- **Language**: C# 12
- **Logging**: Microsoft.Extensions.Logging

## Prerequisites

- .NET 8 SDK or later
- Visual Studio 2022, VS Code, or any .NET-compatible IDE
  

## Installation & Setup

### 1. Clone or Download the Project

Go to https://github.com/SibaDaki/TaskAssessment)> to download the project
Unzip and open it with VS.
Once the project is loaded on vs, clean, build and run the project.
The API will start.


## API Documentation

### Swagger UI

Once the application is running,you will be able to access the interactive API documentation.



## API Endpoints

### Tasks
![Uploading image.png…]()


#### Get All Tasks
```
GET /api/tasks
```

Query Parameters:
- `status` (int): Filter by status (0=Todo, 1=InProgress, 2=Review, 3=Completed, 4=Blocked)
- `priority` (int): Filter by priority (0=Low, 1=Medium, 2=High, 3=Critical)
- `assigneeId` (int): Filter by assigned team member ID
- `pageNumber` (int): Page number (default: 1)
- `pageSize` (int): Items per page (default: 10, max: 100)


#### Create Task
```
POST /api/tasks
Content-Type: application/json

{
  "title": "Review Submitted assessment",
  "description": "Review all the submitted assessment",
  "priority": 3,
  "assignedToId": 1,
  "dueDate": "2026-02-16T00:00:00Z"
}
```

#### Update Task
```
PUT /api/tasks/{id}
Content-Type: application/json

{
  "title": "Updated title",
  "description": "Updated description",
  "status": 1,
  "priority": 2,
  "assignedToId": 1,
  "dueDate": "2024-03-15T00:00:00Z"
}
```

#### Delete Task
```
DELETE /api/tasks/{id}
```

#### Assign Task to Team Member
```
PATCH /api/tasks/{taskId}/assign/{teamMemberId}
```

#### Update Task Status
```
PATCH /api/tasks/{id}/status
Content-Type: application/json

{
  "status": 3
}
```

Status codes: 0=Todo, 1=InProgress, 2=Review, 3=Completed, 4=Blocked

#### Update Task Priority
```
PATCH /api/tasks/{id}/priority
Content-Type: application/json

{
  "priority": 2
}
```

Priority codes: 0=Low, 1=Medium, 2=High, 3=Critical

### Team Members

#### Get All Team Members
```
GET /api/teamMembers
```

#### Get Active Team Members
```
GET /api/teamMembers/active
```

#### Search Team Members
```
GET /api/teamMembers/search?searchTerm=Alice
```

#### Get Team Member by ID
```
GET /api/teamMembers/{id}
```

#### Create Team Member
```
POST /api/teamMembers
Content-Type: application/json

{
  "name": "John Doe",
  "email": "john.doe@company.com",
  "role": "Senior Developer"
}
```

#### Update Team Member
```
PUT /api/teamMembers/{id}
Content-Type: application/json

{
  "name": "Jane Doe",
  "email": "jane.doe@company.com",
  "role": "Tech Lead",
  "isActive": true
}
```

#### Deactivate Team Member
```
PATCH /api/teamMembers/{id}/deactivate
```

#### Delete Team Member
```
DELETE /api/teamMembers/{id}
```

## Seed Data

The application comes with pre-seeded data on startup:

### Team Members
- Siba Daki (Senior Developer)
- Joe Doe (Junior Developer)

### Sample Tasks
- Assessment
- Testing
- Submission

## Error Handling

The API includes comprehensive error handling with detailed error responses:

### Error Response Format
```json
{
  "message": "Resource not found",
  "details": "Task with id 999 was not found.",
  "errors": null
}
```

### Common HTTP Status Codes
- **200 OK**: Successful GET, PUT, PATCH
- **201 Created**: Successful POST
- **204 No Content**: Successful DELETE
- **400 Bad Request**: Validation or invalid operation error
- **404 Not Found**: Resource not found
- **500 Internal Server Error**: Unexpected server error

## Enum Values

### Task Status
- `0` = Todo
- `1` = InProgress
- `2` = Review
- `3` = Completed
- `4` = Blocked

### Task Priority
- `0` = Low
- `1` = Medium
- `2` = High
- `3` = Critical

## Project Structure
```
TaskManagementAPI/
├── Controllers/
│   ├── TasksController.cs
│   └── TeamMembersController.cs

├── DTOs/
│   └── DTOs.cs

├── Helper/
│   └── Exceptions.cs

├── Models/
│   ├── Task.cs
│   └── TeamMember.cs

├── Repositories/
|    ├──Data/
│      └── TaskDbContext.cs
|    ├──IRepository/
|       └── IRepository.cs
|       └── ITaskRepository
|       └── ITeamMemberRepository
│   ├── Repository.cs
│   ├── TaskRepository.cs
│   └── TeamMemberRepository.cs
├── Services/
|   ├── IService
|       └── ITaskService.cs
|       └── ITeamMemberService.cs
|   ├── CoreService.cs
│       ├── TaskService.cs
│       └── TeamMemberService.cs
├── Program.cs
├── appsettings.json
└── TaskManagementAPI.csproj
```

## Design Patterns & Architecture

### Repository Pattern
The API uses the Repository pattern for data access, providing an abstraction layer between the service layer and the database context. This promotes loose coupling and easier testing.

### Service Layer
Business logic is encapsulated in service classes (`ITaskService`, `ITeamMemberService`) to maintain separation of concerns and make the API more maintainable.

### DTOs (Data Transfer Objects)
DTOs are used for API contracts, protecting domain models and ensuring clean API boundaries.

### Custom Exceptions
Domain-specific exceptions (`ResourceNotFoundException`, `ValidationException`, `InvalidOperationException`) provide meaningful error context.

### Global Exception Handling
The `ExceptionHandlingMiddleware` catches all unhandled exceptions and returns consistent error responses.

## Testing

To test the API, you can use:
- **Swagger UI**
- **Postman**: Import the Swagger/OpenAPI documentation


### Logging

Structured logging is configured via `ILogger<T>` injected into controllers. Logs are written to console and debug output.


## Troubleshooting

### Port Already in Use
If port 5001 is already in use, change it in `launchSettings.json` 

### Database Issues
If you encounter database issues, the in-memory database will be recreated on each application restart.

### Swagger Not Loading
Ensure you're using the correct port:
- Default: `https://localhost:5001/swagger`
- Custom: `https://localhost:YOUR_PORT/swagger`

