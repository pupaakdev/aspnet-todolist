using aspnet_todolist.Models;

namespace aspnet_todolist.Services
{
    public interface ITodoService
    {
        /// <summary>
        /// Retrieves all todo items with optional filtering, sorting, and pagination.
        /// </summary>
        Task<object> GetAllAsync(
            bool? isComplete = null,
            bool? isDeleted = null,
            string? search = null,
            string? sortBy = null,
            int? page = null,
            int pageSize = 10,
            bool showDeleted = false,
            string sortOrder = "asc");

        /// <summary>
        /// Retrieves a specific todo item by id.
        /// </summary>
        Task<Todo?> GetByIdAsync(int id);

        /// <summary>
        /// Creates a new todo item.
        /// </summary>
        Task<Todo> CreateAsync(Todo todo);

        /// <summary>
        /// Updates an existing todo item.
        /// </summary>
        Task<Todo?> UpdateAsync(int id, Todo inputTodo);

        /// <summary>
        /// Deletes a specific todo item (soft delete by default).
        /// </summary>
        Task<bool> DeleteAsync(int id, bool hardDelete = false);
    }
}
