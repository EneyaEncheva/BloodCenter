﻿using BloodCenter.Data;
using BloodCenter.Enums;
using BloodCenter.Models.ViewModels;
using BloodCenter.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.Metrics;
using Microsoft.AspNetCore.Mvc.Rendering;

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
                Password = "CANNOT BE NULL",
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




        //Медицински специалист
        public IActionResult AddMedicalSpecialist()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> AddMedicalSpecialist(UserViewModel userViewModel)
        {
            if (ModelState.IsValid)
            {

                ApplicationUser user = new ApplicationUser()
                {
                    UserName = userViewModel.UserName,
                    Email = userViewModel.Email,
                    FirstName = userViewModel.FirstName,
                    LastName = userViewModel.LastName,
                };
                var result = await _userManager.CreateAsync(user, userViewModel.Password);

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, Role.MedicalSpecialist.ToString());


                    //await _context.BloodDonors.AddAsync(bloodDonor);
                    await _context.SaveChangesAsync();
                }
                //ViewData["BloodGroups"] = new SelectList(_context.BloodGroups, "Name", "Name");
                return RedirectToAction("AdminPanel", "Admin");
            }
            return View(userViewModel);
        }
    }
}
