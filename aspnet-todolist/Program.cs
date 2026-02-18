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
            var app = builder.Build();

            app.MapGet("/", () => "Hello world!");

            app.MapGet("/todolist", async (TodoDb db) => await db.Todos.ToListAsync());

            app.MapPost("/todolist", async (Todo todo, TodoDb db) =>
            {
                db.Todos.Add(todo);
                await db.SaveChangesAsync();

                return Results.Created();
            });

            app.MapPut("/todolist/{id}/complete", async (int id, TodoDb db) =>
            {
                var todo = await db.Todos.FindAsync(id);

                if (todo == null) return Results.NotFound();

                todo.IsComplete = true;
                await db.SaveChangesAsync();

                return Results.Ok();
            });

            app.Run();
        }
    }
}
