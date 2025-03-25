using BloodCenter.Data;
using BloodCenter.Enums;
using BloodCenter.Models.ViewModels;
using BloodCenter.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.Metrics;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace BloodCenter.Controllers
{
    public class AdminController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ApplicationDbContext _context;
        public AdminController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        [Authorize(Roles = "Admin, MedicalSpecialist")]
        [HttpGet]
        public async Task<IActionResult> AdminPanel(string searched, int? bloodGroupId, char? rhesusFactor, int pageIndex = 1)
        {
            ViewData["BloodGroups"] = new SelectList(_context.BloodGroups, "Id", "Name");

            var bloodDonors = _context.BloodDonors
                .Include(x => x.User)
                .Include(x => x.BloodGroup)
                .AsQueryable();

            //Търсене
            if (!string.IsNullOrEmpty(searched))
            {
                bloodDonors = bloodDonors.Where(bd =>
                    string.Concat(bd.User.FirstName, " ", bd.User.LastName).Contains(searched));
            }

            //Филтриране
            if (bloodGroupId.HasValue && bloodGroupId > 0)
            {
                bloodDonors = bloodDonors.Where(x => x.BloodGroupId == bloodGroupId);
            }

            if (rhesusFactor.HasValue)
            {
                bloodDonors = bloodDonors.Where(x => x.RhesusFactor == rhesusFactor.Value);
            }

            //Странициране
            const int pageSize = 7;

            var donorsList = await bloodDonors.ToListAsync();
            var paginatedDonors = PaginatedList<BloodDonors>.Create(donorsList, pageIndex, pageSize);

            ViewData["Searched"] = searched;
            ViewData["BloodGroupId"] = bloodGroupId;
            ViewData["RhesusFactor"] = rhesusFactor;

            return View(paginatedDonors);
        }

        [Authorize(Roles = "Admin, MedicalSpecialist")]
        public IActionResult AddBloodDonor()
        {
            ViewData["BloodGroups"] = new SelectList(_context.BloodGroups, "Id", "Name");

            var rhesusFactors = new List<SelectListItem>
            {
                new SelectListItem { Value = "+", Text = "Положителен (+)" },
                new SelectListItem { Value = "-", Text = "Отрицателен (-)" }
            };

            ViewBag.RhesusFactors = rhesusFactors;

            return View();
        }

        [Authorize(Roles = "Admin, MedicalSpecialist")]
        [HttpPost]
        public async Task<IActionResult> AddBloodDonor(BloodDonorViewModel addBloodDonor)
        {
            if (ModelState.IsValid)
            {
                var existingUser = await _userManager.FindByNameAsync(addBloodDonor.UserName);
                if (existingUser != null)
                {
                    ModelState.AddModelError("UserName", "Това потребителско име вече съществува!");
                    return View("AddBloodDonor", addBloodDonor);
                }

                var existingEmail = await _userManager.FindByEmailAsync(addBloodDonor.Email);
                if (existingEmail != null)
                {
                    ModelState.AddModelError("Email", "Този имейл вече се използва!");
                    return View("AddBloodDonor", addBloodDonor);
                }
                ApplicationUser user = new ApplicationUser()
                {
                    UserName = addBloodDonor.UserName,
                    Email = addBloodDonor.Email,
                    FirstName = addBloodDonor.FirstName,
                    LastName = addBloodDonor.LastName,
                };
                var result = await _userManager.CreateAsync(user, addBloodDonor.Password);

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, Role.Donor.ToString());

                    BloodDonors bloodDonor = new BloodDonors()
                    {
                        User = user,
                        Age = addBloodDonor.Age,
                        BloodGroupId = addBloodDonor.BloodGroupsId,
                        Contacts = addBloodDonor.Contacts,
                        RhesusFactor = addBloodDonor.RhesusFactor,
                    };
                    await _context.BloodDonors.AddAsync(bloodDonor);
                    await _context.SaveChangesAsync();
                }
                ViewData["BloodGroups"] = new SelectList(_context.BloodGroups, "Name", "Name");
                return RedirectToAction("AdminPanel", "Admin");
            }
            return View(addBloodDonor);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult EditBloodDonor(int id)
        {
            var bloodDonor = _context.BloodDonors
                .Include(bd => bd.User) // Зареждаме потребителските данни
                .FirstOrDefault(bd => bd.Id == id);

            if (bloodDonor == null)
            {
                return NotFound();
            }

            var viewModel = new BloodDonorViewModel
            {
                Id = bloodDonor.Id, //???
                UserName = bloodDonor.User.UserName,
                Email = bloodDonor.User.Email,
                FirstName = bloodDonor.User.FirstName,
                LastName = bloodDonor.User.LastName,
                Age = bloodDonor.Age,
                //Password = "CANNOT BE NULL",
                BloodGroupsId = bloodDonor.BloodGroupId,
                RhesusFactor = bloodDonor.RhesusFactor,
                Contacts = bloodDonor.Contacts
            };

            ViewData["BloodGroups"] = new SelectList(_context.BloodGroups, "Id", "Name", bloodDonor.BloodGroupId);

            return View(viewModel);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> EditBloodDonor(BloodDonorViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewData["BloodGroups"] = new SelectList(_context.BloodGroups, "Id", "Name", model.BloodGroupsId);

                return View(model);
            }

            var bloodDonor = _context.BloodDonors
                .Include(bd => bd.User)
                .FirstOrDefault(bd => bd.Id == model.Id);

            if (bloodDonor == null)
            {
                return NotFound();
            }

            //Проверка за съществуващо потребителско име, но не проверяваме текущото
            var existingUserName = await _userManager.FindByNameAsync(model.UserName);
            if (existingUserName != null && existingUserName.Id != bloodDonor.UserId)
            {
                ModelState.AddModelError("UserName", "Това потребителско име вече съществува!");
                return View(model);
            }

            //Проверка за съществуващ имейл, но не проверяваме текущия
            var existingEmail = await _userManager.FindByEmailAsync(model.Email);
            if (existingEmail != null && existingEmail.Id != bloodDonor.UserId)
            {
                ModelState.AddModelError("Email", "Този имейл вече се използва!");
                return View(model);
            }

            // Обновяване на потребителските данни
            bloodDonor.User.UserName = model.UserName;
            bloodDonor.User.Email = model.Email;
            bloodDonor.User.FirstName = model.FirstName;
            bloodDonor.User.LastName = model.LastName;
            if (model.NewPassword != null)
            {
                await _userManager.RemovePasswordAsync(bloodDonor.User);
                await _userManager.AddPasswordAsync(bloodDonor.User, model.NewPassword);

            }

            // Обновяване на кръводарителя
            bloodDonor.Age = model.Age;
            bloodDonor.BloodGroupId = model.BloodGroupsId;
            bloodDonor.RhesusFactor = model.RhesusFactor;
            bloodDonor.Contacts = model.Contacts;

            _context.Users.Update(bloodDonor.User);
            _context.BloodDonors.Update(bloodDonor);
            await _context.SaveChangesAsync();

            return RedirectToAction("AdminPanel", "Admin");
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult DeleteBloodDonor(int id)
        {
            var bloodDonor = _context.BloodDonors
                .Include(bd => bd.User)
                .Include(bd => bd.BloodGroup)
                .FirstOrDefault(bd => bd.Id == id);

            if (bloodDonor == null)
            {
                return NotFound();
            }

            var viewModel = new BloodDonorViewModel
            {
                Id = bloodDonor.Id,
                UserName = bloodDonor.User.UserName,
                Email = bloodDonor.User.Email,
                FirstName = bloodDonor.User.FirstName,
                LastName = bloodDonor.User.LastName,
                Age = bloodDonor.Age,
                BloodGroupsId = bloodDonor.BloodGroupId,
                BloodGroupName = bloodDonor.BloodGroup.Name,
                RhesusFactor = bloodDonor.RhesusFactor,
                Contacts = bloodDonor.Contacts
            };
            return View(viewModel);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var bloodDonor = _context.BloodDonors
                .Include(bd => bd.User)
                .FirstOrDefault(bd => bd.Id == id);

            if (bloodDonor == null)
            {
                return NotFound();
            }

            _context.BloodDonors.Remove(bloodDonor);
            _context.Users.Remove(bloodDonor.User); // Изтриваме и свързания потребител
            await _context.SaveChangesAsync();

            return RedirectToAction("AdminPanel", "Admin");
        }

        [Authorize(Roles = "Donor")]
        public async Task<IActionResult> MyProfile()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Взимаме ID на текущия потребител
            var donor = await _context.BloodDonors
                .Include(d => d.User)
                .Include(d => d.BloodGroup)
                //.Include(d => d.DonationHistory)
                .FirstOrDefaultAsync(d => d.UserId == userId);

            if (donor == null)
            {
                return NotFound("Кръводарителят не е намерен.");
            }

            return View(donor);
        }    }
}
