using BloodCenter.Data;
using BloodCenter.Enums;
using BloodCenter.Models;
using BloodCenter.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace BloodCenter.Controllers
{
    [Authorize(Roles = "Admin")]
    public class MedicalSpecialistController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ApplicationDbContext _context;

        public MedicalSpecialistController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        public async Task<IActionResult> MedicalSpecialist(string searched)
        {
            var model = _context.Users
                .Where(u => _context.UserRoles
                .Any(ur => ur.UserId == u.Id && _context.Roles
                .Any(r => r.Id == ur.RoleId && r.Name == "MedicalSpecialist")));

            if (!string.IsNullOrEmpty(searched))
            {
                model = model.Where(bd =>
                    bd.FirstName.Contains(searched) ||
                    bd.LastName.Contains(searched));
            }

            ViewData["Searched"] = searched;
            return View(await model.ToListAsync());
        }
        public IActionResult AddMedicalSpecialist()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddMedicalSpecialist(UserViewModel addMedicalSpecialist)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser user = new ApplicationUser()
                {
                    UserName = addMedicalSpecialist.UserName,
                    Email = addMedicalSpecialist.Email,
                    FirstName = addMedicalSpecialist.FirstName,
                    LastName = addMedicalSpecialist.LastName,
                };
                var result = await _userManager.CreateAsync(user, addMedicalSpecialist.Password);

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, Role.MedicalSpecialist.ToString());
                    await _context.SaveChangesAsync();
                }
                return RedirectToAction("MedicalSpecialist");
            }
            return View(addMedicalSpecialist);
        }

        [HttpGet]
        public async Task<IActionResult> EditMedicalSpecialist(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(id);

            if (user == null || !await _userManager.IsInRoleAsync(user, Role.MedicalSpecialist.ToString()))
            {
                return NotFound();
            }

            var model = new UserViewModel
            {
                UserName = user.UserName,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditMedicalSpecialist(string id, UserViewModel model)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null || !await _userManager.IsInRoleAsync(user, Role.MedicalSpecialist.ToString()))
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                user.UserName = model.UserName;
                user.Email = model.Email;
                user.FirstName = model.FirstName;
                user.LastName = model.LastName;

                var result = await _userManager.UpdateAsync(user);

                if (result.Succeeded)
                {
                    return RedirectToAction("MedicalSpecialist");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> DeleteMedicalSpecialist(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                return NotFound("Потребителят не е намерен.");
            }
            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteConfirmedMS(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                return NotFound("Потребителят не е намерен.");
            }

            var result = await _userManager.DeleteAsync(user);

            if (!result.Succeeded)
            {
                return BadRequest("Възникна грешка при изтриването на потребителя.");
            }

            return RedirectToAction("MedicalSpecialist");
        }
    }
}
