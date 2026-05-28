using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using SafeMind.Application.Interfaces;
using SafeMind.Application.DTOs;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService) => _userService = userService;

    private Guid GetRequesterId() =>
        Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpPatch("lowspoon")]
    public async Task<IActionResult> SetLowSpoonMode([FromBody] PatchLowSpoonDto dto)
    {
        await _userService.SetLowSpoonModeAsync(GetRequesterId(), dto.IsLowSpoonMode);
        return NoContent();
    }

    [HttpGet("forums")]
    public async Task<IActionResult> GetForums()
    {
        var forums = await _userService.GetFilteredForumsAsync(GetRequesterId());
        return Ok(forums);
    }

    [HttpGet("forums/{forumId}/posts")]
    public async Task<IActionResult> GetPosts(Guid forumId)
    {
        var posts = await _userService.GetFilteredPostsByForumAsync(GetRequesterId(), forumId);
        return Ok(posts);
    }
}