using Financial.Domain.Enums;
using System;

namespace Financial.Api.Models
{
    public class TransactionViewModel
    {
        public string CategoryName { get; set; }

        public DateTime DateTime { get; set; }

        public string Description { get; set; }

        public TransactionType Type { get; set; }

        public double Value { get; set; }
    }
}