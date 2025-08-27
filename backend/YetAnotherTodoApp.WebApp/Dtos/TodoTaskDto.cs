using System.ComponentModel.DataAnnotations;

namespace WebApp.Dtos;

public class TodoTaskDto
{
    [Required]
    [MaxLength(100)]
    public string Title { get; set; } = default!;

    [MaxLength(2000)]
    public string Description { get; set; } = "";

    public DateTime? DueDate { get; set; }

    [Required]
    [Range(-90, 90)]
    public double? Latitude { get; set; }

    [Required]
    [Range(-180, 180)]
    public double? Longitude { get; set; }
}
