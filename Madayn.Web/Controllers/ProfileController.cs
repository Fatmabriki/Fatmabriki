using Madayn.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace Madayn.Web.Controllers;

[Authorize]
[Route("{culture:regex(^(ar|en)$)}/[controller]")]
public class ProfileController : BaseController
{
    private readonly IProfileService _profileService;
    private readonly IImageService _imageService;
    private readonly UserManager<IdentityUser> _userManager;

    public ProfileController(IStringLocalizer<SharedResource> localizer,
        IProfileService profileService,
        IImageService imageService,
        UserManager<IdentityUser> userManager) : base(localizer)
    {
        _profileService = profileService;
        _imageService = imageService;
        _userManager = userManager;
    }

    private int GetCurrentUserId()
    {
        // This assumes a bridge exists between Identity and UserProfile
        // For MVP, fetch by AspNetUserId
        var aspId = _userManager.GetUserId(User)!;
        var profile = _profileService.GetProfileByAspNetUserIdAsync(aspId).GetAwaiter().GetResult();
        if (profile == null) throw new InvalidOperationException("Profile not found");
        return profile.UserId;
    }

    [HttpGet("")]
    public async Task<IActionResult> Index()
    {
        var userId = GetCurrentUserId();
        var profile = await _profileService.GetProfileAsync(userId);
        if (profile == null)
        {
            return RedirectToAction("Edit");
        }

        var viewModel = new ProfileViewModel
        {
            Profile = profile,
            CanEdit = true,
            ShowContactInfo = profile.ShowEmail || profile.ShowPhone,
            ActivityCount = new ActivityCountViewModel
            {
                SurveysCompleted = 0,
                CommentsPosted = 0,
                RatingsGiven = 0
            }
        };
        return View(viewModel);
    }

    [HttpGet("edit")]
    public async Task<IActionResult> Edit()
    {
        var userId = GetCurrentUserId();
        var profile = await _profileService.GetProfileAsync(userId);
        var model = new EditProfileViewModel();
        if (profile != null)
        {
            model = new EditProfileViewModel
            {
                FirstName = profile.FirstName,
                LastName = profile.LastName,
                DisplayName = profile.DisplayName,
                Bio = profile.Bio,
                Position = profile.Position,
                Company = profile.Company,
                RegionId = profile.RegionId,
                Phone = profile.Phone,
                Website = profile.Website,
                LinkedIn = profile.LinkedIn,
                Twitter = profile.Twitter,
                ProfileImageUrl = profile.ProfileImageUrl,
                ProfileImageThumbnailUrl = profile.ProfileImageThumbnailUrl,
                ShowEmail = profile.ShowEmail,
                ShowPhone = profile.ShowPhone,
                ShowSocialMedia = profile.ShowSocialMedia,
                AllowMessages = profile.AllowMessages
            };
        }

        ViewBag.Regions = await Task.FromResult(Enumerable.Empty<object>());
        return View(model);
    }

    [HttpPost("edit")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(EditProfileViewModel model, IFormFile? profileImage)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }
        var userId = GetCurrentUserId();
        if (profileImage != null && profileImage.Length > 0)
        {
            var uploadResult = await _imageService.UploadProfileImageAsync(userId, profileImage);
            if (uploadResult.Success)
            {
                model.ProfileImageUrl = uploadResult.ImageUrl;
                model.ProfileImageThumbnailUrl = uploadResult.ThumbnailUrl;
            }
            else
            {
                ModelState.AddModelError(string.Empty, uploadResult.ErrorMessage ?? "");
                return View(model);
            }
        }
        await _profileService.UpdateProfileAsync(userId, model);
        TempData["Success"] = Localizer["ProfileUpdatedSuccessfully"];
        return RedirectToAction("Index");
    }

    [HttpPost("delete-image")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteProfileImage()
    {
        var userId = GetCurrentUserId();
        var result = await _imageService.DeleteProfileImageAsync(userId);
        if (result.Success)
        {
            return Json(new { success = true, message = Localizer["ImageDeletedSuccessfully"].Value });
        }
        return Json(new { success = false, message = result.ErrorMessage });
    }
}
