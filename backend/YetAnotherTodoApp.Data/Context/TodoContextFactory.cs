using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace YetAnotherTodoApp.Data.Context;

public class TodoContextFactory : IDesignTimeDbContextFactory<TodoContext>
{
    public TodoContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<TodoContext>();

        var host = Environment.GetEnvironmentVariable("POSTGRES_HOST") ?? "localhost";
        var port = Environment.GetEnvironmentVariable("POSTGRES_PORT") ?? "5432";
        var db = Environment.GetEnvironmentVariable("POSTGRES_DB") ?? "todo_db";
        var user = Environment.GetEnvironmentVariable("POSTGRES_USER") ?? "todo_user";
        var password = Environment.GetEnvironmentVariable("POSTGRES_PASSWORD") ?? "password";

        var connectionString = $"Host={host};Port={port};Database={db};Username={user};Password={password}";

        optionsBuilder.UseNpgsql(connectionString);

        return new TodoContext(optionsBuilder.Options);
    }
}