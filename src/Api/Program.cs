using Api;
using Api.Endpoints;
using Application;
using Application.DTOs;
using Application.Interfaces;
using Infrastructure;
using Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseMiddleware<ExceptionMiddleware>();
app.UseSwagger();
app.UseSwaggerUI();

using (var scope = app.Services.CreateScope())
{
    scope.ServiceProvider.GetRequiredService<AppDbContext>().Database.EnsureCreated();
}

// ---------- Products ----------
var products = app.MapGroup("/api/products").WithTags("Products");

products.MapGet("/", (IProductService s, CancellationToken ct) => s.GetAllAsync(ct));
products.MapGet("/search", (string? q, IProductService s, CancellationToken ct) => s.SearchAsync(q, ct));
products.MapGet("/{id:guid}", (Guid id, IProductService s, CancellationToken ct) => s.GetByIdAsync(id, ct));
products.MapPost("/", async (CreateProductRequest r, IProductService s, CancellationToken ct) =>
{
    var created = await s.CreateAsync(r, ct);
    return Results.Created($"/api/products/{created.Id}", created);
});
products.MapPut("/{id:guid}", (Guid id, UpdateProductRequest r, IProductService s, CancellationToken ct) => s.UpdateAsync(id, r, ct));
products.MapDelete("/{id:guid}", async (Guid id, IProductService s, CancellationToken ct) =>
{
    await s.DeleteAsync(id, ct);
    return Results.NoContent();
});

// ---------- Orders ----------
var orders = app.MapGroup("/api/orders").WithTags("Orders");

orders.MapGet("/", (IOrderService s, CancellationToken ct) => s.GetAllAsync(ct));
orders.MapGet("/{id:guid}", (Guid id, IOrderService s, CancellationToken ct) => s.GetByIdAsync(id, ct));
orders.MapPost("/", async (CreateOrderRequest r, IOrderService s, CancellationToken ct) =>
{
    var created = await s.CreateAsync(r, ct);
    return Results.Created($"/api/orders/{created.Id}", created);
});
orders.MapPost("/{id:guid}/confirm", (Guid id, IOrderService s, CancellationToken ct) => s.ConfirmAsync(id, ct));
orders.MapPost("/{id:guid}/cancel", (Guid id, IOrderService s, CancellationToken ct) => s.CancelAsync(id, ct));

// ---------- Categories ----------
var categories = app.MapGroup("/api/categories").WithTags("Categories");

categories.MapGet("/", (ICategoryService s, CancellationToken ct) => s.GetAllAsync(ct));
categories.MapGet("/{id:guid}", (Guid id, ICategoryService s, CancellationToken ct) => s.GetByIdAsync(id, ct));
categories.MapPost("/", async (CreateCategoryRequest r, ICategoryService s, CancellationToken ct) =>
{
    var created = await s.CreateAsync(r, ct);
    return Results.Created($"/api/categories/{created.Id}", created);
});
categories.MapPut("/{id:guid}", (Guid id, UpdateCategoryRequest r, ICategoryService s, CancellationToken ct) => s.UpdateAsync(id, r, ct));
categories.MapDelete("/{id:guid}", async (Guid id, ICategoryService s, CancellationToken ct) =>
{
    await s.DeleteAsync(id, ct);
    return Results.NoContent();
});

app.MapPackageEndpoints();

app.Run();
