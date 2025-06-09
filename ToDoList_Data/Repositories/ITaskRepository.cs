using ToDoList_Data.Models;

namespace ToDoList_Data.Repositories;

public interface ITaskRepository
{
    Task<ToDoItem> GetByIdAsync(int id);
    Task<List<ToDoItem>> GetAllAsync();
    Task AddAsync(ToDoItem task);
    Task  UpdateAsync(ToDoItem task);
    Task  DeleteAsync(int id);
}