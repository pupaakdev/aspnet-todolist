namespace aspnet_todolist.DTOs
{
    public record TodoUpdateDto(
        string Name,
        bool IsComplete,
        int? CategoryId
    );
}
