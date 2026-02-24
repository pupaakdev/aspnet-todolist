using Microsoft.EntityFrameworkCore;
using aspnet_todolist.Models;

namespace aspnet_todolist
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddDbContext<TodoDb>(opt => opt.UseInMemoryDatabase("TodoList"));
            builder.Services.AddDatabaseDeveloperPageExceptionFilter();
            builder.Services.AddOpenApi();
            var app = builder.Build();

            app.MapOpenApi();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/openapi/v1.json", "v1");
                options.RoutePrefix = "swagger";
            });

            app.MapGet("/api/todos", async (TodoDb db, bool? isComplete) =>
            {
                var query = db.Todos.AsQueryable();

                if (isComplete.HasValue)
                    query = query.Where(t => t.IsComplete == isComplete.Value);

                return Results.Ok(await query.ToListAsync());
            });

            app.MapGet("/api/todos/{id}", async (int id, TodoDb db) =>
            {
                var todo = await db.Todos.FindAsync(id);
                if (todo == null) return Results.NotFound();

                return Results.Ok(todo);
            });

            app.MapPost("/api/todos", async (Todo todo, TodoDb db) =>
            {
                db.Todos.Add(todo);
                await db.SaveChangesAsync();

                return Results.Created($"/todolist/{todo.Id}", todo);
            });

            app.MapPut("/api/todos/{id}", async (int id, Todo inputTodo, TodoDb db) =>
            {
                var todo = await db.Todos.FindAsync(id);

                if (todo == null) return Results.NotFound();

                todo.Name = inputTodo.Name;
                todo.IsComplete = inputTodo.IsComplete;
                await db.SaveChangesAsync();

                return Results.Ok(todo);
            });

            app.MapDelete("/api/todos/{id}", async (int id, TodoDb db) =>
            {
                var todo = await db.Todos.FindAsync(id);

                if (todo == null) return Results.NotFound();
                
                db.Todos.Remove(todo);
                await db.SaveChangesAsync();
                
                return Results.NoContent();
            });

            app.Run();
        }
    }
}
