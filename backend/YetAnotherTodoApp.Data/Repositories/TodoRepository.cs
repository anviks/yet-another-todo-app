using YetAnotherTodoApp.Core.Contracts.Repositories;
using YetAnotherTodoApp.Core.Entities;
using Microsoft.EntityFrameworkCore;
using YetAnotherTodoApp.Data.Context;

namespace YetAnotherTodoApp.Data.Repositories;

public class TodoRepository(TodoContext db) : ITodoRepository
{
    public async Task<List<TodoTask>> GetAllTasks()
    {
        return await db.Tasks.OrderBy(t => t.CreatedAt).ToListAsync();
    }

    public async Task<TodoTask?> GetTask(int id)
    {
        return await db.Tasks.FindAsync(id);
    }

    public async Task CreateTask(TodoTask task)
    {
        await db.Tasks.AddAsync(task);
        await db.SaveChangesAsync();
    }

    public async Task UpdateTask(TodoTask task)
    {
        db.Tasks.Update(task);
        await db.SaveChangesAsync();
    }

    public async Task DeleteTask(int id)
    {
        TodoTask? task = await db.Tasks.FindAsync(id);
        if (task == null) return;
        db.Tasks.Remove(task);
        await db.SaveChangesAsync();
    }

    public async Task<TodoTask?> MarkTaskCompleted(int id, bool completed)
    {
        var task = await db.Tasks.FindAsync(id);
        if (task == null) return null;

        task.CompletedAt = completed ? DateTime.UtcNow : null;
        await db.SaveChangesAsync();

        return task;
    }

    public async Task<bool> Exists(int id)
    {
        return await db.Tasks
            .AsNoTracking()
            .Where(t => t.Id == id)
            .AnyAsync();
    }
}