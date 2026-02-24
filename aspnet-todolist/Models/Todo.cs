using System.ComponentModel.DataAnnotations;

namespace aspnet_todolist.Models
{
    public class Todo
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Field Name is required")]
        [MaxLength(100, ErrorMessage = "Name can't exceed 100 characters")]
        public string? Name { get; set; }

        public bool IsComplete { get; set; }
    }
}
