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

    public async Task<bool> MarkTaskCompleted(int id)
    {
        TodoTask? task = await todoRepository.GetTask(id);
        if (task == null || task.IsCompleted) return false;

        task.MarkCompleted(true);
        await todoRepository.UpdateTask(task);

        return true;
    }
}