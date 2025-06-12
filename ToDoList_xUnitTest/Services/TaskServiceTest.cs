//using System;
//using System.Collections.Generic;
//using System.Threading.Tasks;
//using Microsoft.Extensions.Caching.Memory;
//using Moq;
//using ToDoList_Data.Models;
//using ToDoList_Data.Repositories;
//using ToDoList_Services.Services;
//using Xunit;

//namespace ToDoList_xUnitTest.Services
//{
//    public class TaskServiceTest
//    {
//        private readonly TaskService _taskService;
//        private readonly Mock<ITaskRepository> _taskRepo;
//        private readonly Mock<IMemoryCache> _memoryCache;
//        private readonly Mock<ICacheEntry> _cacheEntry;

//        public TaskServiceTest()
//        {
//            _taskRepo = new Mock<ITaskRepository>();
//            _memoryCache = new Mock<IMemoryCache>();
//            _cacheEntry = new Mock<ICacheEntry>();

//            // Setup cache entry mock to support caching operations
//            _cacheEntry.SetupAllProperties();
//            _cacheEntry.Setup(m => m.Value).Returns(null);
//            _memoryCache.Setup(m => m.CreateEntry(It.IsAny<object>())).Returns(_cacheEntry.Object);

//            _taskService = new TaskService(_taskRepo.Object, _memoryCache.Object);
//        }

//        [Fact]
//        public async Task GetByIdAsync_ReturnsCachedTask_WhenCacheHit()
//        {
//            var task = new ToDoItem { TaskId = 1, Title = "Test Task" };
//            _memoryCache.Setup(c => c.TryGetValue("Task_1", out task)).Returns(true);

//            var result = await _taskService.GetByIdAsync(1);

//            Assert.True(result.IsSuccess);
//            Assert.Equal(task, result.Data);
//            _taskRepo.Verify(r => r.GetByIdAsync(It.IsAny<int>()), Times.Never);
//        }

//        [Fact]
//        public async Task GetByIdAsync_ReturnsTaskAndCaches_WhenCacheMiss()
//        {
//            ToDoItem cached = null;
//            var task = new ToDoItem { TaskId = 2, Title = "New Task" };

//            // Setup cache miss
//            _memoryCache.Setup(c => c.TryGetValue("Task_2", out cached)).Returns(false);
//            _taskRepo.Setup(r => r.GetByIdAsync(2)).ReturnsAsync(task);

//            var result = await _taskService.GetByIdAsync(2);

//            Assert.True(result.IsSuccess);
//            Assert.Equal(task, result.Data);
//            _memoryCache.Verify(m => m.CreateEntry("Task_2"), Times.Once);
//        }

//        [Fact]
//        public async Task GetByIdAsync_ReturnsNotFound_WhenTaskDoesNotExist()
//        {
//            ToDoItem cached = null;
//            _memoryCache.Setup(c => c.TryGetValue("Task_99", out cached)).Returns(false);
//            _taskRepo.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((ToDoItem)null);

//            var result = await _taskService.GetByIdAsync(99);

//            Assert.False(result.IsSuccess);
//            Assert.Contains("not found", result.Message);
//        }

//        [Fact]
//        public async Task GetAllAsync_ReturnsCachedTasks_WhenCacheHit()
//        {
//            var tasks = new List<ToDoItem> { new ToDoItem { TaskId = 1, Title = "Task1" } };
//            _memoryCache.Setup(c => c.TryGetValue("AllTasks", out tasks)).Returns(true);

//            var result = await _taskService.GetAllAsync();

//            Assert.True(result.IsSuccess);
//            Assert.Equal(tasks, result.Data);
//            _taskRepo.Verify(r => r.GetAllAsync(), Times.Never);
//        }

//        [Fact]
//        public async Task GetAllAsync_ReturnsTasksAndCaches_WhenCacheMiss()
//        {
//            List<ToDoItem> cached = null;
//            var tasks = new List<ToDoItem> { new ToDoItem { TaskId = 3, Title = "Task3" } };

