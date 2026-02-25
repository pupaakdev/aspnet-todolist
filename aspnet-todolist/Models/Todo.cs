using System.ComponentModel.DataAnnotations;

namespace aspnet_todolist.Models
{
    public class Todo
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string? Name { get; set; }

        public bool IsComplete { get; set; }

        public bool IsDeleted { get; set; }
    }
}
