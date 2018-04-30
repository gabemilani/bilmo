using System;

namespace Financial.Bot.Models
{
    [Serializable]
    public class Wallet
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public double CurrentBalance { get; set; }
    }
}