using AccountManagmentAPI.Models;
using AccountManagmentAPI.Models.Helpers;
using AccountManagmentAPI.Repositories.Interfaces;
using AccountManagmentAPI.Repositories.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text;

namespace AccountManagmentAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TransactionController : ControllerBase
    {
        private readonly ITransactionService _transactionService;
        private readonly IBudgetService _budgetService;

        public TransactionController(ITransactionService transactionService, IBudgetService budgetService)
        {
            _transactionService = transactionService;
            _budgetService = budgetService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Transaction>>> GetTransactions()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var transactions = await _transactionService.GetAllTransactionsAsync(userId);
            return Ok(transactions);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Transaction>> GetTransaction(Guid id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var transaction = await _transactionService.GetTransactionByIdAsync(id, userId);

            if (transaction == null)
            {
                return NotFound();
            }

            return transaction;
        }

        [HttpGet("balance")]
        public async Task<ActionResult<decimal>> GetGeneralBalance()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var balance = await _transactionService.GetGeneralBalanceAsync(userId);
            return Ok(balance);
        }

        [HttpGet("balanceByCategory")]
        public async Task<ActionResult<IEnumerable<CategoryBalance>>> GetBalanceByCategoryAsync()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var categoryBalances = await _transactionService.GetBalanceByCategoryAsync(userId);
            return Ok(categoryBalances);
        }

        [HttpPost]
        public async Task<ActionResult<Transaction>> CreateTransaction(Transaction transaction)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            transaction.UserId = userId;

            bool withinBudget = await _budgetService.IsTransactionWithinBudgetAsync(userId, transaction.Amount, transaction.CategoryId);
            if (!withinBudget)
            {
                return BadRequest("Transaction exceeds the budget.");
            }

            try
            {
                var newTransaction = await _transactionService.CreateTransactionAsync(transaction, userId);
                return CreatedAtAction(nameof(GetTransaction), new { id = newTransaction.TransactionId }, newTransaction);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while creating the transaction.");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutTransaction(Guid id, Transaction transaction)
        {
            if (id != transaction.TransactionId)
            {
                return BadRequest();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            await _transactionService.UpdateTransactionAsync(transaction, userId);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTransaction(Guid id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            await _transactionService.DeleteTransactionAsync(id, userId);

            return NoContent();
        }

        [HttpGet("export")]
        public async Task<IActionResult> ExportTransactions(DateTime? startDate, DateTime? endDate)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var transactions = await _transactionService.GetTransactionsForExportAsync(userId, startDate, endDate);
            var csvContent = await _transactionService.GetGenerateCsvContentAsync(transactions);

            return File(Encoding.UTF8.GetBytes(csvContent), "text/csv", "transactions.csv");
        }
    }
}
