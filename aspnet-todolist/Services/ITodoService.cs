using aspnet_todolist.DTOs;
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
        Task<TodoResponseDto?> GetByIdAsync(int id);

        /// <summary>
        /// Creates a new todo item.
        /// </summary>
        Task<TodoResponseDto> CreateAsync(TodoCreateDto todoDto);

        /// <summary>
        /// Updates an existing todo item.
        /// </summary>
        Task<TodoResponseDto?> UpdateAsync(int id, TodoUpdateDto todoDto);

        /// <summary>
        /// Deletes a specific todo item.
        /// </summary>
        Task<bool> DeleteAsync(int id, bool hardDelete = false);
    }
}
