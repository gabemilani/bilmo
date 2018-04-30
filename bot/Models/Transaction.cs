using System;

namespace Financial.Bot.Models
{
    [Serializable]
    public class Transaction
    {
        public int? CategoryId { get; set; }

        public string CategoryName { get; set; }

        public DateTime DateTime { get; set; }

        public TransactionType Type { get; set; }

        public double Value { get; set; }

        public string Description { get; set; }
    }
}