using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriesController(ICategoryService service) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<CategoryDto>>> GetAll(CancellationToken ct)
        => Ok(await service.GetAllAsync(ct));

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<CategoryDto>> GetById(Guid id, CancellationToken ct)
        => Ok(await service.GetByIdAsync(id, ct));

    [HttpPost]
    public async Task<ActionResult<CategoryDto>> Create(CreateCategoryRequest request, CancellationToken ct)
    {
        var created = await service.CreateAsync(request, ct);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<CategoryDto>> Update(Guid id, UpdateCategoryRequest request, CancellationToken ct)
        => Ok(await service.UpdateAsync(id, request, ct));

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await service.DeleteAsync(id, ct);
        return NoContent();
    }
}
