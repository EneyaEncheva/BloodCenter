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
            return View();
        }

        public async Task<IActionResult> Statistics()
        {
            var bloodGroupCounts = await _context.Requests
                .Include(r => r.BloodGroup)
                .GroupBy(r => new { r.BloodGroup.Name, r.RhesusFactor })
                .Select(g => new
                {
                    BloodGroupName = g.Key.Name,
                    RhesusFactor = g.Key.RhesusFactor, // Явно посочваме, за да избегнем проблеми
                    Count = g.Count()
                })
                .OrderByDescending(g => g.Count)
                .Take(3)
                .ToListAsync();

            ViewData["MostRequestedBloodGroups"] = bloodGroupCounts; // Запазваме като IEnumerable<dynamic>
            return View();
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
