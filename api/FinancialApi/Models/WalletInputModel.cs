using System.ComponentModel.DataAnnotations;

namespace Financial.Api.Models
{
    public class WalletInputModel
    {
        [Required]
        public string Name { get; set; }

        public double CurrentBalance { get; set; }
    }
}