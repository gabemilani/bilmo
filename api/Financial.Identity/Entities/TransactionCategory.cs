using Financial.Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Financial.Domain.Entities
{
    [Table("TransactionCategories")]
    public class TransactionCategory
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public TransactionType Type { get; set; }
    }
}