# Task Management API Assessment - Design & Trade-offs Summary

## Architecture Overview

The API follows **Clean Architecture** with four distinct layers: <br/>

Controllers (REST Endpoints) <br/>
    
Services (Business Logic) <br/>
    
Repositories (Data Abstraction) <br/>
    
DbContext (EF Core InMemory) <br/>

---

This separation ensures **testability**, **maintainability**, and **flexibility** to change individual layers without affecting others.

---

## Design Decisions & Trade-offs

### 1. Repository Pattern 
 Generic `IRepository<T>` base class with specialized repositories (TaskRepository, TeamMemberRepository)

**Why:** 
- Abstracts data access logic from services
- Easier to mock for unit testing
- Centralized query optimization

**Trade-off:** Extra abstraction layer prevents tight coupling to EF Core

---

### 2. Service Layer 
 Business logic encapsulation in TaskService and TeamMemberService

**Why:**
- Enforces business rules consistently
- Reusable across multiple endpoints
- Single responsibility principle

**Trade-off:** Maintainability

---

### 3. DTOs (Data Transfer Objects) 
**What:** Separate input/output models 

**Why:**
- Decouples API from domain models
- Clear API contracts
- Control what gets serialized

**Trade-off:** Requires manual DTO-to-Model mapping, but gives explicit control

---

### 4. Custom Exceptions
**What:** Three exception types mapped to HTTP status codes
- `ResourceNotFoundException` → 404
- `ValidationException` → 400  
- `InvalidOperationException` → 400

**Why:**
- Handles all exceptions consistently
- Business logic errors distinct from system errors
- Clean error responses

---

### 5. Enums for Status & Priority 
**What:** TaskStatus  and TaskPriority 

**Why:**
- Type-safe 
- Prevents invalid states
- Easy to validate
**Trade-off:** Can't add new statuses at runtime without code change 

