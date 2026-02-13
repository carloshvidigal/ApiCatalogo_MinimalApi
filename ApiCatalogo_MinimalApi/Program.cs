using ApiCatalogo_MinimalApi.Context;
using ApiCatalogo_MinimalApi.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.//ConfigureServices
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<AppDbContext>(options =>
                 options
                 .UseMySql(connectionString,
                 ServerVersion.AutoDetect(connectionString)));


var app = builder.Build();

//definir os endpoints

app.MapGet("/", () => "Catálogo de Produtos - 2026").ExcludeFromDescription();

app.MapPost("/categorias", async (Categoria categoria, AppDbContext db)
 => {
     db.Categorias.Add(categoria);
     await db.SaveChangesAsync();

     return Results.Created($"/categorias/{categoria.CategoriaId}", categoria);
 });

app.MapGet("/categorias", async (AppDbContext db) => await db.Categorias.ToListAsync());

app.MapGet("/categorias/{id:int}", async (int id, AppDbContext db)
    => {
        return await db.Categorias.FindAsync(id)
                     is Categoria categoria
                     ? Results.Ok(categoria)
                     : Results.NotFound();
    });

app.MapPut("/categorias/{id:int}", async (int id, Categoria categoria, AppDbContext db) =>
{

    if (categoria.CategoriaId != id)
    {
        return Results.BadRequest();
    }

    var categoriaDB = await db.Categorias.FindAsync(id);

    if (categoriaDB is null) return Results.NotFound();

    categoriaDB.Nome = categoria.Nome;
    categoriaDB.Descricao = categoria.Descricao;

    await db.SaveChangesAsync();

    return Results.Ok(categoriaDB);
});

app.MapDelete("/categorias/{id:int}", async (int id, AppDbContext db) =>
{
    var categoria = await db.Categorias.FindAsync(id);

    if (categoria is null)
    {
        return Results.NotFound();
    }

    db.Categorias.Remove(categoria);
    await db.SaveChangesAsync();

    return Results.NoContent();
});






// Configure the HTTP request pipeline.//Configure
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.Run();

