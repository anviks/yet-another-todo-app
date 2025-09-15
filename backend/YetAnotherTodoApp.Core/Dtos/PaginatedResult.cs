namespace YetAnotherTodoApp.Core.Dtos;

public class PaginatedResult<T>
{
    public List<T> Items { get; set; } = default!;
    public int TotalCount { get; set; }
}