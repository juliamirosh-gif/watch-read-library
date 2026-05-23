using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WatchReadLibrary.Models;

namespace WatchReadLibrary.Controllers
{
    [Authorize]
    public class LibraryItemsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LibraryItemsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string? category, string? search, int? minRating, bool favorites = false, string? sort = "newest")
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var items = _context.LibraryItems
                .Where(x => x.UserId == userId)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(category))
                items = items.Where(i => i.Category == category);

            if (!string.IsNullOrWhiteSpace(search))
                items = items.Where(i => i.Title != null && i.Title.ToLower().Contains(search.ToLower()));

            if (minRating.HasValue)
                items = items.Where(i => i.Rating >= minRating.Value);

            if (favorites)
                items = items.Where(i => i.IsFavorite);

            items = sort switch
            {
                "rating" => items.OrderByDescending(i => i.Rating),
                "year_new" => items.OrderByDescending(i => i.Year),
                "year_old" => items.OrderBy(i => i.Year),
                "title" => items.OrderBy(i => i.Title),
                _ => items.OrderByDescending(i => i.CreatedAt)
            };

            ViewBag.Category = category;
            ViewBag.Search = search;
            ViewBag.MinRating = minRating;
            ViewBag.Favorites = favorites;
            ViewBag.Sort = sort;

            return View(await items.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var libraryItem = await _context.LibraryItems
                .FirstOrDefaultAsync(m => m.Id == id && m.UserId == userId);

            if (libraryItem == null) return NotFound();

            return View(libraryItem);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Category,Description,Review,Rating,Year,Status,CoverPath,CoverFile,IsFavorite")] LibraryItem libraryItem)
        {
            if (ModelState.IsValid)
            {
                libraryItem.Title = libraryItem.Title.Trim();
                libraryItem.Category = libraryItem.Category.Trim();
                libraryItem.Status = libraryItem.Status.Trim();

                libraryItem.Description = libraryItem.Description?.Trim() ?? string.Empty;
                libraryItem.Review = libraryItem.Review?.Trim() ?? string.Empty;

                if (libraryItem.CoverFile != null)
                {
                    var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };
                    var extension = Path.GetExtension(libraryItem.CoverFile.FileName).ToLower();

                    if (!allowedExtensions.Contains(extension))
                    {
                        ModelState.AddModelError("", "Дозволені лише JPG, PNG та WEBP.");
                        return View(libraryItem);
                    }

                    if (libraryItem.CoverFile.Length > 3 * 1024 * 1024)
                    {
                        ModelState.AddModelError("", "Максимальний розмір файлу — 3 MB.");
                        return View(libraryItem);
                    }

                    string uploadsFolder = Path.Combine(
                        Directory.GetCurrentDirectory(),
                        "wwwroot",
                        "uploads");

                    if (!Directory.Exists(uploadsFolder))
                        Directory.CreateDirectory(uploadsFolder);

                    string uniqueFileName =
                        $"{Guid.NewGuid():N}_{DateTime.UtcNow.Ticks}{extension}";

                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await libraryItem.CoverFile.CopyToAsync(stream);
                    }

                    libraryItem.CoverPath = "/uploads/" + uniqueFileName;
                }

                libraryItem.CreatedAt = DateTime.UtcNow;
                libraryItem.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                _context.Add(libraryItem);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Запис успішно додано";
                return RedirectToAction(nameof(Index));
            }

            return View(libraryItem);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var libraryItem = await _context.LibraryItems
                .FirstOrDefaultAsync(m => m.Id == id && m.UserId == userId);

            if (libraryItem == null) return NotFound();

            return View(libraryItem);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Category,Description,Review,Rating,Year,Status,CoverPath,CoverFile,IsFavorite")] LibraryItem libraryItem)
        {
            if (id != libraryItem.Id) return NotFound();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (ModelState.IsValid)
            {
                libraryItem.Title = libraryItem.Title.Trim();
                libraryItem.Category = libraryItem.Category.Trim();
                libraryItem.Status = libraryItem.Status.Trim();

                libraryItem.Description = libraryItem.Description?.Trim() ?? string.Empty;
                libraryItem.Review = libraryItem.Review?.Trim() ?? string.Empty;

                try
                {
                    var existingItem = await _context.LibraryItems
                        .FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId);

                    if (existingItem == null) return NotFound();

                    existingItem.Title = libraryItem.Title;
                    existingItem.Category = libraryItem.Category;
                    existingItem.Description = libraryItem.Description;
                    existingItem.Review = libraryItem.Review;
                    existingItem.Rating = libraryItem.Rating;
                    existingItem.Year = libraryItem.Year;
                    existingItem.Status = libraryItem.Status;
                    existingItem.IsFavorite = libraryItem.IsFavorite;

                    if (libraryItem.CoverFile != null)
                    {
                        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };
                        var extension = Path.GetExtension(libraryItem.CoverFile.FileName).ToLower();

                        if (!allowedExtensions.Contains(extension))
                        {
                            ModelState.AddModelError("", "Дозволені лише JPG, PNG та WEBP.");
                            return View(libraryItem);
                        }

                        if (libraryItem.CoverFile.Length > 3 * 1024 * 1024)
                        {
                            ModelState.AddModelError("", "Максимальний розмір файлу — 3 MB.");
                            return View(libraryItem);
                        }

                        string uploadsFolder = Path.Combine(
                            Directory.GetCurrentDirectory(),
                            "wwwroot",
                            "uploads");

                        if (!Directory.Exists(uploadsFolder))
                            Directory.CreateDirectory(uploadsFolder);

                        string uniqueFileName =
                            $"{Guid.NewGuid():N}_{DateTime.UtcNow.Ticks}{extension}";

                        string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await libraryItem.CoverFile.CopyToAsync(stream);
                        }

                        libraryItem.CoverPath = "/uploads/" + uniqueFileName;
                    }

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LibraryItemExists(libraryItem.Id))
                        return NotFound();

                    throw;
                }

                TempData["Success"] = "Зміни успішно збережено";
                return RedirectToAction(nameof(Index));
            }

            return View(libraryItem);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var libraryItem = await _context.LibraryItems
                .FirstOrDefaultAsync(m => m.Id == id && m.UserId == userId);

            if (libraryItem == null) return NotFound();

            return View(libraryItem);
        }

        [HttpPost, ActionName("Delete")]
[ValidateAntiForgeryToken]
public async Task<IActionResult> DeleteConfirmed(int id)
{
    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

    var libraryItem = await _context.LibraryItems
        .FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId);

    if (libraryItem == null)
        return NotFound();

    if (!string.IsNullOrWhiteSpace(libraryItem.CoverPath))
    {
        var coverPath = libraryItem.CoverPath.TrimStart('/');
        var fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", coverPath);

        if (System.IO.File.Exists(fullPath))
        {
            System.IO.File.Delete(fullPath);
        }
    }

    _context.LibraryItems.Remove(libraryItem);
    await _context.SaveChangesAsync();

    TempData["Success"] = "Запис видалено";
    return RedirectToAction(nameof(Index));
}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleFavorite(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var item = await _context.LibraryItems
                .FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId);

            if (item == null)
                return NotFound();

            item.IsFavorite = !item.IsFavorite;
            await _context.SaveChangesAsync();

            return Json(new
            {
                success = true,
                isFavorite = item.IsFavorite
            });
        }

        private bool LibraryItemExists(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            return _context.LibraryItems.Any(e => e.Id == id && e.UserId == userId);
        }
    }
}