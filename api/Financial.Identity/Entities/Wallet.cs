using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Financial.Domain.Entities
{
    public class Wallet
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey(nameof(User))]
        public int UserId { get; set; }

        public User User { get; set; }

        [Required]
        public string Name { get; set; }

        public double CurrentBalance { get; set; }
    }
}