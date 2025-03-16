using BloodCenter.Data;
using BloodCenter.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BloodCenter.Controllers
{
    [Authorize(Roles = "Admin, MedicalSpecialist")]
    public class SupplyController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SupplyController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> BloodInventory()
        {
            var supplies = await _context.Supplies
                .Include(s => s.BloodGroup)
                .ToListAsync();

            return View(supplies);
        }
    }
}
