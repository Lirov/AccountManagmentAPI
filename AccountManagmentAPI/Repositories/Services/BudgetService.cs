using AccountManagmentAPI.Models;
using AccountManagmentAPI.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AccountManagmentAPI.Repositories.Services
{

    public class BudgetService : IBudgetService
    {
        private readonly AccountManagmentContext _context;

        public BudgetService(AccountManagmentContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Budget>> GetAllBudgetsAsync(string userId)
        {
            var budget = await _context.Budgets.Include(u => u.UserId).ToListAsync();
            return await _context.Budgets.ToListAsync();
        }

        public async Task<Budget> GetBudgetByIdAsync(Guid budgetId, string userId)
        {
            return await _context.Budgets.Include(b => b.Amount).FirstOrDefaultAsync(b => b.BudgetId == budgetId);
        }

        public async Task<Budget> GetBudgetByMonthAndCategoryAsync(string userId, int? categoryId, DateTime month)
        {
            return await _context.Budgets.FirstOrDefaultAsync(b => b.UserId == userId && b.CategoryId == categoryId && b.Month == month);
        }

        public async Task<Budget> CreateBudgetAsync(Budget budget, string userId)
        {
            budget.UserId = userId;
            _context.Budgets.Add(budget);
            await _context.SaveChangesAsync();
            return budget;
        }

        public async Task UpdateBudgetAsync(Budget budget, string userId)
        {
            var existingBudget = await _context.Budgets.FirstOrDefaultAsync(b => b.BudgetId == budget.BudgetId && b.UserId == userId);

            if (existingBudget == null) return;

            existingBudget.Amount = budget.Amount;
            existingBudget.Month = budget.Month;
            existingBudget.CategoryId = budget.CategoryId;
            existingBudget.Category = budget.Category;

            _context.Entry(existingBudget).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteBudgetAsync(Guid budgetId, string userId)
        {
            var budget = await _context.Budgets.FirstOrDefaultAsync(b => b.BudgetId == budgetId && b.UserId == userId);

            if (budget == null) return;

            _context.Budgets.Remove(budget);
            await _context.SaveChangesAsync();
        }
    }
}
