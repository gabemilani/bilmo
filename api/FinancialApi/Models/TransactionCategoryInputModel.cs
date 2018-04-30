using Financial.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Financial.Api.Models
{
    public class TransactionCategoryInputModel
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public TransactionType Type { get; set; }
    }
}