using aspnet_todolist.Models;
using aspnet_todolist.Services;
using Microsoft.EntityFrameworkCore;

namespace aspnet_todolist.Tests;

public class TodoServiceGetByIdTests
{
    private static TodoDb CreateDb(string dbName)
    {
        var options = new DbContextOptionsBuilder<TodoDb>()
            .UseInMemoryDatabase(dbName)
            .Options;
        return new TodoDb(options);
    }

    [Fact]
    public async Task GetByIdAsync_WhenTodoExists_ReturnsCorrectTodo()
    {
        await using var db = CreateDb(nameof(GetByIdAsync_WhenTodoExists_ReturnsCorrectTodo));
        db.Todos.AddRange(
            new Todo { Name = "Other Todo", IsComplete = false, IsDeleted = false },
            new Todo { Name = "My Todo", IsComplete = true, IsDeleted = false }
        );
        await db.SaveChangesAsync();
        var targetId = db.Todos.Single(t => t.Name == "My Todo").Id;
        var service = new TodoService(db);

        var result = await service.GetByIdAsync(targetId);

        Assert.True(result.IsSuccess);
        Assert.Equal("My Todo", result.Value.Name);
        Assert.True(result.Value.IsComplete);
    }

    [Fact]
    public async Task GetByIdAsync_WhenTodoDoesNotExist_ReturnsFailure()
    {
        await using var db = CreateDb(nameof(GetByIdAsync_WhenTodoDoesNotExist_ReturnsFailure));
        var service = new TodoService(db);

        var result = await service.GetByIdAsync(999);

        Assert.True(result.IsFailed);
    }

    [Fact]
    public async Task GetByIdAsync_WhenTodoIsDeleted_ReturnsFailure()
    {
        await using var db = CreateDb(nameof(GetByIdAsync_WhenTodoIsDeleted_ReturnsFailure));
        db.Todos.Add(new Todo { Name = "Deleted Todo", IsDeleted = true });
        await db.SaveChangesAsync();
        var todo = db.Todos.Single();
        var service = new TodoService(db);

        var result = await service.GetByIdAsync(todo.Id);

        Assert.True(result.IsFailed);
    }
}
