using AccountManagmentAPI.Models;
using AccountManagmentAPI.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AccountManagmentAPI.Repositories.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly AccountManagmentContext _context;

        public CategoryService(AccountManagmentContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Category>> GetAllCategoriesAsync(string userId)
        {
            return await _context.Categories.ToListAsync();
        }

        public async Task<Category> GetCategoryByIdAsync(Guid categoryId, string userId)
        {
            return await _context.Categories.FindAsync(categoryId);
        }

        public async Task<Category> CreateCategoryAsync(Category category, string userId)
        {

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
            return category;
        }

        public async Task UpdateCategoryAsync(Category category, string userId)
        {
            var existingCategory = await _context.Categories.FirstOrDefaultAsync(c => c.CategoryId == category.CategoryId);

            if (existingCategory == null) return;

            existingCategory.Name = category.Name;
            existingCategory.Type = category.Type;

            _context.Entry(existingCategory).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteCategoryAsync(Guid categoryId, string userId)
        {
            var category = await _context.Categories.FirstOrDefaultAsync(c => c.CategoryId == categoryId);

            if (category == null) return;

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
        }
    }
}
