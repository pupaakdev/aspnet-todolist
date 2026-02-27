using System.ComponentModel.DataAnnotations;
using System.Drawing;

namespace aspnet_todolist.Models
{
    public class Category
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string? Name { get; set; }

        [Required]
        [RegularExpression(@"^#([A-Fa-f0-9]{6}|[A-Fa-f0-9]{3})$", ErrorMessage = "Color must be a valid HEX code")]
        public string? Color { get; set; }
    }
}
