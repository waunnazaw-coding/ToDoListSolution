using ToDoList_Data.Models;
using ToDoList_Data.Repositories;
using ToDoList_Services.Models;

namespace ToDoList_Services.Services;

public class TaskService : ITaskService
    {
        private readonly ITaskRepository _taskRepository;

        public TaskService(ITaskRepository taskRepository)
        {
            _taskRepository = taskRepository;
        }

        public async Task<Result<ToDoItem>> GetByIdAsync(int id)
        {
            var task = await _taskRepository.GetByIdAsync(id);
            if (task == null)
                return Result<ToDoItem>.NotFoundError($"Task with ID {id} not found.");

            return Result<ToDoItem>.Success(task);
        }

        public async Task<Result<List<ToDoItem>>> GetAllAsync()
        {
            var tasks = await _taskRepository.GetAllAsync();
            return Result<List<ToDoItem>>.Success(tasks);
        }

        public async Task<Result<ToDoItem>> AddAsync(ToDoItem task)
        {
            if (task == null)
                return Result<ToDoItem>.ValidationError("Task cannot be null.");

            await _taskRepository.AddAsync(task);
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
            return Result<ToDoItem>.Success(task, "Task updated successfully.");
        }

        public async Task<Result<bool>> DeleteAsync(int id)
        {
            var existingTask = await _taskRepository.GetByIdAsync(id);
            if (existingTask == null)
                return Result<bool>.NotFoundError($"Task with ID {id} not found.");

            await _taskRepository.DeleteAsync(id);
            return Result<bool>.Success(true, "Task deleted successfully.");
        }
    }