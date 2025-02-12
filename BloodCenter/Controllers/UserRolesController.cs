using BloodCenter.Data;
using BloodCenter.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BloodCenter.Controllers;

namespace BloodCenter.Controllers
{
    public class UserRolesController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserRolesController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager; ;
        }

        public async Task<IActionResult> Index()
        {
            List<ApplicationUser> users = await _userManager.Users.ToListAsync();
            List<UserRolesViewModel> userRolesViewModels = new();

            foreach(ApplicationUser user in users)
            {
                UserRolesViewModel userRolesViewModel = new()
                {
                    UserId = user.Id,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Roles = await _userManager.GetRolesAsync(user)
                };

                userRolesViewModels.Add(userRolesViewModel);
            }
            return View(userRolesViewModels);
        }

        [HttpGet]
        public async Task<IActionResult> Manage(string userId)
        {
            ApplicationUser user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                ViewBag.ErrorMessage = $"Потребител с Id = {userId} не може да бъде намерен.";
                return View("Не е намерен");
            }
             ViewBag.UserName = user.UserName;

            //????
            IdentityRole[] roles = _roleManager.Roles.ToArray();

            ManageUserRoleViewModel userRolesViewModel = new()
            {
                RoleIds = new string[roles.Length],
                RoleName = new string[roles.Length]
            };

            for (int index = 0; index < roles.Length; index++)
            {
                IdentityRole identityRole = roles[index];

                userRolesViewModel.RoleIds[index] = identityRole.Id;
                userRolesViewModel.RoleName[index] = identityRole.Name;
            }

            userRolesViewModel.SelectedRole = (await _userManager.GetRolesAsync(user)).First();
            return View(userRolesViewModel);
        }
    }
}
