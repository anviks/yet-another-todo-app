namespace YetAnotherTodoApp.Core.Dtos;

public class TodoTaskFilter
{
    public bool? Completed { get; set; }
    public DateTime? DueAfter { get; set; }
    public DateTime? DueBefore { get; set; }
    // Search query for description
    public string? Q { get; set; }
}