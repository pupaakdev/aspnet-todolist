using aspnet_todolist.DTOs;

namespace aspnet_todolist.Services
{
    public interface ICategoryService
    {
        /// <summary>
        /// Retrieves all categories.
        /// </summary>
        Task<List<CategoryResponseDto>> GetAllAsync();

        /// <summary>
        /// Retrieves a specific category by id.
        /// </summary>
        Task<CategoryResponseDto?> GetByIdAsync(int id);

        /// <summary>
        /// Creates a new category.
        /// </summary>
        Task<CategoryResponseDto> CreateAsync(CategoryCreateDto categoryDto);

        /// <summary>
        /// Updates an existing category.
        /// </summary>
        Task<CategoryResponseDto?> UpdateAsync(int id, CategoryUpdateDto categoryDto);

        /// <summary>
        /// Deletes a specific category.
        /// </summary>
        Task<bool> DeleteAsync(int id);
    }
}
