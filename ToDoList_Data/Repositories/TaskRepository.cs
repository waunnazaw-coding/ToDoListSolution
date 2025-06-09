using Microsoft.EntityFrameworkCore;
using ToDoList_Data.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ToDoList_Data.Repositories
{
    public class TaskRepository : ITaskRepository
    {
        private readonly AppDbContext _context;

        public TaskRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ToDoItem> GetByIdAsync(int id)
        {
            return await _context.ToDoItems.FindAsync(id);
        }
        
        public async Task<List<ToDoItem>> GetAllAsync()
        {
            return await _context.ToDoItems.ToListAsync();
        }
        
        public async Task AddAsync(ToDoItem task)
        {
            await _context.ToDoItems.AddAsync(task);
            await _context.SaveChangesAsync();
        }
        
        public async Task UpdateAsync(ToDoItem task)
        {
            _context.Entry(task).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }
        
        public async Task DeleteAsync(int id)
        {
            var entity = await GetByIdAsync(id);

            if (entity == null)
            {
              throw new ArgumentNullException(nameof(entity));
            }

            _context.ToDoItems.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }
}