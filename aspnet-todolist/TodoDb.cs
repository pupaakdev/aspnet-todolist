using Microsoft.EntityFrameworkCore;
using aspnet_todolist.Models;

namespace aspnet_todolist
{
    public class TodoDb : DbContext
    {
        public TodoDb(DbContextOptions<TodoDb> options) : base(options) { }

        public DbSet<Todo> Todos => Set<Todo>();
    }
}
