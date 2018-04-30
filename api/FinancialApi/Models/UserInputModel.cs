using System.ComponentModel.DataAnnotations;

namespace Financial.Api.Models
{
    public class UserInputModel
    {
        [Required]
        public string Name { get; set; }

        public string Email { get; set; }
    }
}