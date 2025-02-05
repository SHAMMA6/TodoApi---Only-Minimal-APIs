using Microsoft.EntityFrameworkCore;
using TodoApi;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddDbContext<TodoDb>(opt => opt.UseInMemoryDatabase("TodoList"));

var app = builder.Build();

app.MapGet("/api/todoitems", async (TodoDb db) =>
{
    return await db.TodoItems.ToListAsync();
});

app.MapGet("/api/todoitems/{id}", async (TodoDb db, long id) =>
{
    var todoItem = await db.TodoItems.FindAsync(id);
    if (todoItem == null)
    {
        return Results.NotFound();
    }
    return Results.Ok(todoItem);
});

app.MapPost("/api/todoitems", async (TodoDb db, TodoItem todoItem) =>
{
    db.TodoItems.Add(todoItem);
    await db.SaveChangesAsync();
    return Results.Created($"/api/todoitems/{todoItem.Id}", todoItem);
});

app.MapPut("/api/todoitems/{id}", async (TodoDb db, long id, TodoItem todoItem) =>
{
    if (id != todoItem.Id)
    {
        return Results.BadRequest();
    }
    db.Entry(todoItem).State = EntityState.Modified;
    try
    {
        await db.SaveChangesAsync();
    }
    catch (DbUpdateConcurrencyException)
    {
        if (await db.TodoItems.FindAsync(id) == null)
        {
            return Results.NotFound();
        }
        else
        {
            throw;
        }
    }
    return Results.NoContent();
});

app.MapDelete("/api/todoitems/{id}", async (TodoDb db, long id) =>
{
    var todoItem = await db.TodoItems.FindAsync(id);
    if (todoItem == null)
    {
        return Results.NotFound();
    }
    db.TodoItems.Remove(todoItem);
    await db.SaveChangesAsync();
    return Results.NoContent();
});



app.MapGet("/", () => "Hello World!");

app.Run();
