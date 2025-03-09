using BloodCenter.Data;
using BloodCenter.Models;
using BloodCenter.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace BloodCenter.Controllers
{
    public class DonationHistoryController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ApplicationDbContext _context;
        public DonationHistoryController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        [Authorize(Roles = "Admin, MedicalSpecialist")]
        [HttpGet]
        public async Task<IActionResult> History(DateTime? startDate, DateTime? endDate)
        {
            var donations = _context.DonationHistories
                .Include(d => d.BloodDonor)
                .ThenInclude(bd => bd.User)
                .Include(d => d.BloodDonor.BloodGroup)
                .AsQueryable();

            if (startDate.HasValue)
            {
                donations = donations.Where(d => d.DonationDate.Date >= startDate.Value.Date);
            }
            if (endDate.HasValue)
            {
                donations = donations.Where(d => d.DonationDate.Date <= endDate.Value.Date);
            }

            ViewData["StartDate"] = startDate?.ToString("yyyy-MM-dd"); 
            ViewData["EndDate"] = endDate?.ToString("yyyy-MM-dd");

            ViewData["BloodDonors"] = new SelectList(_context.BloodDonors, "Id", "User.FirstName");
            return View(await donations.ToListAsync());
        }

        [Authorize(Roles = "Admin, MedicalSpecialist")]
        [HttpGet]
        public async Task<IActionResult> DonationHistory(int id)
        {
            var donor = await _context.BloodDonors
                .Include(d => d.DonationHistory)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (donor == null)
            {
                return NotFound();
            }

            return View(donor);
        }

        [Authorize(Roles = "Admin, MedicalSpecialist")]
        [HttpGet]
        public IActionResult AddDonation()
        {
            ViewData["BloodDonors"] = new SelectList(_context.BloodDonors.Include(d => d.User), "Id", "User.FirstName");

            return View();
        }

        [Authorize(Roles = "Admin, MedicalSpecialist")]
        [HttpPost]
        public async Task<IActionResult> AddDonation(DonationViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewData["BloodDonors"] = new SelectList(_context.BloodDonors.Include(d => d.User), "Id", "User.FirstName");
                return View(model);
            }

            var donor = await _context.BloodDonors.FindAsync(model.BloodDonorId);

            if (donor == null)
            {
                return NotFound("Кръводарителят не е намерен.");
            }

            // Добавяне на дарение в историята
            DonationHistory donation = new DonationHistory
            {
                BloodDonorId = donor.Id,
                DonationDate = DateTime.Now,
                Quantity = model.Quantity
            };

            _context.DonationHistories.Add(donation);
            await _context.SaveChangesAsync();

            await UpdateBloodSupply(donor.Id, model.Quantity);
            return RedirectToAction("History", "DonationHistory");
        }

        private async Task UpdateBloodSupply(int donorId, double quantity)
        {
            var donor = await _context.BloodDonors
                .Include(d => d.BloodGroup) // Включваме връзката към кръвната група
                .FirstOrDefaultAsync(d => d.Id == donorId);

            if (donor == null)
            {
                throw new Exception("Кръводарителят не съществува.");
            }

            var supply = await _context.Supplies
                .FirstOrDefaultAsync(s => s.BloodGroupId == donor.BloodGroupId && s.RhesusFactor == donor.RhesusFactor);

            if (supply != null)
            {
                // Ако има запис за тази кръвна група и резус-фактор, увеличаваме количеството
                supply.Quantity += quantity;
                supply.LastUpdated = DateTime.Now;
            }
            else
            {
                // Ако няма такъв запис, създаваме нов
                var newSupply = new Supply
                {
                    BloodGroupId = donor.BloodGroupId,
                    RhesusFactor = donor.RhesusFactor,
                    Quantity = quantity,
                    LastUpdated = DateTime.Now
                };
                await _context.Supplies.AddAsync(newSupply);
            }

            await _context.SaveChangesAsync();
        }

        [Authorize(Roles = "Admin, MedicalSpecialist")]
        [HttpGet]
        public IActionResult DeleteDonation(int id)
        {
            var donation = _context.DonationHistories
                .Include(d => d.BloodDonor)
                .ThenInclude(bd => bd.User) // Включваме потребителя на кръводарителя
                .FirstOrDefault(d => d.Id == id);

            if (donation == null)
            {
                return NotFound();
            }

            var viewModel = new DonationViewModel
            {
                BloodDonorId = donation.BloodDonorId,
                Quantity = donation.Quantity,
                DonationDate = donation.DonationDate
            };

            return View(viewModel);
        }

        [Authorize(Roles = "Admin, MedicalSpecialist")]
        [HttpPost]
        public async Task<IActionResult> DeleteConfirmedDonation(int id)
        {
            var donation = await _context.DonationHistories
                .Include(d => d.BloodDonor)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (donation == null)
            {
                return NotFound();
            }

            // Намаляване на наличностите
            await UpdateBloodSupply(donation.BloodDonorId, -donation.Quantity);

            _context.DonationHistories.Remove(donation);
            await _context.SaveChangesAsync();

            return RedirectToAction("History");
        }



        //private async Task RemoveFromBloodSupply(int donorId, double quantity)
        //{
        //    var donor = await _context.BloodDonors
        //        .Include(d => d.BloodGroup)
        //        .FirstOrDefaultAsync(d => d.Id == donorId);

        //    if (donor == null)
        //    {
        //        throw new Exception("Кръводарителят не съществува.");
        //    }

        //    var supply = await _context.Supplies
        //        .FirstOrDefaultAsync(s => s.BloodGroupId == donor.BloodGroupId && s.RhesusFactor == donor.RhesusFactor);

        //    if (supply != null)
        //    {
        //        // Намаляване на количеството, но да не става отрицателно
        //        supply.Quantity = Math.Max(0, supply.Quantity - quantity);
        //        supply.LastUpdated = DateTime.Now;

        //        await _context.SaveChangesAsync();
        //    }
        //}


        [Authorize(Roles = "Donor")]
        public async Task<IActionResult> MyDonations()
        {
            // Взима логнатия потребител
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account"); // Ако не е логнат, пренасочи към логин
            }

            var userId = user.Id;

            // Намира всички дарения на този потребител
            var donations = await _context.DonationHistories
            .Where(d => d.BloodDonor.UserId == userId)
            .OrderByDescending(d => d.DonationDate)
            .ToListAsync();

            return View(donations);
        }
    }
}
