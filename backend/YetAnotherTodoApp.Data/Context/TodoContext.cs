using YetAnotherTodoApp.Core.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace YetAnotherTodoApp.Data.Context;

public class TodoContext(DbContextOptions<TodoContext> options) : IdentityDbContext(options)
{
    public DbSet<TodoTask> Tasks { get; set; }
}
