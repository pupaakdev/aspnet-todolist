# ASP.NET Core Web API - Learning Tasks

Welcome! This document contains a series of tasks to help you learn ASP.NET Core Web API development. The tasks are organized from beginner to intermediate level. Complete them in order for the best learning experience.

---

## Current Project Overview

You've built a simple Todo API with:

- Minimal API endpoints (GET, POST, PUT)
- Entity Framework Core with In-Memory database
- A single `Todo` model

**Great start!** Now let's expand your knowledge.

---

## Task 1: Update Endpoints to Follow REST Conventions

**Goal:** Learn RESTful API design principles and update your endpoints to follow standard conventions.

**Why it matters:** REST is the most common API architecture style. Following REST conventions makes your API predictable and easier for other developers to use.

### Steps:

1. Rename the base route from `/todolist` to `/api/todos` (plural noun, with api prefix)
2. Ensure your endpoints follow REST conventions:
   - `GET /api/todos` - Get all todos
   - `GET /api/todos/{id}` - Get a single todo
   - `POST /api/todos` - Create a new todo
   - `PUT /api/todos/{id}` - Update a todo (full update)
   - `DELETE /api/todos/{id}` - Delete a todo
3. Use proper HTTP status codes:
   - `200 OK` for successful GET/PUT
   - `201 Created` for successful POST (with Location header)
   - `204 No Content` for successful DELETE
   - `404 Not Found` when resource doesn't exist
4. Remove the `/todolist/complete` endpoint (filtering should use query parameters instead)

### Hints:

- Search for "REST API naming conventions"
- Search for "REST API best practices"
- Resources should be nouns (todos), not verbs (getTodos)
- Use query parameters for filtering: `GET /api/todos?completed=true`

### Learn:

- What does REST stand for?
- What are the 6 REST constraints?
- What is the difference between PUT and PATCH?

---

## Task 2: Add Swagger/OpenAPI Documentation

**Goal:** Learn how to document your API and use Swagger UI for testing.

**Why it matters:** Real-world APIs need documentation. Swagger provides interactive docs that let developers test your API in the browser.

### Steps:

1. In `Program.cs`, add the OpenAPI services
2. Enable the Swagger middleware
3. Install and configure Swagger UI by adding the `Swashbuckle.AspNetCore` package
4. Run the app and visit `/swagger` to see your API documentation

### Hints:

- Search for "ASP.NET Core OpenAPI" in the official docs
- Look for `AddOpenApi()` and `MapOpenApi()` methods

### Bonus:

- Add XML comments to your endpoints and configure Swagger to display them
- Group endpoints using tags

---

## Task 3: Add Error Handling

**Goal:** Learn proper error handling and consistent error responses.

**Why it matters:** Every production API needs proper error handling. Without it, unhandled exceptions expose sensitive information and provide poor user experience. A global exception handler catches all errors in one place.

### Steps:

1. Create a global exception handler using `IExceptionHandler` interface
2. Create a standard error response class/record (e.g., `ApiError` with Message, StatusCode, Details)
3. Handle common exceptions (e.g., `DbUpdateException`, `ValidationException`)
4. Return appropriate HTTP status codes (400, 404, 500, etc.)
5. Make sure to NOT expose stack traces or sensitive info in production

### Hints:

- Search for "ASP.NET Core global exception handling"
- Look up "Problem Details" (RFC 7807) - it's a standard format for API errors
- Research `IExceptionHandler` interface (the modern approach)
- Check the difference between Development and Production error responses

### Bonus:

- Use the Problem Details standard (RFC 7807)
- Add request logging to track errors

---

## Task 4: Add Input Validation

**Goal:** Learn how to validate user input before saving to the database.

**Why it matters:** Never trust user input! Validation prevents bad data and security issues.

### Steps:

1. Add Data Annotations to the `Todo` model (e.g., `[Required]`, `[StringLength]`)
2. Create a validation filter or use `MiniValidator` package
3. Return proper `400 Bad Request` responses with validation errors

### Validation Requirements for Todo:

