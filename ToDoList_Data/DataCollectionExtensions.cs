using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ToDoList_Data.Models;
using ToDoList_Data.Repositories;

namespace ToDoList_Data
{
    public static class DataCollectionExtensions
    {
        public static IServiceCollection AddData(this IServiceCollection services, string connectionString)
        {
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(connectionString));

            services.AddScoped<ITaskRepository, TaskRepository>();

            return services; // Return the IServiceCollection for chaining
        }
    }
}