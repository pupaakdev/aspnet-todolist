using System.ComponentModel.DataAnnotations;

namespace aspnet_todolist.DTOs
{
    public record TodoUpdateDto(
        [Required]
        [MaxLength(100)]
        string Name,

        bool IsComplete,
        
        int? CategoryId
    );
}
