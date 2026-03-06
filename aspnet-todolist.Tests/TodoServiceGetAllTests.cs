using aspnet_todolist.DTOs;
using aspnet_todolist.Models;
using aspnet_todolist.Services;
using Microsoft.EntityFrameworkCore;

namespace aspnet_todolist.Tests;

public class TodoServiceGetAllTests
{
    private static TodoDb CreateDb(string dbName)
    {
        var options = new DbContextOptionsBuilder<TodoDb>()
            .UseInMemoryDatabase(dbName)
            .Options;
        return new TodoDb(options);
    }

    [Fact]
    public async Task GetAllAsync_ByDefault_ReturnsAllNonDeletedTodos()
    {
        await using var db = CreateDb(nameof(GetAllAsync_ByDefault_ReturnsAllNonDeletedTodos));
        db.Todos.AddRange(
            new Todo { Name = "Active 1", IsDeleted = false },
            new Todo { Name = "Active 2", IsDeleted = false },
            new Todo { Name = "Active 3", IsDeleted = false },
            new Todo { Name = "Deleted",  IsDeleted = true }
        );
        await db.SaveChangesAsync();
        var service = new TodoService(db);

        var result = await service.GetAllAsync();

        var todos = Assert.IsType<List<TodoResponseDto>>(result.Value);
        Assert.Equal(3, todos.Count);
        Assert.All(todos, t => Assert.StartsWith("Active", t.Name));
    }

    [Fact]
    public async Task GetAllAsync_ByDefault_ReturnsOnlyNonDeletedTodos()
    {
        await using var db = CreateDb(nameof(GetAllAsync_ByDefault_ReturnsOnlyNonDeletedTodos));
        db.Todos.AddRange(
            new Todo { Name = "Active",  IsDeleted = false },
            new Todo { Name = "Deleted", IsDeleted = true }
        );
        await db.SaveChangesAsync();
        var service = new TodoService(db);

        var result = await service.GetAllAsync();

        var todos = Assert.IsType<List<TodoResponseDto>>(result.Value);
        Assert.Single(todos);
        Assert.Equal("Active", todos[0].Name);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task GetAllAsync_WhenIsCompleteProvided_ReturnsOnlyMatchingTodos(bool isComplete)
    {
        await using var db = CreateDb($"{nameof(GetAllAsync_WhenIsCompleteProvided_ReturnsOnlyMatchingTodos)}_{isComplete}");
        db.Todos.AddRange(
            new Todo { Name = "Complete", IsComplete = true, IsDeleted = false },
            new Todo { Name = "Incomplete", IsComplete = false, IsDeleted = false }
        );
        await db.SaveChangesAsync();
        var service = new TodoService(db);

        var result = await service.GetAllAsync(isComplete: isComplete);

        var todos = Assert.IsType<List<TodoResponseDto>>(result.Value);
        Assert.Single(todos);
        Assert.All(todos, t => Assert.Equal(isComplete, t.IsComplete));
    }
}
