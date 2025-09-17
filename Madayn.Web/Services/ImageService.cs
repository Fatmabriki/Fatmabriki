using Madayn.Web.Data;
using Madayn.Web.Models;
using Microsoft.AspNetCore.Hosting;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace Madayn.Web.Services;

public interface IImageService
{
    Task<ImageUploadResult> UploadProfileImageAsync(int userId, IFormFile image);
    Task<ImageUploadResult> DeleteProfileImageAsync(int userId);
    Task<string> GenerateThumbnailAsync(string imagePath, int width = 150, int height = 150);
    bool ValidateImage(IFormFile image);
}

public class ImageService : IImageService
{
    private readonly IWebHostEnvironment _environment;
    private readonly ApplicationDbContext _context;

    public ImageService(IWebHostEnvironment environment, ApplicationDbContext context)
    {
        _environment = environment;
        _context = context;
    }

    public async Task<ImageUploadResult> UploadProfileImageAsync(int userId, IFormFile image)
    {
        if (!ValidateImage(image))
        {
            return new ImageUploadResult { Success = false, ErrorMessage = "Invalid image format or size" };
        }

        try
        {
            var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads", "profiles");
            Directory.CreateDirectory(uploadsFolder);

            var extension = Path.GetExtension(image.FileName);
            var fileName = $"profile_{userId}_{DateTime.UtcNow:yyyyMMddHHmmss}{extension}";
            var filePath = Path.Combine(uploadsFolder, fileName);

            await DeleteOldProfileImageAsync(userId);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await image.CopyToAsync(stream);
            }

            var thumbPath = await GenerateThumbnailAsync(filePath);

            var imageUrl = $"/uploads/profiles/{fileName}";
            var thumbnailUrl = $"/uploads/profiles/thumbnails/{Path.GetFileName(thumbPath)}";

            await SaveImageHistoryAsync(userId, imageUrl, image.Length, image.ContentType);

            var profile = await _context.UserProfiles.FindAsync(userId);
            if (profile != null)
            {
                profile.ProfileImageUrl = imageUrl;
                profile.ProfileImageThumbnailUrl = thumbnailUrl;
                profile.LastProfileUpdate = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }

            return new ImageUploadResult { Success = true, ImageUrl = imageUrl, ThumbnailUrl = thumbnailUrl };
        }
        catch (Exception ex)
        {
            return new ImageUploadResult { Success = false, ErrorMessage = "Error uploading image: " + ex.Message };
        }
    }

    public bool ValidateImage(IFormFile image)
    {
        if (image.Length > 5 * 1024 * 1024) return false;
        var allowed = new[] { "image/jpeg", "image/jpg", "image/png", "image/gif" };
        return allowed.Contains(image.ContentType.ToLowerInvariant());
    }

    public async Task<string> GenerateThumbnailAsync(string imagePath, int width = 150, int height = 150)
    {
        var thumbnailsFolder = Path.Combine(_environment.WebRootPath, "uploads", "profiles", "thumbnails");
        Directory.CreateDirectory(thumbnailsFolder);
        var fileName = Path.GetFileName(imagePath);
        var thumbPath = Path.Combine(thumbnailsFolder, fileName);

        using var image = await Image.LoadAsync(imagePath);
        image.Mutate(x => x.Resize(new ResizeOptions
        {
            Size = new Size(width, height),
            Mode = ResizeMode.Crop
        }));
        await image.SaveAsync(thumbPath);
        return thumbPath;
    }

    public async Task<ImageUploadResult> DeleteProfileImageAsync(int userId)
    {
        var profile = await _context.UserProfiles.FindAsync(userId);
        if (profile == null) return new ImageUploadResult { Success = false, ErrorMessage = "Profile not found" };

        try
        {
            if (!string.IsNullOrWhiteSpace(profile.ProfileImageUrl))
            {
                var path = Path.Combine(_environment.WebRootPath, profile.ProfileImageUrl.TrimStart('/'));
                if (File.Exists(path)) File.Delete(path);
            }
            if (!string.IsNullOrWhiteSpace(profile.ProfileImageThumbnailUrl))
            {
                var pathT = Path.Combine(_environment.WebRootPath, profile.ProfileImageThumbnailUrl.TrimStart('/'));
                if (File.Exists(pathT)) File.Delete(pathT);
            }
            profile.ProfileImageUrl = null;
            profile.ProfileImageThumbnailUrl = null;
            await _context.SaveChangesAsync();
            return new ImageUploadResult { Success = true };
        }
        catch (Exception ex)
        {
            return new ImageUploadResult { Success = false, ErrorMessage = ex.Message };
        }
    }

    private async Task DeleteOldProfileImageAsync(int userId)
    {
        var profile = await _context.UserProfiles.FindAsync(userId);
        if (profile == null) return;
        if (!string.IsNullOrEmpty(profile.ProfileImageUrl))
        {
            var oldPath = Path.Combine(_environment.WebRootPath, profile.ProfileImageUrl.TrimStart('/'));
            if (File.Exists(oldPath)) File.Delete(oldPath);
        }
        if (!string.IsNullOrEmpty(profile.ProfileImageThumbnailUrl))
        {
            var oldThumb = Path.Combine(_environment.WebRootPath, profile.ProfileImageThumbnailUrl.TrimStart('/'));
            if (File.Exists(oldThumb)) File.Delete(oldThumb);
        }
    }

    private async Task SaveImageHistoryAsync(int userId, string imageUrl, long fileSize, string mimeType)
    {
        var history = new ProfileImageHistory
        {
            UserId = userId,
            ImageUrl = imageUrl,
            ImageSize = fileSize,
            MimeType = mimeType,
            UploadedAt = DateTime.UtcNow
        };
        _context.ProfileImageHistory.Add(history);
        await _context.SaveChangesAsync();
    }
}

public class ImageUploadResult
{
    public bool Success { get; set; }
    public string? ImageUrl { get; set; }
    public string? ThumbnailUrl { get; set; }
    public string? ErrorMessage { get; set; }
}
