using BloodCenter.Data;
using BloodCenter.Models;
using BloodCenter.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;

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
        public async Task<IActionResult> History(string searched, DateTime? startDate, DateTime? endDate, int? bloodGroupId, char? rhesusFactor, int pageIndex = 1)
        {
            var donations = _context.DonationHistories
                .Include(d => d.BloodDonor)
                .ThenInclude(bd => bd.User)
                .Include(d => d.BloodDonor.BloodGroup)
                .AsQueryable();

            //Търсене
            if (!string.IsNullOrWhiteSpace(searched))
            {
                string lowerSearched = searched.ToLower();
                donations = donations.Where(d =>
                    string.Concat(d.BloodDonor.User.FirstName.ToLower(), " ", d.BloodDonor.User.LastName.ToLower()).Contains(lowerSearched));
            }

            //Филтриране
            if (startDate.HasValue)
            {
                donations = donations.Where(d => d.DonationDate.Date >= startDate.Value.Date);
            }
            if (endDate.HasValue)
            {
                donations = donations.Where(d => d.DonationDate.Date <= endDate.Value.Date);
            }

            if (bloodGroupId.HasValue && bloodGroupId > 0)
            {
                donations = donations.Where(d => d.BloodDonor.BloodGroupId == bloodGroupId);
            }

            if (rhesusFactor.HasValue)
            {
                donations = donations.Where(d => d.BloodDonor.RhesusFactor == rhesusFactor.Value);
            }

            const int pageSize = 5;

            var donationList = await donations.ToListAsync();
            var paginatedDonations = PaginatedList<DonationHistory>.Create(donationList, pageIndex, pageSize);

            ViewData["Searched"] = searched;
            ViewData["StartDate"] = startDate?.ToString("yyyy-MM-dd"); 
            ViewData["EndDate"] = endDate?.ToString("yyyy-MM-dd");
            

            ViewData["BloodDonors"] = new SelectList(_context.BloodDonors, "Id", "User.FirstName");
            ViewData["BloodGroups"] = new SelectList(_context.BloodGroups, "Id", "Name");
            ViewData["RhesusFactor"] = rhesusFactor;
            return View(donationList);
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
            ViewBag.BloodDonors = _context.BloodDonors
                .Include(d => d.User)
                .Include(d => d.BloodGroup)
                .Select(d => new SelectListItem
                {
                    Value = d.Id.ToString(),
                    Text = $"{d.User.FirstName} {d.User.LastName} ({d.BloodGroup.Name}{d.RhesusFactor})"
                })
                .ToList();

            return View(new DonationViewModel());
        }

        [Authorize(Roles = "Admin, MedicalSpecialist")]
        [HttpPost]
        public async Task<IActionResult> AddDonation(DonationViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.BloodDonors = _context.BloodDonors
                    .Include(d => d.User)
                    .Include(d => d.BloodGroup)
                    .Select(d => new SelectListItem
                    {
                        Value = d.Id.ToString(),
                        Text = $"{d.User.FirstName} {d.User.LastName} ({d.BloodGroup.Name}{d.RhesusFactor})"
                    })
                    .ToList();

                return View(model);
            }

            var donor = await _context.BloodDonors.FindAsync(model.BloodDonorId);

            if (donor == null)
            {
                ModelState.AddModelError("", "Кръводарителят не е намерен.");
                return View(model);
            }

            // Записване в историята на даренията
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
            // Гарантираме, че дареното количество е в интервала 405-450 мл
            if (quantity < 405 || quantity > 450)
            {
                throw new Exception("Количеството дарена кръв трябва да бъде между 405 и 450 мл.");
            }

            var donor = await _context.BloodDonors
                .Include(d => d.BloodGroup)
                .FirstOrDefaultAsync(d => d.Id == donorId);

            if (donor == null)
            {
                throw new Exception("Кръводарителят не съществува.");
            }

            var supply = await _context.Availabilities
                .FirstOrDefaultAsync(s => s.BloodGroupId == donor.BloodGroupId && s.RhesusFactor == donor.RhesusFactor);

            // Преобразуваме количеството в сакове (1 сак = 405-450 мл)
            int bloodBags = 1;

            if (supply != null)
            {
                // Ако има запис, увеличаваме наличното количество с 1 сак
                supply.Quantity += bloodBags;
                supply.LastUpdated = DateTime.Now;
            }
            else
            {
                // Ако няма запис, създаваме нов със стартова стойност 1 сак
                var newSupply = new Availability
                {
                    BloodGroupId = donor.BloodGroupId,
                    RhesusFactor = donor.RhesusFactor,
                    Quantity = bloodBags,
                    LastUpdated = DateTime.Now
                };
                await _context.Availabilities.AddAsync(newSupply);
            }

            await _context.SaveChangesAsync();
        }


        [Authorize(Roles = "Admin, MedicalSpecialist")]
        [HttpGet]
        public IActionResult DeleteDonation(int id)
        {
            var donation = _context.DonationHistories
                .Include(d => d.BloodDonor)
                .ThenInclude(bd => bd.User) 
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

        [Authorize(Roles = "Donor")]
        public async Task<IActionResult> MyDonations()
        {
            // Взема логнатия потребител
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
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
