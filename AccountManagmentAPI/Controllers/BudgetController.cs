using AccountManagmentAPI.Models;
using AccountManagmentAPI.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AccountManagmentAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BudgetController : ControllerBase
    {
        private readonly IBudgetService _budgetService;

        public BudgetController(IBudgetService budgetService)
        {
            _budgetService = budgetService;
        }


        [HttpGet]
        public async Task<ActionResult<IEnumerable<Budget>>> GetAllBudgets()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var budgets = await _budgetService.GetAllBudgetsAsync(userId);
            return Ok(budgets);
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<Budget>> GetBudget(Guid id, [FromQuery] int categoryId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var budget = await _budgetService.GetBudgetByIdAsync(id, userId, categoryId);

            if (budget == null)
            {
                return NotFound();
            }

            return budget;
        }


        [HttpPost]
        public async Task<ActionResult<Budget>> CreateBudget(Budget budget)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized();
            }

            budget.UserId = userId;
            var createdBudget = await _budgetService.CreateBudgetAsync(budget, userId);
            return CreatedAtAction(nameof(GetBudget), new { id = createdBudget.BudgetId }, createdBudget);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBudget(Guid id, Budget budget)
        {
            if (id != budget.BudgetId)
            {
                return BadRequest();
            }

            //var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null || budget.UserId != userId)
            {
                // Prevent updating a budget that does not belong to the current user
                return Unauthorized();
            }

            try
            {
                await _budgetService.UpdateBudgetAsync(budget, userId);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }

            return NoContent();
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBudget(Guid id)
        {

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            await _budgetService.DeleteBudgetAsync(id, userId);

            return NoContent();
        }
    }
}
