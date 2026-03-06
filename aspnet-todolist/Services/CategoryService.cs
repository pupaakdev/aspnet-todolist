using aspnet_todolist.DTOs;
using aspnet_todolist.Models;
using FluentResults;
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

        public async Task<Result<List<CategoryResponseDto>>> GetAllAsync()
        {
            var categories = await _db.Categories.ToListAsync();
            return Result.Ok(categories.Select(MapToDto).ToList());
        }

        public async Task<Result<CategoryResponseDto>> GetByIdAsync(int id)
        {
            var category = await _db.Categories.FindAsync(id);
            if (category == null) return Result.Fail("Category not found");

            return Result.Ok(MapToDto(category));
        }

        public async Task<Result<CategoryResponseDto>> CreateAsync(CategoryCreateDto categoryDto)
        {
            var category = new Category
            {
                Name = categoryDto.Name,
                Color = categoryDto.Color
            };

            _db.Categories.Add(category);
            await _db.SaveChangesAsync();

            return Result.Ok(MapToDto(category));
        }

        public async Task<Result<CategoryResponseDto>> UpdateAsync(int id, CategoryUpdateDto categoryDto)
        {
            var category = await _db.Categories.FindAsync(id);

            if (category == null) return Result.Fail("Category not found");

            category.Name = categoryDto.Name;
            category.Color = categoryDto.Color;
            await _db.SaveChangesAsync();

            return Result.Ok(MapToDto(category));
        }

        public async Task<Result<bool>> DeleteAsync(int id)
        {
            var category = await _db.Categories.FindAsync(id);

            if (category == null) return Result.Fail("Category not found");

            _db.Categories.Remove(category);
            await _db.SaveChangesAsync();
            return Result.Ok();
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
