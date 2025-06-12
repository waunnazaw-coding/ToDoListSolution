using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using ToDoList_Data.Models;
using ToDoList_Data.Repositories;
using Xunit;

namespace ToDoList_Data.Tests.Repositories
{
    public class TaskRepositoryTests : IDisposable
    {
        private readonly AppDbContext _context;
        private readonly TaskRepository _repository;

        public TaskRepositoryTests()
        {
            // Setup in-memory database for testing
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Unique DB per test class instance
                .Options;

            _context = new AppDbContext(options);
            _context.Database.EnsureCreated();

            // Seed initial data
            _context.ToDoItems.Add(new ToDoItem { TaskId = 1, Title = "Test Task 1" });
            _context.SaveChanges();

            _repository = new TaskRepository(_context);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnTask_WhenTaskExists()
        {
            // Act
            var task = await _repository.GetByIdAsync(1);

            // Assert
            Assert.NotNull(task);
            Assert.Equal(1, task.TaskId);
            Assert.Equal("Test Task 1", task.Title);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnNull_WhenTaskDoesNotExist()
        {
            // Act
            var task = await _repository.GetByIdAsync(999);

            // Assert
            Assert.Null(task);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllTasks()
        {
            // Act
            var tasks = await _repository.GetAllAsync();

            // Assert
            Assert.NotNull(tasks);
            Assert.Single(tasks);
        }

        [Fact]
        public async Task AddAsync_ShouldAddTask()
        {
            // Arrange
            var newTask = new ToDoItem { TaskId = 2, Title = "New Task" };

            // Act
            await _repository.AddAsync(newTask);
            var task = await _repository.GetByIdAsync(2);

            // Assert
            Assert.NotNull(task);
            Assert.Equal("New Task", task.Title);
        }

        [Fact]
        public async Task UpdateAsync_ShouldModifyExistingTask()
        {
            // Arrange
            var task = await _repository.GetByIdAsync(1);
            task.Title = "Updated Task";

            // Act
            await _repository.UpdateAsync(task);
            var updatedTask = await _repository.GetByIdAsync(1);

            // Assert
            Assert.NotNull(updatedTask);
            Assert.Equal("Updated Task", updatedTask.Title);
        }

        [Fact]
        public async Task DeleteAsync_ShouldRemoveTask()
        {
            // Act
            await _repository.DeleteAsync(1);
            var deletedTask = await _repository.GetByIdAsync(1);

            // Assert
            Assert.Null(deletedTask);
        }

        [Fact]
        public async Task DeleteAsync_ShouldThrowArgumentNullException_WhenTaskDoesNotExist()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => _repository.DeleteAsync(999));
        }

        public void Dispose()
        {
            _context?.Dispose();
        }
    }
}
