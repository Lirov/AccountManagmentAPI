using AccountManagmentAPI.Models;

namespace AccountManagmentAPI.Repositories.Interfaces
{
    public interface IBudgetService
    {
        Task<IEnumerable<Budget>> GetAllBudgetsAsync(string userId);
        Task<Budget> GetBudgetByIdAsync(Guid budgetId, string userId);
        Task<Budget> CreateBudgetAsync(Budget budget, string userId);
        Task UpdateBudgetAsync(Budget budget, string userId);
        Task DeleteBudgetAsync(Guid budgetId, string userId);
    }
}
