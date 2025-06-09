using Microsoft.Extensions.Caching.Memory;
using ToDoList_Data.Models;
using ToDoList_Data.Repositories;
using ToDoList_Services.Models;

namespace ToDoList_Services.Services;

public class TaskService : ITaskService
{
    private readonly ITaskRepository _taskRepository;
    private readonly IMemoryCache _cache;

    public TaskService(ITaskRepository taskRepository, IMemoryCache cache)
    {
        _taskRepository = taskRepository;
        _cache = cache;
    }

    public async Task<Result<ToDoItem>> GetByIdAsync(int id)
    {
        string cacheKey = $"Task_{id}";

        if (_cache.TryGetValue(cacheKey, out ToDoItem cachedTask))
        {
            return Result<ToDoItem>.Success(cachedTask);
        }

        var task = await _taskRepository.GetByIdAsync(id);
        if (task == null)
            return Result<ToDoItem>.NotFoundError($"Task with ID {id} not found.");

        _cache.Set(cacheKey, task, TimeSpan.FromMinutes(5)); // Cache for 5 minutes

        return Result<ToDoItem>.Success(task);
    }

    public async Task<Result<List<ToDoItem>>> GetAllAsync()
    {
        string cacheKey = "AllTasks";

        if (_cache.TryGetValue(cacheKey, out List<ToDoItem> cachedTasks))
        {
            return Result<List<ToDoItem>>.Success(cachedTasks);
        }

        var tasks = await _taskRepository.GetAllAsync();

        _cache.Set(cacheKey, tasks, TimeSpan.FromMinutes(5)); // Cache for 5 minutes

        return Result<List<ToDoItem>>.Success(tasks);
    }

    public async Task<Result<ToDoItem>> AddAsync(ToDoItem task)
    {
        if (task == null)
            return Result<ToDoItem>.ValidationError("Task cannot be null.");

        await _taskRepository.AddAsync(task);

        // Invalidate cache
        _cache.Remove("AllTasks");

        return Result<ToDoItem>.Success(task, "Task added successfully.");
    }

    public async Task<Result<ToDoItem>> UpdateAsync(ToDoItem task)
    {
        if (task == null || task.TaskId <= 0)
            return Result<ToDoItem>.ValidationError("Invalid task data.");

        var existingTask = await _taskRepository.GetByIdAsync(task.TaskId);
        if (existingTask == null)
            return Result<ToDoItem>.NotFoundError($"Task with ID {task.TaskId} not found.");

        await _taskRepository.UpdateAsync(task);

        // Invalidate cache
        _cache.Remove("AllTasks");
        _cache.Remove($"Task_{task.TaskId}");

        return Result<ToDoItem>.Success(task, "Task updated successfully.");
    }

    public async Task<Result<bool>> DeleteAsync(int id)
    {
        var existingTask = await _taskRepository.GetByIdAsync(id);
        if (existingTask == null)
            return Result<bool>.NotFoundError($"Task with ID {id} not found.");

        await _taskRepository.DeleteAsync(id);

        // Invalidate cache
        _cache.Remove("AllTasks");
        _cache.Remove($"Task_{id}");

        return Result<bool>.Success(true, "Task deleted successfully.");
    }
}
