using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using ToDoList_Services.Services;

namespace ToDoList_Services;

public static class ServicesManager
{
    public static void AddDomain(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<ITaskService , TaskService>();
    }
}