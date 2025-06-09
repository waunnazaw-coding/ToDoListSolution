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
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var result = await _taskService.GetByIdAsync(id);

                if (result.IsNotFoundError)
                    return NotFound(result.Message);

                if (result.IsError)
                    return BadRequest(result.Message);

                return Ok(result.Data);
            }
            catch (Exception ex)
            {
                // Log exception here as needed
                return StatusCode(StatusCodes.Status500InternalServerError, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var result = await _taskService.GetAllAsync();

                if (result.IsError)
                    return BadRequest(result.Message);

                return Ok(result.Data);
            }
            catch (Exception ex)
            {
                // Log exception here as needed
                return StatusCode(StatusCodes.Status500InternalServerError, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ToDoItem task)
        {
            try
            {
                if (task == null)
                    return BadRequest("Task object is null.");

                var result = await _taskService.AddAsync(task);

                if (result.IsValidationError)
                    return BadRequest(result.Message);

                if (result.IsError)
                    return BadRequest(result.Message);

                return CreatedAtAction(nameof(GetById), new { id = result.Data.TaskId }, result.Data);
            }
            catch (Exception ex)
            {
                // Log exception here as needed
                return StatusCode(StatusCodes.Status500InternalServerError, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] ToDoItem task)
        {
            try
            {
                if (task == null || id != task.TaskId)
                    return BadRequest("Task ID mismatch or task object is null.");

                var result = await _taskService.UpdateAsync(task);

                if (result.IsNotFoundError)
                    return NotFound(result.Message);

                if (result.IsValidationError)
                    return BadRequest(result.Message);

                if (result.IsError)
                    return BadRequest(result.Message);

                return Ok(result.Data);
            }
            catch (Exception ex)
            {
                // Log exception here as needed
                return StatusCode(StatusCodes.Status500InternalServerError, $"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var result = await _taskService.DeleteAsync(id);

                if (result.IsNotFoundError)
                    return NotFound(result.Message);

                if (result.IsError)
                    return BadRequest(result.Message);

                return NoContent();
            }
            catch (Exception ex)
            {
                // Log exception here as needed
                return StatusCode(StatusCodes.Status500InternalServerError, $"Internal server error: {ex.Message}");
            }
        }
    }
}