| Property     | Rules                                                 |
| ------------ | ----------------------------------------------------- |
| `Name`       | Required, minimum 1 character, maximum 100 characters |
| `IsComplete` | No validation needed (boolean defaults to false)      |

### Hints:

- Search for "System.ComponentModel.DataAnnotations"
- Look into attributes like `[Required]`, `[StringLength]`, `[Range]`
- For unique validation (used later in Category): throw an exception if the value is not unique - the global exception handler will catch it

### Bonus:

- Create a custom validation attribute (e.g., `NoProfanityAttribute`)
- Learn about FluentValidation as an alternative

---

## Task 5: Add a DELETE Endpoint

**Goal:** Complete the CRUD operations by adding delete functionality.

**Why it matters:** Most APIs need all CRUD operations (Create, Read, Update, Delete).

### Steps:

1. Add a new endpoint: `DELETE /todolist/{id}`
2. Find the todo by ID
3. Return `404 Not Found` if it doesn't exist
4. Delete the todo and save changes
5. Return `204 No Content` on success

### Bonus:

- Add a "soft delete" option using an `IsDeleted` property instead of actually removing the record

---

## Task 6: Fix the UPDATE Endpoint

**Goal:** Update the existing PUT endpoint to properly update the entire todo item, not just mark it complete.

**Why it matters:** The current `/todolist/{id}/complete` endpoint only marks a todo as complete. A proper REST API should have a PUT endpoint that updates all properties.

### Steps:

1. Replace the existing `PUT /todolist/{id}/complete` with `PUT /api/todos/{id}`
2. Accept a todo object in the request body
3. Validate the input
4. Update all properties (Name, IsComplete)
5. Return the updated todo

### Consider:

- What if the ID in the URL doesn't match the ID in the body?
- Should you use PUT or PATCH? Research the difference!

---

## Task 7: Add Filtering, Sorting, and Pagination

**Goal:** Learn to handle large datasets efficiently.

**Why it matters:** Real APIs often return thousands of records. Returning everything at once is slow and wasteful. Filtering, sorting, and pagination are essential for performance and usability.

### Steps:

1. Add query parameters to `GET /todolist`:
   - `?completed=true` - filter by completion status
   - `?search=grocery` - search by name
   - `?sortBy=name&sortOrder=asc` - sorting
   - `?page=1&pageSize=10` - pagination
2. Create a `PagedResult<T>` class/record to return items with pagination metadata

### Hints:

- Use LINQ methods: `.Where()`, `.OrderBy()`, `.Skip()`, `.Take()`
- Search for "ASP.NET Core pagination"
- Think about: what metadata does the client need? (total count, current page, page size, total pages)

---

## Task 8: Switch to a Real Database (SQLite)

**Goal:** Learn to use a persistent database instead of in-memory.

**Why it matters:** In-memory databases are great for testing, but real applications need data that persists between restarts. This also introduces you to EF Core migrations.

### Steps:

1. Install the `dotnet-ef` tool globally
2. Add the required NuGet packages: `Microsoft.EntityFrameworkCore.Sqlite` and `Microsoft.EntityFrameworkCore.Design`
3. Add a connection string in `appsettings.json`
4. Update `Program.cs` to use SQLite instead of InMemory
5. Create and apply your first migration using the EF Core CLI

### Hints:

- Search for "EF Core SQLite getting started"
- Look up "EF Core migrations" in the official docs
- The connection string format for SQLite is simple: `Data Source=filename.db`

### Learn:

- What is a migration? Why do we need them?
- Explore the `Migrations` folder that gets created
- What happens if you change your model and run a new migration?

---

## Task 9: Create a Category Model (EF Core Relationships)

**Goal:** Learn Entity Framework Core relationships by adding categories to todos.

**Why it matters:** Real applications have related data. Understanding relationships is crucial.

### Steps:

1. Create a new model `Category.cs` with properties: `Id`, `Name`, `Color`, and a navigation property for Todos
2. Add a foreign key (`CategoryId`) and navigation property (`Category`) to `Todo`
3. Add `DbSet<Category>` to `TodoDb`
4. Create a new migration and update the database
5. Create CRUD endpoints for categories
6. Update the todo endpoints to include category information

