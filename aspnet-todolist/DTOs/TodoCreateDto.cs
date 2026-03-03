namespace aspnet_todolist.DTOs
{
    public record TodoCreateDto(
        string Name,
        int CategoryId
    );
}
