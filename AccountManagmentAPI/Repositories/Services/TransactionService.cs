using AccountManagmentAPI.Models;
using AccountManagmentAPI.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AccountManagmentAPI.Repositories.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly AccountManagmentContext _context;

        public TransactionService(AccountManagmentContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Transaction>> GetAllTransactionsAsync(string userId)
        {
            return await _context.Transactions.ToListAsync();
        }

        public async Task<Transaction> GetTransactionByIdAsync(Guid transactionId, string userId)
        {
            return await _context.Transactions.FindAsync(transactionId);
        }

        public async Task<IEnumerable<Transaction>> GetTransactionsByUserAsync(string userId)
        {
            var transactions = await _context.Transactions
                     .Where(t => t.UserId == userId)
                     .ToListAsync();

            return transactions;
        }

        public async Task<Transaction> CreateTransactionAsync(Transaction transaction, string userId)
        {
            transaction.UserId = userId;
            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();
            return transaction;
        }

        public async Task UpdateTransactionAsync(Transaction transaction, string userId)
        {
            var existingTransaction = await _context.Transactions.FirstOrDefaultAsync(t => t.TransactionId == transaction.TransactionId && t.UserId == userId);

            if (existingTransaction == null) return;

            existingTransaction.Amount = transaction.Amount;
            existingTransaction.Date = transaction.Date;
            existingTransaction.Description = transaction.Description;
            existingTransaction.Category = transaction.Category;

            _context.Entry(existingTransaction).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteTransactionAsync(Guid transactionId, string userId)
        {
            var transaction = await _context.Transactions.FirstOrDefaultAsync(t => t.TransactionId == transactionId && t.UserId == userId);

            if (transaction == null) return;

            _context.Transactions.Remove(transaction);
            await _context.SaveChangesAsync();
        }
    }
}
