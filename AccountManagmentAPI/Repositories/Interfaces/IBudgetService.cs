using AccountManagmentAPI.Models;

namespace AccountManagmentAPI.Repositories.Interfaces
{
    public interface IBudgetService
    {
        Task<IEnumerable<Budget>> GetAllBudgetsAsync(string userId);
        Task<Budget> GetBudgetByIdAsync(Guid budgetId, string userId, int categoryId);
        Task<Budget> GetBudgetByCategoryAsync(string userId, int? categoryId);
        Task<decimal> GetCurrentExpenditureAsync(string userId, int? categoryId);
        Task<Budget> GetBudgetByMonthAndCategoryAsync(string userId, int? categoryId, DateTime month);
        Task<bool> IsTransactionWithinBudgetAsync(string userId, decimal transactionAmount, int? categoryId);
        Task<Budget> CreateBudgetAsync(Budget budget, string userId);
        Task UpdateBudgetAsync(Budget budget, string userId);
        Task DeleteBudgetAsync(Guid budgetId, string userId);
    }
}
