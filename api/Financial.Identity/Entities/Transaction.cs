using Financial.Domain.Enums;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Financial.Domain.Entities
{
    public class Transaction
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey(nameof(Wallet))]
        public int WalletId { get; set; }

        public Wallet Wallet { get; set; }

        [ForeignKey(nameof(Category))]
        public int CategoryId { get; set; }

        public TransactionCategory Category { get; set; }

        public DateTime DateTime { get; set; }

        public TransactionType Type { get; set; }

        public double Value { get; set; }

        public string Description { get; set; }
    }
}