### Validation Requirements for Category:

| Property | Rules                                                                |
| -------- | -------------------------------------------------------------------- |
| `Name`   | Required, minimum 1 character, maximum 50 characters, must be unique |
| `Color`  | Required, must be a valid hex color code (e.g., `#FF5733` or `#FFF`) |

**Important:** For the unique `Name` validation, check in your service/endpoint if a category with the same name already exists. If it does, throw a custom exception (e.g., `DuplicateCategoryException`). The global exception handler you created in Task 3 will catch this and return a proper error response.

### Hints:

- Search for "EF Core one-to-many relationship"
- Look up "navigation properties" in EF Core docs
- Research the difference between required and optional relationships (nullable foreign key)

### Bonus:

- Use `.Include()` to eager-load categories with todos
- Create an endpoint to get all todos in a specific category

---

## Task 10: Generate Seed Data

**Goal:** Learn how to populate your database with fake data for testing and development.

**Why it matters:** Testing your API with realistic data is essential. You need more than 2-3 manually created records to properly test filtering, sorting, and pagination.

### Steps:

1. Install the `Bogus` NuGet package (a popular fake data generator)
2. Create a `DataSeeder` class that generates fake todos and categories
3. Generate at least 500 todos across multiple categories with varied:
   - Names (realistic task descriptions)
   - Completion status (mix of complete and incomplete)
   - Categories (randomly assigned)
4. Call the seeder when the application starts (only if the database is empty)

### Hints:

- Search for "Bogus C# fake data"
- Look at the Bogus GitHub page for examples
- Check `!context.Todos.Any()` before seeding to avoid duplicates
- Do NOT use `context.Database.EnsureCreated()` - it doesn't work with migrations! Use migrations instead.
- Consider creating a separate seeding method for development vs production

### Bonus:

- Create a `/seed` endpoint (development only) to reset and reseed the database
- Use `Faker<T>` rules to generate consistent, realistic data

---

## Task 11: Use DTOs and Create a Service Layer

**Goal:** Learn to separate your API contracts from your database models, and organize your code with a proper business logic layer.

**Why it matters:** You often don't want to expose all database fields to the client, and you might want different shapes for input vs output. Additionally, putting all logic in endpoints makes code hard to test and maintain. Services provide a clean separation of concerns.

### Steps:

1. Create a `DTOs` folder
2. Create a `TodoCreateDto` record with properties needed for creating a todo
3. Create a `TodoResponseDto` record with properties you want to return to the client
4. Create a `Services` folder
5. Create an `ITodoService` interface with methods like `GetAllAsync()`, `GetByIdAsync(int id)`, `CreateAsync(TodoCreateDto dto)`, etc.
6. Create a `TodoService` class that implements `ITodoService` and contains all the business logic
7. Register the service in `Program.cs` using dependency injection
8. Inject `ITodoService` into your endpoints instead of using `TodoDb` directly
9. Consider using a library like `Mapster` or `AutoMapper` for mapping

### Hints:

- Search for "C# record types"
- Research "DTO pattern" and why it's useful
- Search for "ASP.NET Core dependency injection"
- Look up "repository pattern" and "service layer pattern"
- Think about: what fields should the client send vs receive?
- Think about: what logic belongs in a service vs in an endpoint?

---

## Task 12: Implement the Result Pattern

**Goal:** Learn how to handle errors gracefully in your service layer without throwing exceptions.

**Why it matters:** Throwing exceptions for expected failures (like "todo not found") is expensive and makes error handling harder. The Result pattern provides a clean way to return success or failure from methods, making your code more explicit and easier to test.

### Steps:

1. Install a Result library NuGet package (e.g., `FluentResults`, `Ardalis.Result`, or `ErrorOr`)
2. Update your service interface methods to return `Result<T>` instead of just `T`
3. Modify `GetByIdAsync()` to return a failure result when the todo is not found
4. Modify `DeleteAsync()` to return a failure result when the todo doesn't exist
5. Modify `UpdateAsync()` to return a failure result for not found or validation errors
6. Update your endpoints/controllers to handle the Result and return appropriate HTTP status codes
7. Create custom error types if needed (e.g., `NotFoundError`, `ValidationError`)

