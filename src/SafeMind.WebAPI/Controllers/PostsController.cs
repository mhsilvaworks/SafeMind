using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using SafeMind.Application.Interfaces;
using SafeMind.Application.DTOs;
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PostsController : ControllerBase
{
    private readonly IPostService _postService;

    public PostsController(IPostService postService) => _postService = postService;

    private Guid GetRequesterId() =>
        Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreatePostDto dto)
    {
        var post = await _postService.CreateAsync(dto, GetRequesterId());
        return CreatedAtAction(nameof(GetById), new { id = post.Id }, post);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var post = await _postService.GetByIdAsync(id);
        return post is null ? NotFound() : Ok(post);
    }

    [HttpGet("forum/{forumId}")]
    public async Task<IActionResult> GetByForum(Guid forumId) =>
        Ok(await _postService.GetByForumAsync(forumId));

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdatePostDto dto)
    {
        await _postService.UpdateAsync(id, dto, GetRequesterId());
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _postService.DeleteAsync(id, GetRequesterId());
        return NoContent();
    }
}