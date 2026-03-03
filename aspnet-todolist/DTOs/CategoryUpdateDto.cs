using System.ComponentModel.DataAnnotations;

namespace aspnet_todolist.DTOs
{
    public record CategoryUpdateDto(
        [Required]
        [MaxLength(50)]
        string Name,

        [Required]
        [RegularExpression(@"^#([A-Fa-f0-9]{6}|[A-Fa-f0-9]{3})$", ErrorMessage = "Color must be a valid HEX code")]
        string Color
    );
}
