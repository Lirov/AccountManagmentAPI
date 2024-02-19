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

        public async Task<Budget> GetBudgetByIdAsync(Guid budgetId, string userId, int categoryId)
        {
            return await _context.Budgets.Include(b => b.Amount).FirstOrDefaultAsync(b => b.BudgetId == budgetId && b.CategoryId == categoryId);
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

        public async Task<bool> IsTransactionWithinBudgetAsync(string userId, decimal transactionAmount, int? categoryId = null)
        {
            // Assuming GetBudgetByCategoryAsync is the revised method
            var budget = await GetBudgetByCategoryAsync(userId, categoryId);
            if (budget == null)
            {
                // No budget set, so no restriction
                return true;
            }

            var currentExpenditure = await GetCurrentExpenditureAsync(userId, categoryId);
            var projectedExpenditure = currentExpenditure + transactionAmount;

            // Check if the projected expenditure exceeds the budget
            return projectedExpenditure <= budget.Amount;
        }

        public async Task<Budget> GetBudgetByCategoryAsync(string userId, int? categoryId)
        {
            return await _context.Budgets.FirstOrDefaultAsync(b => b.UserId == userId && (!categoryId.HasValue || b.CategoryId == categoryId.Value));
        }

        public async Task<decimal> GetCurrentExpenditureAsync(string userId, int? categoryId)
        {
            IQueryable<Transaction> query = _context.Transactions
                                                     .Where(t => t.UserId == userId);

            if (categoryId.HasValue)
            {
                query = query.Where(t => t.CategoryId == categoryId.Value);
            }

            decimal totalExpenditure = await query.SumAsync(t => t.Amount);

            return totalExpenditure;
        }
    }
}
