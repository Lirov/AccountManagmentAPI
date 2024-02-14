using AccountManagmentAPI.Models;

namespace AccountManagmentAPI.Repositories.Interfaces
{
    public interface ITransactionService
    {
        Task<IEnumerable<Transaction>> GetAllTransactionsAsync(string userId);
        Task<IEnumerable<Transaction>> GetTransactionsByUserAsync(string userId);
        Task<Transaction> GetTransactionByIdAsync(Guid transactionId, string userId);
        Task<Transaction> CreateTransactionAsync(Transaction transaction, string userId);
        Task UpdateTransactionAsync(Transaction transaction, string userId);
        Task DeleteTransactionAsync(Guid transactionId, string userId);
    }
}
