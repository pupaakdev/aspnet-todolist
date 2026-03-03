using System.ComponentModel.DataAnnotations;

namespace aspnet_todolist.DTOs
{
    public record TodoCreateDto(
        [Required]
        [MaxLength(100)]
        string Name,

        int CategoryId
    );
}
