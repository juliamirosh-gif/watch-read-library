using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WatchReadLibrary.Models;

namespace WatchReadLibrary.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var userItems = _context.LibraryItems
                .Where(i => i.UserId == userId);

            ViewBag.TotalItems = await userItems.CountAsync();

            ViewBag.FavoriteItems = await userItems
                .CountAsync(i => i.IsFavorite);

            ViewBag.AverageRating = await userItems.AnyAsync()
                ? Math.Round(await userItems.AverageAsync(i => i.Rating), 1)
                : 0;

            ViewBag.LatestItems = await userItems
                .OrderByDescending(i => i.CreatedAt)
                .Take(5)
                .ToListAsync();

            ViewBag.TopItems = await userItems
                .OrderByDescending(i => i.Rating)
                .Take(5)
                .ToListAsync();

            return View();
        }
    }
}