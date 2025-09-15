using YetAnotherTodoApp.Core.Contracts.Repositories;
using YetAnotherTodoApp.Core.Entities;
using Microsoft.EntityFrameworkCore;
using YetAnotherTodoApp.Core.Dtos;
using YetAnotherTodoApp.Data.Context;

namespace YetAnotherTodoApp.Data.Repositories;

public class TodoRepository(TodoContext db) : ITodoRepository
{
    public async Task<PaginatedResult<TodoTask>> GetAllTasks(TodoTaskFilter filter)
    {
        var query = db.Tasks.AsNoTracking().AsQueryable();

        if (filter.Completed.HasValue)
            query = query.Where(t => (t.CompletedAt != null) == filter.Completed);
        if (filter.DueAfter.HasValue)
            query = query.Where(t => t.DueDate.HasValue && t.DueDate.Value >= filter.DueAfter.Value);
        if (filter.DueBefore.HasValue)
            query = query.Where(t => t.DueDate.HasValue && t.DueDate.Value <= filter.DueBefore.Value);
        if (!string.IsNullOrEmpty(filter.Q))
            query = query.Where(t => t.Description.ToLower().Contains(filter.Q.ToLower()));

        var items = await query
            .OrderByDescending(t => t.CreatedAt)
            .Skip((filter.Page - 1) * filter.PageSize)
            .Take(filter.PageSize + 1)
            .ToListAsync();

        var hasNextPage = items.Count > filter.PageSize;
        if (hasNextPage)
            items.RemoveAt(items.Count - 1);

        return new PaginatedResult<TodoTask>
        {
            Items = items,
            HasNextPage = hasNextPage,
        };
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

    public async Task SaveChanges()
    {
        await db.SaveChangesAsync();
    }
}