### Example Scenarios to Handle:

| Operation              | Error Condition    | Result                                              |
| ---------------------- | ------------------ | --------------------------------------------------- |
| `GetByIdAsync(id)`     | Todo not found     | `Result.Fail("Todo not found")` or `NotFound` error |
| `DeleteAsync(id)`      | Todo not found     | `Result.Fail("Cannot delete: todo not found")`      |
| `UpdateAsync(id, dto)` | Todo not found     | `Result.Fail("Cannot update: todo not found")`      |
| `UpdateAsync(id, dto)` | Validation fails   | `Result.Fail(validationErrors)`                     |
| `CreateAsync(dto)`     | Category not found | `Result.Fail("Category does not exist")`            |

### Hints:

- Search for "Result pattern C#" or "Railway-oriented programming"
- Compare different Result libraries: FluentResults, Ardalis.Result, ErrorOr
- Look at how to map Result types to HTTP responses
- Think about: when should you throw exceptions vs return a Result?

### Learn:

- What are the advantages of Result pattern over exceptions?
- How does this pattern improve testability?
- What is "Railway-oriented programming"?

---

## Task 13: Add Unit Tests

**Goal:** Learn how to write unit tests for your service layer.

**Why it matters:** Tests catch bugs before they reach production. They also make refactoring safer and serve as documentation for how your code should behave. Having a service layer makes unit testing much easier!

### Steps:

1. Create a new test project: `dotnet new xunit -n aspnet-todolist.Tests`
2. Add a reference to your main project
3. Write tests for `TodoService`:
   - Test `GetAllAsync()` returns all todos
   - Test `GetByIdAsync()` returns the correct todo
   - Test `GetByIdAsync()` returns null for non-existent ID
   - Test `CreateAsync()` adds a new todo
   - Test `DeleteAsync()` removes a todo

### Hints:

- Search for "xUnit .NET testing"
- Do not mock the db-context, use in-memory db
- Use the Arrange-Act-Assert (AAA) pattern
- Each test should test ONE thing

### Learn:

- What is the difference between unit tests and integration tests?
- Why do we mock dependencies?
- What is code coverage and why does it matter?

---

## Task 14: Refactor to Use Controllers

**Goal:** Learn the traditional controller-based approach.

**Why it matters:** Many existing projects use controllers. You should understand both approaches.

### Steps:

1. Create a `Controllers` folder
2. Create `TodosController.cs` inheriting from `ControllerBase`
3. Move your endpoint logic to controller actions
4. Add `builder.Services.AddControllers()` and `app.MapControllers()`
5. Use attributes: `[ApiController]`, `[Route("api/[controller]")]`, `[HttpGet]`, etc.
6. **Remove all the minimal API endpoints** from `Program.cs` - don't keep both approaches running simultaneously

### Important:

Your `Program.cs` should be much cleaner after this refactoring. All the `app.MapGet()`, `app.MapPost()`, `app.MapPut()`, `app.MapDelete()` calls should be removed and replaced with controller actions.

---

## Task 15: Create Custom Middleware

**Goal:** Understand the ASP.NET Core request pipeline and how middleware works.

**Why it matters:** Middleware is the foundation of ASP.NET Core. Understanding it helps you add cross-cutting concerns like logging, timing, and custom headers.

### Steps:

1. Create a `Middleware` folder
2. Create a `RequestTimingMiddleware.cs` class that measures how long each request takes
3. The middleware should:
   - Accept a `RequestDelegate` in the constructor
   - Have an `InvokeAsync(HttpContext context)` method
   - Start a stopwatch, call `_next(context)`, then log the elapsed time
4. Register it in `Program.cs` using `app.UseMiddleware<T>()`
5. Check your console/logs to see the timing information

### Hints:

