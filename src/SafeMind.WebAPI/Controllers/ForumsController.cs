using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using SafeMind.Application.Interfaces;
using SafeMind.Application.DTOs;
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ForumsController : ControllerBase
{
    private readonly IForumService _forumService;

    public ForumsController(IForumService forumService) => _forumService = forumService;

    private Guid GetRequesterId() =>
        Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateForumDto dto)
    {
        var forum = await _forumService.CreateAsync(dto, GetRequesterId());
        return CreatedAtAction(nameof(GetById), new { id = forum.Id }, forum);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll() =>
        Ok(await _forumService.GetAllAsync());

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        // 1. A BARREIRA DA KAN-9 (O Segurança atua na porta de entrada!)
        await _forumService.ValidarAcessoAoForumAsync(GetRequesterId(), id);
        
        var forum = await _forumService.GetByIdAsync(id);
        return forum is null ? NotFound() : Ok(forum);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateForumDto dto)
    {
        // 1. A BARREIRA DA KAN-9 (O Segurança atua na edição também!)
        await _forumService.ValidarAcessoAoForumAsync(GetRequesterId(), id);
        
        // 2. Se passar pela barreira sem explodir, atualiza o fórum
        await _forumService.UpdateAsync(id, dto, GetRequesterId());
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _forumService.DeleteAsync(id, GetRequesterId());
        return NoContent();
    }
}