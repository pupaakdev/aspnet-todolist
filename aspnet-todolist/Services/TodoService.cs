using aspnet_todolist.Models;
using Microsoft.EntityFrameworkCore;

namespace aspnet_todolist.Services
{
    public class TodoService : ITodoService
    {
        private readonly TodoDb _db;

        public TodoService(TodoDb db)
        {
            _db = db;
        }

        public async Task<object> GetAllAsync(
            bool? isComplete = null,
            bool? isDeleted = null,
            string? search = null,
            string? sortBy = null,
            int? page = null,
            int pageSize = 10,
            bool showDeleted = false,
            string sortOrder = "asc")
        {
            var query = _db.Todos.Include(t => t.Category).AsQueryable();

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

                return result;
            }

            return await query.ToListAsync();
        }

        public async Task<Todo?> GetByIdAsync(int id)
        {
            var todo = await _db.Todos.Include(t => t.Category).FirstOrDefaultAsync(t => t.Id == id);
            if (todo == null) return null;
            if (todo.IsDeleted == true) return null;

            return todo;
        }

        public async Task<Todo> CreateAsync(Todo todo)
        {
            if (todo.CategoryId.HasValue)
            {
                var categoryExists = await _db.Categories.AnyAsync(c => c.Id == todo.CategoryId.Value);
                if (!categoryExists)
                    throw new ArgumentException("Category does not exist");
            }

            _db.Todos.Add(todo);
            await _db.SaveChangesAsync();

            if (todo.CategoryId.HasValue)
            {
                await _db.Entry(todo).Reference(t => t.Category).LoadAsync();
            }

            return todo;
        }

        public async Task<Todo?> UpdateAsync(int id, Todo inputTodo)
        {
            var todo = await _db.Todos.FindAsync(id);

            if (todo == null) return null;
            if (todo.IsDeleted == true) return null;

            if (inputTodo.CategoryId.HasValue)
            {
                var categoryExists = await _db.Categories.AnyAsync(c => c.Id == inputTodo.CategoryId.Value);
                if (!categoryExists)
                    throw new ArgumentException("Category does not exist");
            }

            todo.Name = inputTodo.Name;
            todo.IsComplete = inputTodo.IsComplete;
            todo.CategoryId = inputTodo.CategoryId;
            await _db.SaveChangesAsync();

            if (todo.CategoryId.HasValue)
            {
                await _db.Entry(todo).Reference(t => t.Category).LoadAsync();
            }

            return todo;
        }

        public async Task<bool> DeleteAsync(int id, bool hardDelete = false)
        {
            var todo = await _db.Todos.FindAsync(id);

            if (todo == null) return false;
            if (todo.IsDeleted == true && !hardDelete) return false;

            if (hardDelete)
                _db.Todos.Remove(todo);
            else
                todo.IsDeleted = true;

            await _db.SaveChangesAsync();
            return true;
        }
    }
}
