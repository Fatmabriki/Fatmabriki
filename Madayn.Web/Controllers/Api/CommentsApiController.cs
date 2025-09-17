using Madayn.Web.Data;
using Madayn.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace Madayn.Web.Controllers.Api;

[ApiController]
[Route("api/[controller]")]
public class CommentsApiController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    public CommentsApiController(ApplicationDbContext context) { _context = context; }

    public record CommentDto(string EntityType, int EntityId, string Text, string? Name, int? UserId);

    [HttpPost("")]
    public async Task<IActionResult> Post(CommentDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Text)) return BadRequest();
        var c = new Comment
        {
            EntityType = dto.EntityType,
            EntityId = dto.EntityId,
            CommentText = dto.Text,
            AnonymousName = dto.Name,
            UserId = dto.UserId,
            IsApproved = false,
            IsHidden = false,
            CreatedAt = DateTime.UtcNow
        };
        _context.Comments.Add(c);
        await _context.SaveChangesAsync();
        return Ok(new { success = true, pendingModeration = true });
    }
}
