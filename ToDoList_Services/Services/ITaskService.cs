using ToDoList_Data.Models;
using ToDoList_Services.Models;
namespace ToDoList_Services.Services;

public interface ITaskService
{
    Task<Result<ToDoItem>> GetByIdAsync(int id);
    Task<Result<List<ToDoItem>>> GetAllAsync();
    Task<Result<ToDoItem>> AddAsync(ToDoItem task);
    Task<Result<ToDoItem>> UpdateAsync(ToDoItem task);
    Task<Result<bool>> DeleteAsync(int id);
}