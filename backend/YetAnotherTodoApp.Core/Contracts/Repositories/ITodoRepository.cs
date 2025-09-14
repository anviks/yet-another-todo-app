using YetAnotherTodoApp.Core.Dtos;
using YetAnotherTodoApp.Core.Entities;

namespace YetAnotherTodoApp.Core.Contracts.Repositories;

public interface ITodoRepository
{
    public Task<List<TodoTask>> GetAllTasks(TodoTaskFilter? filter = null);
    public Task<TodoTask?> GetTask(int id);
    public Task CreateTask(TodoTask task);
    public Task UpdateTask(TodoTask task);
    public Task DeleteTask(int id);
    public Task<TodoTask?> MarkTaskCompleted(int id, bool completed);
    public Task<bool> Exists(int id);
}