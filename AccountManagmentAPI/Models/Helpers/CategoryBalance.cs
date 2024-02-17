namespace AccountManagmentAPI.Models.Helpers
{
    public class CategoryBalance
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public decimal TotalAmount { get; set; }
        public string Type { get; set; } // Income or Expense
    }
}