- Search for "ASP.NET Core custom middleware"
- Understand `RequestDelegate` - it represents the next middleware in the pipeline
- Use `System.Diagnostics.Stopwatch` for timing
- Use `ILogger<T>` for logging

### Bonus Tasks:

- Create a `RequestLoggingMiddleware` that logs request/response details
- Create an `ApiKeyMiddleware` that checks for a valid API key header
- Add a custom response header (e.g., `X-Request-Id`) using middleware
- Learn about `app.Use()`, `app.Map()`, and `app.Run()` inline middleware

### Learn:

- How does the middleware pipeline work?
- What does `await _next(context)` do?
- What's the difference between running code before vs after `_next()`?
- How does middleware order affect behavior?

---

## Task 16: Add Due Dates and Priority

**Goal:** Extend the model with more realistic properties and learn about proper time abstraction for testability.

### Steps:

1. Add `DueDate` (nullable DateTime) and `Priority` properties to `Todo`
2. Create a `Priority` enum with values like Low, Medium, High, Urgent
3. Create a new migration and update the database
4. Create an `ITimeProvider` interface with a method like `DateTime GetCurrentTime()`
5. Create a `TimeProvider` implementation that returns `DateTime.UtcNow`
6. Register `ITimeProvider` in dependency injection
7. Inject `ITimeProvider` into your service instead of using `DateTime.Now` directly
8. Add an endpoint to get overdue todos
9. Add an endpoint to get todos due today
10. Write unit tests for the overdue/due today logic by mocking `ITimeProvider`

### Hints:

- Search for "C# enum"
- Search for "testing DateTime in C#" or "time abstraction pattern"
- Think about: how do you query "overdue" items? What about "due today"?
- In tests, mock `ITimeProvider` to return a fixed date so your tests are deterministic

### Learn:

- Why is using `DateTime.Now` directly a problem for testing?
- How does dependency injection help with testability?
- Note: .NET 8+ has a built-in `TimeProvider` class - research it!

---

## Task 17: Add Integration Tests

**Goal:** Learn how to test your API endpoints end-to-end.

**Why it matters:** Unit tests verify individual components, but integration tests verify that everything works together correctly - routing, middleware, validation, database access, and response formatting.

### Steps:

1. Create a new test project or add to your existing one
2. Install `Microsoft.AspNetCore.Mvc.Testing` package
3. Create a `WebApplicationFactory<Program>` to host your API in-memory
4. Configure the test to use an in-memory database (so tests don't affect real data)
5. Write integration tests:
   - Test `GET /todolist` returns 200 OK with a list
   - Test `GET /todolist/{id}` returns 404 for non-existent ID
   - Test `POST /todolist` creates a new todo and returns 201 Created
   - Test `POST /todolist` with invalid data returns 400 Bad Request
   - Test `DELETE /todolist/{id}` returns 204 No Content
   - Test filtering and pagination work correctly
6. Run tests with `dotnet test`

### Hints:

- Search for "ASP.NET Core integration testing"
- Look up `WebApplicationFactory` in the official docs
- Use `HttpClient` to make requests to your test server
- Tests should be independent - each test should set up its own data
- Use `[Collection]` attribute if tests need to share resources

### Learn:

- When should you write unit tests vs integration tests?
- How do you handle database state between tests?
- What is the test pyramid?

---

## Additional Resources

- [ASP.NET Core Documentation](https://docs.microsoft.com/aspnet/core)
- [Entity Framework Core Documentation](https://docs.microsoft.com/ef/core)
- [Minimal APIs Tutorial](https://docs.microsoft.com/aspnet/core/tutorials/min-web-api)
- [REST API Best Practices](https://restfulapi.net/)

---

## Tips for Success

1. **Commit often** - Use Git to save your progress after each task
2. **Test your endpoints** - Use the `.http` file, Swagger, or tools like Postman
3. **Read error messages** - They usually tell you exactly what's wrong
4. **Google is your friend** - Professional developers search for solutions constantly
5. **Ask questions** - Don't be stuck for hours; ask for help!

---

Good luck! Each task builds on the previous ones, so take your time and make sure you understand each concept before moving on.
