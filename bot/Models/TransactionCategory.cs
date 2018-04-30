using System;

namespace Financial.Bot.Models
{
    [Serializable]
    public class TransactionCategory
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public TransactionType Type { get; set; }

        public override string ToString() => Name;
    }
}