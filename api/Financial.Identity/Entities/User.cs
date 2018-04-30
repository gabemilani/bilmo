using System.ComponentModel.DataAnnotations;

namespace Financial.Domain.Entities
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string Email { get; set; }
    }
}