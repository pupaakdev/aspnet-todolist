using Microsoft.EntityFrameworkCore;
using aspnet_todolist.Models;
using aspnet_todolist.Exceptions;

namespace aspnet_todolist
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddProblemDetails();
            builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

            builder.Services.AddHttpLogging(logging =>
            {
                logging.LoggingFields = Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.RequestPropertiesAndHeaders |
                                       Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.ResponsePropertiesAndHeaders;
            });

            builder.Services.AddDbContext<TodoDb>(opt => opt.UseInMemoryDatabase("TodoList"));
            builder.Services.AddDatabaseDeveloperPageExceptionFilter();

            builder.Services.AddOpenApi();
            builder.Services.AddValidation();

            var app = builder.Build();

            app.UseHttpLogging();
            app.UseExceptionHandler();

            app.MapOpenApi();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/openapi/v1.json", "v1");
                options.RoutePrefix = "swagger";
            });

            /// <summary>
            /// Retrieves all todo items.
            /// </summary>
            /// <param name="isComplete">Optional filter to get only completed or incomplete todos.</param>
            /// <returns>A list of todo items.</returns>
            /// <response code="200">Returns the list of todo items.</response>
            app.MapGet("/api/todos", async (TodoDb db, bool? isComplete, bool? isDeleted, string ? search, string ? sortBy, bool showDeleted = false, string sortOrder = "asc") =>
            {
                var query = db.Todos.AsQueryable();

                if (!showDeleted && !isDeleted.HasValue)
                    query = query.Where(t => t.IsDeleted == false);

                if (isDeleted.HasValue)
                    query = query.Where(t => t.IsDeleted == isDeleted.Value);

                if (isComplete.HasValue)
                    query = query.Where(t => t.IsComplete == isComplete.Value);

                if (!string.IsNullOrEmpty(search))
                    query = query.Where(t => t.Name!.Contains(search));

                if (!string.IsNullOrEmpty(sortBy))
                {
                    var isDescending = sortOrder.Equals("desc", StringComparison.OrdinalIgnoreCase);

                    query = sortBy.ToLower() switch
                    {
                        "id" => isDescending ? query.OrderByDescending(t => t.Id) : query.OrderBy(t => t.Id),
                        "name" => isDescending ? query.OrderByDescending(t => t.Name) : query.OrderBy(t => t.Name),
                        "complete" => isDescending ? query.OrderByDescending(t => t.IsComplete) : query.OrderBy(t => t.IsComplete),
                        _ => query
                    };
                }

                return Results.Ok(await query.ToListAsync());
            })
            .WithTags("Todos")
            .WithSummary("Retrieves all todo items")
            .WithDescription("Gets a list of all todo items. Optionally filter by completion status using the isComplete query parameter. Search todos by name using the search parameter. Sort using sortBy (id, name, iscomplete, isdeleted) and sortOrder (asc, desc) parameters.");

            /// <summary>
            /// Retrieves a specific todo item by id.
            /// </summary>
            /// <param name="id">The id of the todo item to retrieve.</param>
            /// <returns>The todo item with the specified id.</returns>
            /// <response code="200">Returns the todo item.</response>
            /// <response code="404">If the todo item is not found.</response>
            app.MapGet("/api/todos/{id}", async (int id, TodoDb db) =>
            {
                var todo = await db.Todos.FindAsync(id);
                if (todo == null) return Results.NotFound();
                if (todo.IsDeleted == true) return Results.NotFound();

                return Results.Ok(todo);
            })
            .WithTags("Todos")
            .WithSummary("Retrieves a specific todo item")
            .WithDescription("Gets a single todo item by its unique identifier.");

            /// <summary>
            /// Creates a new todo item.
            /// </summary>
            /// <param name="todo">The todo item to create.</param>
            /// <returns>The newly created todo item.</returns>
            /// <response code="201">Returns the newly created todo item.</response>
            app.MapPost("/api/todos", async (Todo todo, TodoDb db) =>
            {
                db.Todos.Add(todo);
                await db.SaveChangesAsync();

                return Results.Created($"/todolist/{todo.Id}", todo);
            })
            .WithTags("Todos")
            .WithSummary("Creates a new todo item")
            .WithDescription("Creates a new todo item and adds it to the list.");

            /// <summary>
            /// Updates an existing todo item.
            /// </summary>
            /// <param name="id">The id of the todo item to update.</param>
            /// <param name="inputTodo">The updated todo item data.</param>
            /// <returns>The updated todo item.</returns>
            /// <response code="200">Returns the updated todo item.</response>
            /// <response code="404">If the todo item is not found.</response>
            app.MapPut("/api/todos/{id}", async (int id, Todo inputTodo, TodoDb db) =>
            {
                var todo = await db.Todos.FindAsync(id);

                if (todo == null) return Results.NotFound();
                if (todo.IsDeleted == true) return Results.NotFound();

                todo.Name = inputTodo.Name;
                todo.IsComplete = inputTodo.IsComplete;
                await db.SaveChangesAsync();

                return Results.Ok(todo);
            })
            .WithTags("Todos")
            .WithSummary("Updates an existing todo item")
            .WithDescription("Updates the name and completion status of an existing todo item.");

            /// <summary>
            /// Deletes a specific todo item.
            /// </summary>
            /// <param name="id">The id of the todo item to delete.</param>
            /// <returns>No content.</returns>
            /// <response code="204">If the todo item was successfully deleted.</response>
            /// <response code="404">If the todo item is not found.</response>
            app.MapDelete("/api/todos/{id}", async (int id, TodoDb db, bool hardDelete = false) =>
            {
                var todo = await db.Todos.FindAsync(id);

                if (todo == null) return Results.NotFound();
                if (todo.IsDeleted == true && !hardDelete) return Results.NotFound();

                if (hardDelete)
                    db.Todos.Remove(todo);
                else
                    todo.IsDeleted = true;

                await db.SaveChangesAsync();
                return Results.NoContent();
            })
            .WithTags("Todos")
            .WithSummary("Deletes a specific todo item")
            .WithDescription("Removes a todo item from the list by its unique identifier.");

            app.Run();
        }
    }
}
