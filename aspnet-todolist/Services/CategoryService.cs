using aspnet_todolist.DTOs;
using aspnet_todolist.Models;
using Microsoft.EntityFrameworkCore;

namespace aspnet_todolist.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly TodoDb _db;

        public CategoryService(TodoDb db)
        {
            _db = db;
        }

        public async Task<List<CategoryResponseDto>> GetAllAsync()
        {
            var categories = await _db.Categories.ToListAsync();
            return categories.Select(MapToDto).ToList();
        }

        public async Task<CategoryResponseDto?> GetByIdAsync(int id)
        {
            var category = await _db.Categories.FindAsync(id);
            if (category == null) return null;

            return MapToDto(category);
        }

        public async Task<CategoryResponseDto> CreateAsync(CategoryCreateDto categoryDto)
        {
            var category = new Category
            {
                Name = categoryDto.Name,
                Color = categoryDto.Color
            };

            _db.Categories.Add(category);
            await _db.SaveChangesAsync();

            return MapToDto(category);
        }

        public async Task<CategoryResponseDto?> UpdateAsync(int id, CategoryUpdateDto categoryDto)
        {
            var category = await _db.Categories.FindAsync(id);

            if (category == null) return null;

            category.Name = categoryDto.Name;
            category.Color = categoryDto.Color;
            await _db.SaveChangesAsync();

            return MapToDto(category);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var category = await _db.Categories.FindAsync(id);

            if (category == null) return false;

            _db.Categories.Remove(category);
            await _db.SaveChangesAsync();
            return true;
        }

        private static CategoryResponseDto MapToDto(Category category)
        {
            return new CategoryResponseDto(
                category.Id,
                category.Name!,
                category.Color!
            );
        }
    }
}
