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

namespace BloodCenter.Controllers
{
    //[Authorize(Roles = "Admin")]
    //[Authorize(Roles = "MedicalSpecialist")]
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
        public IActionResult AdminPanel()
        {
            var bloodDonors = _context.BloodDonors
                .Include(x => x.User)
                .Include(x => x.BloodGroup)
                .ToList();

            return View(bloodDonors);
        }

        public IActionResult AddBloodDonor()
        {
            ViewData["BloodGroups"] = new SelectList(_context.BloodGroups, "Id", "Name");

            return View();
        }

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


        //История на даренията

        [HttpGet]
        public async Task<IActionResult> History()
        {
            var donations = await _context.DonationHistories
                .Include(d => d.BloodDonor)
                .ToListAsync();

            ViewData["BloodDonors"] = new SelectList(_context.BloodDonors, "Id", "User.FirstName");
            return View(donations);
        }

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

        //[HttpGet]
        //public IActionResult AddDonation()
        //{
        //    ViewData["BloodDonors"] = new SelectList(_context.BloodDonors, "Id", "User.FirstName");
        //    return View();
        //}

        [HttpGet]
        public IActionResult AddDonation()
        {
            ViewData["BloodDonors"] = new SelectList(_context.BloodDonors.Include(d => d.User), "Id", "User.FirstName");
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> AddDonation(DonationViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewData["Donors"] = new SelectList(_context.BloodDonors.Include(d => d.User), "Id", "User.FirstName");
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
                DonationDate = DateTime.Now, // Автоматично попълване на датата
                Quantity = model.Quantity
            };

            _context.DonationHistories.Add(donation);
            await _context.SaveChangesAsync();

            return RedirectToAction("History"); // Пренасочване към списъка с дарения
        }


        //[HttpPost]
        //public async Task<IActionResult> AddDonation(DonationViewModel model)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        ViewData["BloodDonors"] = new SelectList(_context.BloodDonors, "Id", "User.FirstName");
        //        return View(model);
        //    }
        //    if (ModelState.IsValid)
        //    {
        //        var donor = await _context.BloodDonors.FindAsync(model.BloodDonorId);
        //        if (donor == null)
        //        {
        //            return NotFound();
        //        }

        //        var donation = new DonationHistory
        //        {
        //            BloodDonorId = donor.Id,
        //            Quantity = model.Quantity,
        //            DonationDate = DateTime.Now
        //        };

        //        await _context.DonationHistories.AddAsync(donation);
        //        await _context.SaveChangesAsync();

        //        // Взимаме кръвната група и резус-фактора от дарителя
        //        await UpdateBloodSupply(donor.BloodGroupId, donor.RhesusFactor, model.Quantity);

        //        return RedirectToAction("History");
        //    }
        //    return View(model);
        //}


        public async Task<IActionResult> BloodInventory()
        {
            var supplies = await _context.Supplies.Include(s => s.BloodGroup).ToListAsync();
            return View(supplies);
        }

        private async Task UpdateBloodSupply(int bloodGroupId, char rhesusFactor, double quantity)
        {
            var existingSupply = await _context.Supplies
                .FirstOrDefaultAsync(s => s.BloodGroupId == bloodGroupId && s.RhesusFactor == rhesusFactor);

            if (existingSupply != null)
            {
                existingSupply.Quantity += quantity;
                existingSupply.LastUpdated = DateTime.Now;
            }
            else
            {
                var newSupply = new Supply
                {
                    BloodGroupId = bloodGroupId,
                    RhesusFactor = rhesusFactor,
                    Quantity = quantity,
                    LastUpdated = DateTime.Now
                };

                await _context.Supplies.AddAsync(newSupply);
            }

            await _context.SaveChangesAsync();
        }


        //Медицински специалист
        public async Task<IActionResult> MedicalSpecialist()
        {
            var model = await _context.Users
                .Where(u => _context.UserRoles
                .Any(ur => ur.UserId == u.Id && _context.Roles
                .Any(r => r.Id == ur.RoleId && r.Name == "MedicalSpecialist")))
                .ToListAsync();
            return View(model);
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
                //var existingUser = await _userManager.FindByNameAsync(addMedicalSpecialist.UserName);
                //if (existingUser != null)
                //{
                //    ModelState.AddModelError("UserName", "Това потребителско име вече съществува!");
                //    return View("MedicalSpecialist/AddMedicalSpecialist", addMedicalSpecialist);
                //}
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
                return RedirectToAction("MedicalSpecialist", "Admin");
            }
            return View(addMedicalSpecialist);
        }

        [HttpGet]
        public async Task<IActionResult> EditMedicalSpecialist(string id)
        {
            //var medicalSpecialist = _context.Users.FirstOrDefault(x => x.Id == id);

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
                    return RedirectToAction("MedicalSpecialist", "Admin");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }

            return View(model);
        }

    }
}
