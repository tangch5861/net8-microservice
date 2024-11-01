using Microsoft.EntityFrameworkCore;
using TodoService.Models;
using TodoService.Services;

var builder = WebApplication.CreateBuilder(args);

// Add PostgreSQL with Entity Framework Core
builder.Services.AddDbContext<TodoDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("PostgresConnection")));

// Register LoggingClient
builder.Services.AddHttpClient<LoggingClient>();

// Add Swagger services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Create the database if it doesn't exist
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<TodoDbContext>();
    db.Database.Migrate();
}

// Enable middleware for serving generated Swagger as a JSON endpoint
app.UseSwagger();

// Enable middleware for serving swagger-ui (HTML, JS, CSS, etc.)
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Todo API V1");
    c.RoutePrefix = string.Empty; // Set the Swagger UI at the app's root
});


// Log a message for each action (create, read, update, delete)

// Get all Todo items
app.MapGet("/todos", async (TodoDbContext db, LoggingClient logger) =>
{
    await logger.LogAsync("Fetching all todo items", "Information");
    return await db.TodoItems.ToListAsync();
});

// Get a specific Todo item by ID
app.MapGet("/todos/{id}", async (int id, TodoDbContext db, LoggingClient logger) =>
{
    var todo = await db.TodoItems.FindAsync(id);
    if (todo != null)
    {
        await logger.LogAsync($"Fetched todo item with ID: {id}", "Information");
        return Results.Ok(todo);
    }
    await logger.LogAsync($"Todo item with ID {id} not found", "Warning");
    return Results.NotFound();
});

// Create a new Todo item
app.MapPost("/todos", async (TodoItem todo, TodoDbContext db, LoggingClient logger) =>
{
    db.TodoItems.Add(todo);
    await db.SaveChangesAsync();
    await logger.LogAsync($"Created new todo item: {todo.Title}", "Information");
    return Results.Created($"/todos/{todo.Id}", todo);
});

// Update an existing Todo item
app.MapPut("/todos/{id}", async (int id, TodoItem updatedTodo, TodoDbContext db, LoggingClient logger) =>
{
    var todo = await db.TodoItems.FindAsync(id);
    if (todo is null)
    {
        await logger.LogAsync($"Todo item with ID {id} not found for update", "Warning");
        return Results.NotFound();
    }

    todo.Title = updatedTodo.Title;
    todo.IsCompleted = updatedTodo.IsCompleted;
    await db.SaveChangesAsync();
    await logger.LogAsync($"Updated todo item with ID: {id}", "Information");
    return Results.NoContent();
});

// Delete a Todo item
app.MapDelete("/todos/{id}", async (int id, TodoDbContext db, LoggingClient logger) =>
{
    if (await db.TodoItems.FindAsync(id) is TodoItem todo)
    {
        db.TodoItems.Remove(todo);
        await db.SaveChangesAsync();
        await logger.LogAsync($"Deleted todo item with ID: {id}", "Information");
        return Results.Ok(todo);
    }

    await logger.LogAsync($"Todo item with ID {id} not found for deletion", "Warning");
    return Results.NotFound();
});

app.Run();
