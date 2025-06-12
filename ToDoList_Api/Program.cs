using System.Configuration;
using Microsoft.EntityFrameworkCore;
using ToDoList_Data.Repositories;
using ToDoList_Services.Services;
using ToDoList_Data;
using ToDoList_Data.Models;
using ToDoList_Services; // Assuming this contains your AppDbContext

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMemoryCache();
// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "ToDo List API",
        Version = "v1",
        Description = "An API for managing to-do items"
    });
});
builder.Services.AddData(builder.Configuration.GetConnectionString("DefaultConnection"));

builder.AddDomain();

// Add controllers
builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
    });
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();