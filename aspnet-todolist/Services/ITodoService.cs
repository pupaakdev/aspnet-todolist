using aspnet_todolist.DTOs;
using aspnet_todolist.Models;
using FluentResults;

namespace aspnet_todolist.Services
{
    public interface ITodoService
    {
        /// <summary>
        /// Retrieves all todo items with optional filtering, sorting, and pagination.
        /// </summary>
        Task<Result<object>> GetAllAsync(
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
        Task<Result<TodoResponseDto>> GetByIdAsync(int id);

        /// <summary>
        /// Creates a new todo item.
        /// </summary>
        Task<Result<TodoResponseDto>> CreateAsync(TodoCreateDto todoDto);

        /// <summary>
        /// Updates an existing todo item.
        /// </summary>
        Task<Result<TodoResponseDto>> UpdateAsync(int id, TodoUpdateDto todoDto);

        /// <summary>
        /// Deletes a specific todo item.
        /// </summary>
        Task<Result<bool>> DeleteAsync(int id, bool hardDelete = false);
    }
}
