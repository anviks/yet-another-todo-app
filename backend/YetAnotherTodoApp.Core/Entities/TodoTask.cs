using System.ComponentModel.DataAnnotations;

namespace YetAnotherTodoApp.Core.Entities;

public class TodoTask
{
    public int Id { get; set; }
    [MaxLength(100)]
    public string Title { get; set; } = default!;
    [MaxLength(2000)]
    public string Description { get; set; } = default!;
    public DateTime? DueDate { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public DateTime? CompletedAt { get; set; }

    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

    // TODO: Add author
}