//using BloodCenter.Data;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;

//namespace BloodCenter.Controllers
//{
//    [Authorize(Roles = "Admin")] 
//    public class DonationHistoryController : Controller
//    {
//        private readonly ApplicationDbContext _context;

//        public DonationHistoryController(ApplicationDbContext context)
//        {
//            _context = context;
//        }

//        public async Task<IActionResult> Index()
//        {
//            var donations = await _context.DonationHistories
//                .Include(d => d.BloodDonor)
//                .ToListAsync();

//            return View(donations);
//        }
//    }
//}
