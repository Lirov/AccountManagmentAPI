using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace AccountManagmentAPI.Models
{
    public class Budget
    {

        public Guid BudgetId { get; set; }

        [Required]
        public string UserId { get; set; } // Link to ASP.NET Core Identity User ID

        public int? CategoryId { get; set; } // Optional, for category-specific budgets

        [Required]
        public decimal Amount { get; set; }

        [Required]
        public DateTime Month { get; set; } // You might want to store just the year and month

        public Category Category { get; set; } // Allows null for overall budgets
    }
}
