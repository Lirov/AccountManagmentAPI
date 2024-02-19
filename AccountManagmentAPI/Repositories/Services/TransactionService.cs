using AccountManagmentAPI.Models;
using AccountManagmentAPI.Models.Helpers;
using AccountManagmentAPI.Repositories.Interfaces;
using CsvHelper.Configuration;
using CsvHelper;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Text;

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
            return await _context.Transactions.Where(u => u.UserId == userId).ToListAsync();

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
            var currentBalance = await GetGeneralBalanceAsync(userId);

            if (transaction.Amount < 0)
            {

                if (currentBalance + transaction.Amount < 100)
                {
                    throw new InsufficientBalanceException("The transaction was cancelled. Not enough money in the account to maintain a minimum balance of 100.");
                }
            }

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

        public async Task<decimal> GetGeneralBalanceAsync(string userId)
        {
            var income = await _context.Transactions
                .Where(t => t.UserId == userId && t.Category.Type == "Income")
                .SumAsync(t => t.Amount);

            var expenses = await _context.Transactions
                .Where(t => t.UserId == userId && t.Category.Type == "Expense")
                .SumAsync(t => t.Amount);

            return income - expenses;
        }

        public async Task<IEnumerable<CategoryBalance>> GetBalanceByCategoryAsync(string userId)
        {
            var categoryBalances = await _context.Transactions
                .Where(t => t.UserId == userId)
                .GroupBy(t => t.CategoryId)
                .Select(group => new CategoryBalance
                {
                    CategoryId = group.Key,
                    TotalAmount = group.Sum(t => t.Amount),
                    CategoryName = group.First().Category.Name,
                    Type = group.First().Category.Type
                })
                .ToListAsync();

            return categoryBalances;
        }

        public async Task<List<Transaction>> GetTransactionsForExportAsync(string userId, DateTime? startDate, DateTime? endDate)
        {
            IQueryable<Transaction> query = _context.Transactions
                                                     .Include(t => t.Category)
                                                     .Where(t => t.UserId == userId);

            if (startDate.HasValue)
            {
                query = query.Where(t => t.Date >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                query = query.Where(t => t.Date <= endDate.Value);
            }

            return await query.ToListAsync();
        }

        public async Task<string> GetGenerateCsvContentAsync(List<Transaction> transactions)
        {
            using var memoryStream = new MemoryStream();
            using (var streamWriter = new StreamWriter(memoryStream, leaveOpen: true)) // Prevent streamWriter from closing memoryStream
            using (var csvWriter = new CsvWriter(streamWriter, new CsvConfiguration(CultureInfo.InvariantCulture) { Delimiter = "," }))
            {
                csvWriter.Context.RegisterClassMap<TransactionMap>();
                csvWriter.WriteRecords(transactions);
                await streamWriter.FlushAsync(); // Ensure all data is written to the MemoryStream
            }

            // No need to dispose memoryStream here as we need to read from it
            memoryStream.Position = 0; // Reset the position to the beginning of the stream

            // Now it's safe to read from memoryStream
            using var reader = new StreamReader(memoryStream);
            var csvContent = await reader.ReadToEndAsync();

            // Now that we've read the content, memoryStream can be disposed if necessary
            return csvContent;
        }
    }
}
