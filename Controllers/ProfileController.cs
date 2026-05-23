using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WatchReadLibrary.Models;

namespace WatchReadLibrary.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ProfileController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.GetUserAsync(User);

            var items = await _context.LibraryItems
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();

            var model = new ProfileViewModel
            {
                Email = user?.Email ?? "User",
                AvatarPath = user?.AvatarPath,
                TotalItems = items.Count,
                FavoriteItems = items.Count(x => x.IsFavorite),
                CompletedItems = items.Count(x =>
                    x.Status == "Прочитано" ||
                    x.Status == "Переглянуто" ||
                    x.Status == "Завершено"),
                AverageRating = items.Any()
                    ? Math.Round(items.Average(x => x.Rating), 1)
                    : 0,
                LatestItems = items.Take(3).ToList()
            };

            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAvatar()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return RedirectToAction(nameof(Index));

            if (!string.IsNullOrWhiteSpace(user.AvatarPath))
            {
                var avatarPath = user.AvatarPath.TrimStart('/');
                var fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", avatarPath);

                if (System.IO.File.Exists(fullPath))
                    System.IO.File.Delete(fullPath);

                user.AvatarPath = null;
                await _userManager.UpdateAsync(user);
            }

            TempData["Success"] = "Аватар видалено";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadAvatar(IFormFile avatarFile)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return RedirectToAction(nameof(Index));

            if (avatarFile == null || avatarFile.Length == 0)
                return RedirectToAction(nameof(Index));

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };
            var extension = Path.GetExtension(avatarFile.FileName).ToLowerInvariant();

            if (!allowedExtensions.Contains(extension))
            {
                TempData["Success"] = "Дозволені лише JPG, PNG та WEBP.";
                return RedirectToAction(nameof(Index));
            }

            if (avatarFile.Length > 2 * 1024 * 1024)
            {
                TempData["Success"] = "Максимальний розмір аватара — 2 MB.";
                return RedirectToAction(nameof(Index));
            }

            string avatarsFolder = Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot",
                "avatars");

            if (!Directory.Exists(avatarsFolder))
                Directory.CreateDirectory(avatarsFolder);

            if (!string.IsNullOrWhiteSpace(user.AvatarPath))
            {
                var oldPath = user.AvatarPath.TrimStart('/');
                var oldFullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", oldPath);

                if (System.IO.File.Exists(oldFullPath))
                    System.IO.File.Delete(oldFullPath);
            }

            string fileName = $"{Guid.NewGuid():N}_{DateTime.UtcNow.Ticks}{extension}";
            string filePath = Path.Combine(avatarsFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await avatarFile.CopyToAsync(stream);
            }

            user.AvatarPath = "/avatars/" + fileName;
            await _userManager.UpdateAsync(user);

            TempData["Success"] = "Аватар оновлено";
            return RedirectToAction(nameof(Index));
        }
    }
}
