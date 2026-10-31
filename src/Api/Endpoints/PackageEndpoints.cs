using Application.DTOs;
using Application.Interfaces;

namespace Api.Endpoints;

public static class PackageEndpoints
{
    /// <summary>Maps CRUD endpoints for composite packages under /api/packages.</summary>
    public static IEndpointRouteBuilder MapPackageEndpoints(this IEndpointRouteBuilder app)
    {
        var packages = app.MapGroup("/api/packages").WithTags("Packages");

        packages.MapGet("/", (IPackageService s, CancellationToken ct) => s.GetAllAsync(ct));
        packages.MapGet("/{id:guid}", (Guid id, IPackageService s, CancellationToken ct) => s.GetByIdAsync(id, ct));
        packages.MapPost("/", async (CreatePackageRequest r, IPackageService s, CancellationToken ct) =>
        {
            var created = await s.CreateAsync(r, ct);
            return Results.Created($"/api/packages/{created.Id}", created);
        });
        packages.MapPut("/{id:guid}", (Guid id, UpdatePackageRequest r, IPackageService s, CancellationToken ct) => s.UpdateAsync(id, r, ct));
        packages.MapDelete("/{id:guid}", async (Guid id, IPackageService s, CancellationToken ct) =>
        {
            await s.DeleteAsync(id, ct);
            return Results.NoContent();
        });

        return app;
    }
}
