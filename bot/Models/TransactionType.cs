using System.ComponentModel;

namespace Financial.Bot.Models
{
    public enum TransactionType
    {
        [Description("N/A")]
        Unknown,
        [Description("Despesa")]
        Expense,
        [Description("Renda")]
        Income
    }
}