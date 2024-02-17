using AccountManagmentAPI.Models;
using AccountManagmentAPI.Models.Helpers;

namespace AccountManagmentAPI.Repositories.Interfaces
{
    public interface ITransactionService
    {
        Task<IEnumerable<Transaction>> GetAllTransactionsAsync(string userId);
        Task<IEnumerable<Transaction>> GetTransactionsByUserAsync(string userId);
        Task<decimal> GetGeneralBalanceAsync(string userId);
        Task<IEnumerable<CategoryBalance>> GetBalanceByCategoryAsync(string userId);
        Task<Transaction> GetTransactionByIdAsync(Guid transactionId, string userId);
        Task<List<Transaction>> GetTransactionsForExportAsync(string userId, DateTime? startDate, DateTime? endDate);
        Task<string> GetGenerateCsvContentAsync(List<Transaction> transactions);
        Task<Transaction> CreateTransactionAsync(Transaction transaction, string userId);
        Task UpdateTransactionAsync(Transaction transaction, string userId);
        Task DeleteTransactionAsync(Guid transactionId, string userId);

    }
}
