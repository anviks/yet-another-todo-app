using YetAnotherTodoApp.Core.Entities;
using YetAnotherTodoApp.Core.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using WebApp.Hubs;

namespace WebApp.ApiControllers;

[ApiController]
[Route("api/[controller]")]
public class TodoController(TodoService todoService, IHubContext<TodoHub> hub) : ControllerBase
{
    private async Task BroadcastUpdate()
    {
        await hub.Clients.All.SendAsync("TasksUpdated");
    }

    [HttpGet]
    public async Task<ActionResult<TodoTask[]>> GetAllTasks()
    {
        return Ok(await todoService.GetAllTasks());
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<TodoTask>> GetTask(int id)
    {
        TodoTask? task = await todoService.GetTask(id);
        if (task == null)
        {
            return NotFound();
        }
        return Ok(task);
    }

    [HttpPost]
    public async Task<IActionResult> CreateTask(TodoTask task)
    {
        await todoService.CreateTask(task);
        await BroadcastUpdate();
        return CreatedAtAction(nameof(GetAllTasks), new { id = task.Id }, task);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateTask(int id, TodoTask task)
    {
        if (id != task.Id) return BadRequest("Task ID mismatch.");
        if (!await todoService.Exists(id)) return NotFound();
        await todoService.UpdateTask(task);
        await BroadcastUpdate();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteTask(int id)
    {
        if (!await todoService.Exists(id)) return NotFound();
        await todoService.DeleteTask(id);
        await BroadcastUpdate();
        return NoContent();
    }

    [HttpPost("{id:int}/{actionName:regex(complete|uncomplete)}")]
    public async Task<IActionResult> MarkTaskCompleted(int id, string actionName)
    {
        var markCompleted = actionName == "complete";
        var result = await todoService.MarkTaskCompleted(id, markCompleted);
        if (!result) return NotFound();
        await BroadcastUpdate();
        return NoContent();
    }
}