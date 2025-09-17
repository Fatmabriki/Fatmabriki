using Madayn.Web.Models;
using Madayn.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace Madayn.Web.Controllers.Admin;

[Authorize(Roles = "Admin")]
[Route("{culture:regex(^(ar|en)$)}/Admin/[controller]")]
public class NewsManagementController : Madayn.Web.Controllers.BaseController
{
    private readonly ICRUDService<News> _service;
    public NewsManagementController(IStringLocalizer<Madayn.Web.Controllers.SharedResource> localizer, ICRUDService<News> service) : base(localizer)
    {
        _service = service;
    }

    [HttpGet("")]
    public async Task<IActionResult> Index() => View(await _service.GetAllAsync(includeDeleted: true));

    [HttpGet("create")] public IActionResult Create() => View(new News());

    [HttpPost("create")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(News model)
    {
        if (!ModelState.IsValid) return View(model);
        await _service.CreateAsync(model);
        return RedirectToAction("Index");
    }

    [HttpGet("edit/{id}")]
    public async Task<IActionResult> Edit(int id)
    {
        var item = await _service.GetByIdAsync(id);
        if (item == null) return NotFound();
        return View(item);
    }

    [HttpPost("edit/{id}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, News model)
    {
        if (!ModelState.IsValid) return View(model);
        await _service.UpdateAsync(model);
        return RedirectToAction("Index");
    }

    [HttpPost("delete/{id}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id) { await _service.DeleteAsync(id); return RedirectToAction("Index"); }

    [HttpPost("restore/{id}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Restore(int id) { await _service.RestoreAsync(id); return RedirectToAction("Index"); }

    [HttpPost("hide/{id}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Hide(int id) { await _service.HideAsync(id); return RedirectToAction("Index"); }

    [HttpPost("unhide/{id}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Unhide(int id) { await _service.UnhideAsync(id); return RedirectToAction("Index"); }
}
