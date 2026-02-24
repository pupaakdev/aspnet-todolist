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

            builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

            builder.Services.AddDbContext<TodoDb>(opt => opt.UseInMemoryDatabase("TodoList"));
            builder.Services.AddDatabaseDeveloperPageExceptionFilter();

            builder.Services.AddOpenApi();

            var app = builder.Build();

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
            app.MapGet("/api/todos", async (TodoDb db, bool? isComplete) =>
            {
                var query = db.Todos.AsQueryable();

                if (isComplete.HasValue)
                    query = query.Where(t => t.IsComplete == isComplete.Value);

                return Results.Ok(await query.ToListAsync());
            })
            .WithTags("Todos")
            .WithSummary("Retrieves all todo items")
            .WithDescription("Gets a list of all todo items. Optionally filter by completion status using the isComplete query parameter.");

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
            app.MapDelete("/api/todos/{id}", async (int id, TodoDb db) =>
            {
                var todo = await db.Todos.FindAsync(id);

                if (todo == null) return Results.NotFound();

                db.Todos.Remove(todo);
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
