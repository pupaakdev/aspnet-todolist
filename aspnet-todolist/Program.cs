using aspnet_todolist.Exceptions;
using aspnet_todolist.Models;
using aspnet_todolist.Services;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace aspnet_todolist
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddProblemDetails(options =>
            {
                options.CustomizeProblemDetails = context =>
                {
                    Activity? activity = context.HttpContext.Features.Get<IHttpActivityFeature>()?.Activity;
                    context.ProblemDetails.Extensions.TryAdd("traceId", activity?.Id);
                };
            });
            builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

            builder.Services.AddHttpLogging(logging =>
            {
                logging.LoggingFields = Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.RequestPropertiesAndHeaders |
                                       Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.ResponsePropertiesAndHeaders;
            });

            builder.Services.AddDbContext<TodoDb>(opt => 
                opt.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));
            builder.Services.AddDatabaseDeveloperPageExceptionFilter();

            builder.Services.AddTransient<DataSeeder>();

            builder.Services.AddOpenApi();
            builder.Services.AddValidation();

            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<TodoDb>();
                db.Database.Migrate();
            }

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
            app.MapGet("/api/todos", async (TodoDb db,
                bool? isComplete,
                bool? isDeleted,
                string? search,
                string? sortBy,
                int? page,
                int pageSize = 10,
                bool showDeleted = false,
                string sortOrder = "asc") =>
            {
                var query = db.Todos.Include(t => t.Category).AsQueryable();

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

                if (page.HasValue)
                {
                    var totalCount = await query.CountAsync();
                    var items = await query.Skip(((int)page - 1) * pageSize).Take(pageSize).ToListAsync();

                    var result = new PagedResult<Todo>
                    {
                        Items = items,
                        TotalCount = totalCount,
                        CurrentPage = (int)page,
                        PageSize = pageSize
                    };

                    return Results.Ok(result);
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
                var todo = await db.Todos.Include(t => t.Category).FirstOrDefaultAsync(t => t.Id == id);
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
            /// <response code="400">If the provided CategoryId does not exist.</response>
            app.MapPost("/api/todos", async (Todo todo, TodoDb db) =>
            {
                if (todo.CategoryId.HasValue)
                {
                    var categoryExists = await db.Categories.AnyAsync(c => c.Id == todo.CategoryId.Value);
                    if (!categoryExists)
                        return Results.BadRequest("Category does not exist");
                }

                db.Todos.Add(todo);
                await db.SaveChangesAsync();

                if (todo.CategoryId.HasValue)
                {
                    await db.Entry(todo).Reference(t => t.Category).LoadAsync();
                }

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
            /// <response code="400">If the provided CategoryId does not exist.</response>
            app.MapPut("/api/todos/{id}", async (int id, Todo inputTodo, TodoDb db) =>
            {
                var todo = await db.Todos.FindAsync(id);

                if (todo == null) return Results.NotFound();
                if (todo.IsDeleted == true) return Results.NotFound();

                if (inputTodo.CategoryId.HasValue)
                {
                    var categoryExists = await db.Categories.AnyAsync(c => c.Id == inputTodo.CategoryId.Value);
                    if (!categoryExists)
                        return Results.BadRequest("Category does not exist");
                }

                todo.Name = inputTodo.Name;
                todo.IsComplete = inputTodo.IsComplete;
                todo.CategoryId = inputTodo.CategoryId;
                await db.SaveChangesAsync();

                if (todo.CategoryId.HasValue)
                {
                    await db.Entry(todo).Reference(t => t.Category).LoadAsync();
                }

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

            /// <summary>
            /// Retrieves all categories.
            /// </summary>
            /// <returns>A list of categories.</returns>
            /// <response code="200">Returns the list of categories.</response>
            app.MapGet("/api/categories", async (TodoDb db) =>
            {
                var categories = await db.Categories.ToListAsync();
                return Results.Ok(categories);
            })
            .WithTags("Categories")
            .WithSummary("Retrieves all categories")
            .WithDescription("Gets a list of all categories.");

            /// <summary>
            /// Retrieves a specific category by id.
            /// </summary>
            /// <param name="id">The id of the category to retrieve.</param>
            /// <returns>The category with the specified id.</returns>
            /// <response code="200">Returns the category.</response>
            /// <response code="404">If the category is not found.</response>
            app.MapGet("/api/categories/{id}", async (int id, TodoDb db) =>
            {
                var category = await db.Categories.FindAsync(id);
                if (category == null) return Results.NotFound();

                return Results.Ok(category);
            })
            .WithTags("Categories")
            .WithSummary("Retrieves a specific category")
            .WithDescription("Gets a single category by its unique identifier.");

            /// <summary>
            /// Creates a new category.
            /// </summary>
            /// <param name="category">The category to create.</param>
            /// <returns>The newly created category.</returns>
            /// <response code="201">Returns the newly created category.</response>
            app.MapPost("/api/categories", async (Category category, TodoDb db) =>
            {
                db.Categories.Add(category);
                await db.SaveChangesAsync();

                return Results.Created($"/api/categories/{category.Id}", category);
            })
            .WithTags("Categories")
            .WithSummary("Creates a new category")
            .WithDescription("Creates a new category and adds it to the list.");

            /// <summary>
            /// Updates an existing category.
            /// </summary>
            /// <param name="id">The id of the category to update.</param>
            /// <param name="inputCategory">The updated category data.</param>
            /// <returns>The updated category.</returns>
            /// <response code="200">Returns the updated category.</response>
            /// <response code="404">If the category is not found.</response>
            app.MapPut("/api/categories/{id}", async (int id, Category inputCategory, TodoDb db) =>
            {
                var category = await db.Categories.FindAsync(id);

                if (category == null) return Results.NotFound();

                category.Name = inputCategory.Name;
                category.Color = inputCategory.Color;
                await db.SaveChangesAsync();

                return Results.Ok(category);
            })
            .WithTags("Categories")
            .WithSummary("Updates an existing category")
            .WithDescription("Updates the name and color of an existing category.");

            /// <summary>
            /// Deletes a specific category.
            /// </summary>
            /// <param name="id">The id of the category to delete.</param>
            /// <returns>No content.</returns>
            /// <response code="204">If the category was successfully deleted.</response>
            /// <response code="404">If the category is not found.</response>
            app.MapDelete("/api/categories/{id}", async (int id, TodoDb db) =>
            {
                var category = await db.Categories.FindAsync(id);

                if (category == null) return Results.NotFound();

                db.Categories.Remove(category);
                await db.SaveChangesAsync();
                return Results.NoContent();
            })
            .WithTags("Categories")
            .WithSummary("Deletes a specific category")
            .WithDescription("Removes a category from the list by its unique identifier.");

            if (app.Environment.IsDevelopment())
            {
                /// <summary>
                /// Resets and reseeds the database with sample data.
                /// </summary>
                /// <returns>A summary of seeded data.</returns>
                /// <response code="200">Returns the count of seeded items.</response>
                app.MapPost("/seed", async (TodoDb db, DataSeeder seeder) =>
                {
                    db.Todos.RemoveRange(db.Todos);
                    db.Categories.RemoveRange(db.Categories);
                    await db.SaveChangesAsync();

                    var categories = new List<Category>();
                    for (int i = 0; i < 20; i++)
                    {
                        var category = seeder.GenerateCategory();
                        categories.Add(category);
                        db.Categories.Add(category);
                    }
                    await db.SaveChangesAsync();

                    var todos = new List<Todo>();
                    for (int i = 0; i < 500; i++)
                    {
                        var todo = seeder.GenerateTodo();
                        todos.Add(todo);
                        db.Todos.Add(todo);
                    }
                    await db.SaveChangesAsync();

                    return Results.Ok(new
                    {
                        message = "Database reset and reseeded successfully",
                        categoriesCreated = categories.Count,
                        todosCreated = todos.Count
                    });
                })
                .WithTags("Development")
                .WithSummary("Resets and reseeds the database")
                .WithDescription("Development only: Clears all existing data and reseeds the database with sample categories and todos.");
            }

            app.Run();


        }
    }
}
