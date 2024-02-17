using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace AccountManagmentAPI.Models
{
    public class Budget
    {

        public Guid BudgetId { get; set; }

        [Required]
        public string UserId { get; set; }

        public int? CategoryId { get; set; }

        [Required]
        public decimal Amount { get; set; }

        [Required]
        public DateTime Month { get; set; }

        public Category Category { get; set; }
    }
}
