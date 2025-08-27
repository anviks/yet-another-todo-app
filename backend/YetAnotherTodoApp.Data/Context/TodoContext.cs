using YetAnotherTodoApp.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace YetAnotherTodoApp.Data.Context;

public class TodoContext(DbContextOptions<TodoContext> options) : DbContext(options)
{
    public DbSet<TodoTask> Tasks { get; set; }
}
