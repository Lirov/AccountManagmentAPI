using AccountManagmentAPI.Models;

namespace AccountManagmentAPI.Repositories.Interfaces
{
    public interface ICategoryService
    {
        Task<IEnumerable<Category>> GetAllCategoriesAsync(string userId);
        Task<Category> GetCategoryByIdAsync(Guid categoryId, string userId);
        Task<Category> CreateCategoryAsync(Category category, string userId);
        Task UpdateCategoryAsync(Category category, string userId);
        Task DeleteCategoryAsync(Guid categoryId, string userId);
    }
}
