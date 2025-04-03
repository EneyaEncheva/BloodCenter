using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BloodCenter.Data;
using BloodCenter.Models;
using Azure.Core;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace BloodCenter.Controllers
{
    //[Authorize(Roles = "MedicalSpecialist,Admin")]
    public class RequestsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RequestsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int? bloodGroupId, char? rhesusFactor, DateTime? startDate, DateTime? endDate, string status)
        {
            var requests = _context.Requests
                .Include(r => r.BloodGroup)
                .AsQueryable();

            // Филтриране
            if (bloodGroupId.HasValue && bloodGroupId > 0)
            {
                requests = requests.Where(r => r.BloodGroupId == bloodGroupId);
            }

            if (rhesusFactor.HasValue)
            {
                requests = requests.Where(r => r.RhesusFactor == rhesusFactor.Value);
            }

            if (startDate.HasValue)
            {
                requests = requests.Where(r => r.Date.Date >= startDate.Value.Date);
            }

            if (endDate.HasValue)
            {
                requests = requests.Where(r => r.Date.Date <= endDate.Value.Date);
            }

            if (!string.IsNullOrEmpty(status))
            {
                requests = requests.Where(r => r.Status == status);
            }

            ViewData["BloodGroups"] = new SelectList(_context.BloodGroups, "Id", "Name");

            ViewData["BloodGroupId"] = bloodGroupId;
            ViewData["RhesusFactor"] = rhesusFactor;
            ViewData["StartDate"] = startDate?.ToString("yyyy-MM-dd");
            ViewData["EndDate"] = endDate?.ToString("yyyy-MM-dd");
            ViewData["Status"] = status;

            return View(await requests.ToListAsync());
        }

        //[Authorize(Roles = "MedicalSpecialist")]
        [HttpGet]
        public IActionResult Add()
        {
            ViewBag.BloodGroups = new SelectList(_context.BloodGroups, "Id", "Name");

            var rhesusFactors = new List<SelectListItem>
    {
        new SelectListItem { Value = "+", Text = "Положителен (+)" },
        new SelectListItem { Value = "-", Text = "Отрицателен (-)" }
    };

            ViewBag.RhesusFactors = rhesusFactors;
            return View(new Requests());
        }

        [HttpPost]
        public async Task<IActionResult> Add(Requests request)
        {
            if (!ModelState.IsValid)
            {
                // Ако има грешки във валидацията, презареди падащите менюта
                ViewBag.BloodGroups = new SelectList(_context.BloodGroups, "Id", "Name");

                ViewBag.RhesusFactors = new List<SelectListItem>
        {
            new SelectListItem { Value = "+", Text = "Положителен (+)" },
            new SelectListItem { Value = "-", Text = "Отрицателен (-)" }
        };

                return View(request);
            }

            request.RequestedById = User.FindFirstValue(ClaimTypes.NameIdentifier);
            request.Status = "В процес на изпълнение";

            _context.Requests.Add(request);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }


        //[Authorize(Roles = "MedicalSpecialist")]
        public async Task<IActionResult> ApproveRequests()
        {
            var requests = await _context.Requests
                .Include(r => r.BloodGroup)
                .ToListAsync();
            return View(requests);
        }

        //[Authorize(Roles = "MedicalSpecialist")]
        [HttpPost]
        public async Task<IActionResult> UpdateStatus(int id, string status)
        {
            var request = await _context.Requests.FindAsync(id);
            if (request == null) return NotFound();

            if (status == "Одобрена")
            {
                var supply = await _context.Availabilities
                    .FirstOrDefaultAsync(s => s.BloodGroupId == request.BloodGroupId && s.RhesusFactor == request.RhesusFactor);

                if (supply != null && supply.Quantity >= request.Quantity)
                {
                    // Ако има достатъчно кръв от търсената група, намаляваме от нея
                    supply.Quantity -= request.Quantity;
                    request.Status = "Одобрена";
                }
                else
                {
                    // Ако няма достатъчно кръв от търсената група, проверяваме 0-
                    var universalDonor = await _context.Availabilities
                        .FirstOrDefaultAsync(s => s.BloodGroup.Name == "0" && s.RhesusFactor == '-');

                    if (universalDonor != null && universalDonor.Quantity >= request.Quantity)
                    {
                        // Използваме 0- кръв, ако е налична
                        universalDonor.Quantity -= request.Quantity;
                        request.Status = "Одобрена (използвана 0-)";
                    }
                    else
                    {
                        request.Status = "Няма достатъчно кръв";
                    }
                }

                request.ExecutionDate = DateTime.Now;
            }
            else
            {
                request.Status = "Отхвърлена";
                request.ExecutionDate = DateTime.Now;
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }


        public async Task<IActionResult> Delete(int? id)
        {
            var request = await _context.Requests
                .Include(r => r.BloodGroup)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (request == null)
            {
                return NotFound();
            }

            return View(request);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var requests = await _context.Requests.FindAsync(id);
            if (requests != null)
            {
                _context.Requests.Remove(requests);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RequestsExists(int id)
        {
            return _context.Requests.Any(e => e.Id == id);
        }
    }
}
