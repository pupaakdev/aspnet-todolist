namespace aspnet_todolist.DTOs
{
    public record TodoResponseDto(
        int Id,
        string Name,
        bool IsComplete,
        CategoryResponseDto? Category
    );
}
