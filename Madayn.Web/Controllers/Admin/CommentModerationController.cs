using Madayn.Web.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Madayn.Web.Controllers.Admin;

[Authorize(Roles = "Admin")]
[Route("{culture:regex(^(ar|en)$)}/Admin/[controller]")]
public class CommentModerationController : Controller
{
    private readonly ApplicationDbContext _context;
    public CommentModerationController(ApplicationDbContext context) { _context = context; }

    [HttpPost("approve/{id}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Approve(int id)
    {
        var c = await _context.Comments.FirstOrDefaultAsync(x => x.CommentId == id);
        if (c == null) return NotFound();
        c.IsApproved = true; c.IsHidden = false; c.ApprovedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return Redirect(Request.Headers["Referer"].ToString());
    }

    [HttpPost("reject/{id}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Reject(int id)
    {
        var c = await _context.Comments.FirstOrDefaultAsync(x => x.CommentId == id);
        if (c == null) return NotFound();
        c.IsApproved = false; c.IsHidden = true;
        await _context.SaveChangesAsync();
        return Redirect(Request.Headers["Referer"].ToString());
    }

    [HttpPost("hide/{id}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Hide(int id)
    {
        var c = await _context.Comments.FirstOrDefaultAsync(x => x.CommentId == id);
        if (c == null) return NotFound();
        c.IsHidden = true; await _context.SaveChangesAsync();
        return Redirect(Request.Headers["Referer"].ToString());
    }

    [HttpPost("unhide/{id}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Unhide(int id)
    {
        var c = await _context.Comments.FirstOrDefaultAsync(x => x.CommentId == id);
        if (c == null) return NotFound();
        c.IsHidden = false; await _context.SaveChangesAsync();
        return Redirect(Request.Headers["Referer"].ToString());
    }
}