//            _memoryCache.Setup(c => c.TryGetValue("AllTasks", out cached)).Returns(false);
//            _taskRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(tasks);

//            var result = await _taskService.GetAllAsync();

//            Assert.True(result.IsSuccess);
//            Assert.Equal(tasks, result.Data);
//            _memoryCache.Verify(m => m.CreateEntry("AllTasks"), Times.Once);
//        }

//        [Fact]
//        public async Task AddAsync_ReturnsValidationError_WhenTaskIsNull()
//        {
//            var result = await _taskService.AddAsync(null);

//            Assert.False(result.IsSuccess);
//            Assert.Contains("cannot be null", result.Message);
//        }

//        [Fact]
//        public async Task AddAsync_AddsTaskAndInvalidatesCache()
//        {
//            var task = new ToDoItem { TaskId = 4, Title = "New Task" };
//            _taskRepo.Setup(r => r.AddAsync(task)).Returns(Task.CompletedTask);

//            var result = await _taskService.AddAsync(task);

//            Assert.True(result.IsSuccess);
//            _memoryCache.Verify(c => c.Remove("AllTasks"), Times.Once);
//            _taskRepo.Verify(r => r.AddAsync(task), Times.Once);
//        }

//        [Fact]
//        public async Task UpdateAsync_ReturnsValidationError_WhenTaskIsInvalid()
//        {
//            var invalidTask = new ToDoItem { TaskId = 0 };

//            var result = await _taskService.UpdateAsync(invalidTask);

//            Assert.False(result.IsSuccess);
//            Assert.Contains("Invalid task data", result.Message);
//        }

//        [Fact]
//        public async Task UpdateAsync_ReturnsNotFound_WhenTaskDoesNotExist()
//        {
//            var task = new ToDoItem { TaskId = 5, Title = "Update Task" };
//            _taskRepo.Setup(r => r.GetByIdAsync(5)).ReturnsAsync((ToDoItem)null);

//            var result = await _taskService.UpdateAsync(task);

//            Assert.False(result.IsSuccess);
//            Assert.Contains("not found", result.Message);
//        }

//        [Fact]
//        public async Task UpdateAsync_UpdatesTaskAndInvalidatesCache()
//        {
//            var task = new ToDoItem { TaskId = 6, Title = "Update Task" };
//            _taskRepo.Setup(r => r.GetByIdAsync(6)).ReturnsAsync(task);
//            _taskRepo.Setup(r => r.UpdateAsync(task)).Returns(Task.CompletedTask);

//            var result = await _taskService.UpdateAsync(task);

//            Assert.True(result.IsSuccess);
//            _memoryCache.Verify(c => c.Remove("AllTasks"), Times.Once);
//            _memoryCache.Verify(c => c.Remove("Task_6"), Times.Once);
//            _taskRepo.Verify(r => r.UpdateAsync(task), Times.Once);
//        }

//        [Fact]
//        public async Task DeleteAsync_ReturnsNotFound_WhenTaskDoesNotExist()
//        {
//            _taskRepo.Setup(r => r.GetByIdAsync(7)).ReturnsAsync((ToDoItem)null);

//            var result = await _taskService.DeleteAsync(7);

//            Assert.False(result.IsSuccess);
//            Assert.Contains("not found", result.Message);
//        }

//        [Fact]
//        public async Task DeleteAsync_DeletesTaskAndInvalidatesCache()
//        {
//            var task = new ToDoItem { TaskId = 8, Title = "Delete Task" };
//            _taskRepo.Setup(r => r.GetByIdAsync(8)).ReturnsAsync(task);
//            _taskRepo.Setup(r => r.DeleteAsync(8)).Returns(Task.CompletedTask);

//            var result = await _taskService.DeleteAsync(8);

//            Assert.True(result.IsSuccess);
//            _memoryCache.Verify(c => c.Remove("AllTasks"), Times.Once);
//            _memoryCache.Verify(c => c.Remove("Task_8"), Times.Once);
//            _taskRepo.Verify(r => r.DeleteAsync(8), Times.Once);
//        }
//    }
//}
