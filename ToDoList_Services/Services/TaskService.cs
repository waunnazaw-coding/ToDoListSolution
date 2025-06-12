using Microsoft.Extensions.Caching.Memory;
using ToDoList_Data.Models;
using ToDoList_Data.Repositories;
using ToDoList_Services.Models;
using System.Threading;

namespace ToDoList_Services.Services;

public class TaskService : ITaskService
{
    private readonly ITaskRepository _taskRepository;
    private readonly IMemoryCache _cache;
    private static readonly SemaphoreSlim _allTasksLock = new(1, 1);
    private static readonly SemaphoreSlim _taskLock = new(1, 1);


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
            return Result<ToDoItem>.Success(cachedTask , "Task Retrieved Successfully.");
        }

        await _taskLock.WaitAsync();
        try
        {
            // Double-check after acquiring lock
            if (_cache.TryGetValue(cacheKey, out cachedTask))
            {
                return Result<ToDoItem>.Success(cachedTask , "Task Retrieved Successfully.");
            }

            var task = await _taskRepository.GetByIdAsync(id);
            if (task == null)
                return Result<ToDoItem>.NotFoundError($"Task with ID {id} not found.");

            var cacheOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromSeconds(30))
                .SetAbsoluteExpiration(TimeSpan.FromSeconds(300))
                .SetPriority(CacheItemPriority.Normal);

            _cache.Set(cacheKey, task, cacheOptions);
            return Result<ToDoItem>.Success(task , "Task Retrieved Successfully.");
        }
        finally
        {
            _taskLock.Release();
        }
    }

    public async Task<Result<List<ToDoItem>>> GetAllAsync()
    {
        string cacheKey = "AllTasks";

        if (_cache.TryGetValue(cacheKey, out List<ToDoItem> cachedTasks))
        {
            return Result<List<ToDoItem>>.Success(cachedTasks , "Tasks Retrieved Successfully.");
        }

        await _allTasksLock.WaitAsync();
        try
        {
            // Double-check after acquiring lock
            if (_cache.TryGetValue(cacheKey, out cachedTasks))
            {
                return Result<List<ToDoItem>>.Success(cachedTasks, "Tasks Retrieved Successfully.");
            }

            var tasks = await _taskRepository.GetAllAsync();
            var cacheOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromSeconds(30))
                .SetAbsoluteExpiration(TimeSpan.FromSeconds(300))
                .SetPriority(CacheItemPriority.NeverRemove)
                .SetSize(2048);

            _cache.Set(cacheKey, tasks, cacheOptions);

            return Result<List<ToDoItem>>.Success(tasks , "Tasks Retrieved Successfully.");
        }
        finally
        {
            _allTasksLock.Release();
        }
    }

    public async Task<Result<ToDoItem>> AddAsync(ToDoItem task)
    {
        if (task == null)
            return Result<ToDoItem>.ValidationError("Task cannot be null.");

        await _taskRepository.AddAsync(task);

        // Invalidate related cache
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

        // Invalidate related cache
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

        // Invalidate related cache
        _cache.Remove("AllTasks");
        _cache.Remove($"Task_{id}");

        return Result<bool>.Success(true, "Task deleted successfully.");
    }
}
