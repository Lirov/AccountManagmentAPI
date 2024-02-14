using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace AccountManagmentAPI.Models
{
    public class Category
    {

        public Guid CategoryId { get; set; }

        [Required]
        public string Name { get; set; }

        public string Type { get; set; }

        public ICollection<Transaction> Transactions { get; set; }
    }
}
