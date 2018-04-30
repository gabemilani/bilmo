using Financial.Domain.Enums;

namespace Financial.Api.Models
{
    public class TransactionInputModel
    {
        public int? CategoryId { get; set; }

        public string CategoryName { get; set; }

        public TransactionType Type { get; set; }

        public double Value { get; set; }

        public string Description { get; set; }
    }
}