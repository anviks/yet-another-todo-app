using YetAnotherTodoApp.Core.Contracts.Repositories;
using YetAnotherTodoApp.Core.Entities;

namespace YetAnotherTodoApp.Core.Services;

public class TodoService(ITodoRepository todoRepository)
{
    public async Task<List<TodoTask>> GetAllTasks()
    {
        return await todoRepository.GetAllTasks();
    }

    public async Task<TodoTask?> GetTask(int id)
    {
        return await todoRepository.GetTask(id);
    }

    public async Task CreateTask(TodoTask task)
    {
        await todoRepository.CreateTask(task);
    }

    public async Task UpdateTask(TodoTask task)
    {
        await todoRepository.UpdateTask(task);
    }

    public async Task DeleteTask(int id)
    {
        await todoRepository.DeleteTask(id);
    }

    public async Task<TodoTask?> MarkTaskCompleted(int id, bool completed = true)
    {
        return await todoRepository.MarkTaskCompleted(id, completed);
    }

    public async Task<bool> Exists(int id)
    {
        return await todoRepository.Exists(id);
    }
}
