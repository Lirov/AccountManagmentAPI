using CsvHelper.Configuration;
using System.Globalization;

namespace AccountManagmentAPI.Models.Helpers
{
    public class TransactionMap : ClassMap<Transaction>
    {
        public TransactionMap()
        {
            AutoMap(CultureInfo.InvariantCulture);

            Map(m => m.TransactionId);
            Map(m => m.UserId);
            Map(m => m.Amount);
            Map(m => m.Date);
            Map(m => m.Description);

            Map(m => m.Category.Name).Name("CategoryName").Convert(row => row.Value.Category?.Name ?? string.Empty);
            Map(m => m.Category.Type).Name("CategoryType").Convert(row => row.Value.Category?.Type ?? string.Empty);
        }
    }
}
