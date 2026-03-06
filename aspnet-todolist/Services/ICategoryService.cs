using aspnet_todolist.DTOs;
using FluentResults;

namespace aspnet_todolist.Services
{
    public interface ICategoryService
    {
        /// <summary>
        /// Retrieves all categories.
        /// </summary>
        Task<Result<List<CategoryResponseDto>>> GetAllAsync();

        /// <summary>
        /// Retrieves a specific category by id.
        /// </summary>
        Task<Result<CategoryResponseDto>> GetByIdAsync(int id);

        /// <summary>
        /// Creates a new category.
        /// </summary>
        Task<Result<CategoryResponseDto>> CreateAsync(CategoryCreateDto categoryDto);

        /// <summary>
        /// Updates an existing category.
        /// </summary>
        Task<Result<CategoryResponseDto>> UpdateAsync(int id, CategoryUpdateDto categoryDto);

        /// <summary>
        /// Deletes a specific category.
        /// </summary>
        Task<Result<bool>> DeleteAsync(int id);
    }
}
