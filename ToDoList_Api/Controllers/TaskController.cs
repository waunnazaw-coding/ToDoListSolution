using Microsoft.AspNetCore.Mvc;
using ToDoList_Data.Models;
using ToDoList_Services.Services;
namespace ToDoList_Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TaskController : ControllerBase
    {
        private readonly ITaskService _taskService;

        public TaskController(ITaskService taskService)
        {
            _taskService = taskService;
        }

        [HttpGet("{id}")]
        public async Task<IResult> GetById(int id)
        {
            var result = await _taskService.GetByIdAsync(id);
             return result.Execute();
        }

        [HttpGet]
        public async Task<IResult> GetAll()
        {
            var result = await _taskService.GetAllAsync();
            return result.Execute();
        }

        [HttpPost]
        public async Task<IResult> Create([FromBody] ToDoItem task)
        {
            if (task == null)
                return Results.BadRequest("Task object is null.");

            var result = await _taskService.AddAsync(task);
            return result.Execute();
        }

        [HttpPut("{id}")]
        public async Task<IResult> Update(int id, [FromBody] ToDoItem task)
        {
            if (task == null || id != task.TaskId)
                return Results.BadRequest("Task ID mismatch or task object is null.");

            var result = await _taskService.UpdateAsync(task);
            return result.Execute();
        }

        [HttpDelete("{id}")]
        public async Task<IResult> Delete(int id)
        {
            var result = await _taskService.DeleteAsync(id);
            return result.Execute();
        }

    }
}
