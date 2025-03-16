using BloodCenter.Data;
using BloodCenter.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace BloodCenter.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var bloodGroupCounts = await _context.Requests
                .Include(r => r.BloodGroup)
                .GroupBy(r => new { r.BloodGroup.Name, r.RhesusFactor }) // Групиране и по резус-фактор
                .Select(g => new
                {
                    BloodGroupName = g.Key.Name,
                    g.Key.RhesusFactor, // Добавено
                    Count = g.Count()
                })
                .OrderByDescending(g => g.Count)
                .Take(3)
                .ToListAsync();

            ViewData["MostRequestedBloodGroups"] = bloodGroupCounts;
            return View(bloodGroupCounts);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
