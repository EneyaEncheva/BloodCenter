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
    //{
    //    public class RequestsController : Controller
    //    {
    //        private readonly ApplicationDbContext _context;

    //        public RequestsController(ApplicationDbContext context)
    //        {
    //            _context = context;
    //        }

    //        // GET: Requests
    //        public async Task<IActionResult> Index()
    //        {
    //            return View(await _context.Requests.ToListAsync());
    //        }

    //        // GET: Requests/Create
    //        public IActionResult Create()
    //        {
    //            return View();
    //        }

    //        // POST: Requests/Create
    //        // To protect from overposting attacks, enable the specific properties you want to bind to.
    //        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    //        [HttpPost]
    //        [ValidateAntiForgeryToken]
    //        public async Task<IActionResult> Create([Bind("Id,Hospital,BloodGroupsId,Quantity,Date,IsAvailable")] Requests requests)
    //        {
    //            if (ModelState.IsValid)
    //            {
    //                _context.Add(requests);
    //                await _context.SaveChangesAsync();
    //                return RedirectToAction(nameof(Index));
    //            }
    //            return View(requests);
    //        }

    //        // GET: Requests/Edit/5
    //        public async Task<IActionResult> Edit(int? id)
    //        {
    //            if (id == null)
    //            {
    //                return NotFound();
    //            }

    //            var requests = await _context.Requests.FindAsync(id);
    //            if (requests == null)
    //            {
    //                return NotFound();
    //            }
    //            return View(requests);
    //        }

    //        // POST: Requests/Edit/5
    //        // To protect from overposting attacks, enable the specific properties you want to bind to.
    //        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    //        [HttpPost]
    //        [ValidateAntiForgeryToken]
    //        public async Task<IActionResult> Edit(int id, [Bind("Id,Hospital,BloodGroupsId,Quantity,Date,IsAvailable")] Requests requests)
    //        {
    //            if (id != requests.Id)
    //            {
    //                return NotFound();
    //            }

    //            if (ModelState.IsValid)
    //            {
    //                try
    //                {
    //                    _context.Update(requests);
    //                    await _context.SaveChangesAsync();
    //                }
    //                catch (DbUpdateConcurrencyException)
    //                {
    //                    if (!RequestsExists(requests.Id))
    //                    {
    //                        return NotFound();
    //                    }
    //                    else
    //                    {
    //                        throw;
    //                    }
    //                }
    //                return RedirectToAction(nameof(Index));
    //            }
    //            return View(requests);
    //        }

    //        // GET: Requests/Delete/5
    //        public async Task<IActionResult> Delete(int? id)
    //        {
    //            if (id == null)
    //            {
    //                return NotFound();
    //            }

    //            var requests = await _context.Requests
    //                .FirstOrDefaultAsync(m => m.Id == id);
    //            if (requests == null)
    //            {
    //                return NotFound();
    //            }

    //            return View(requests);
    //        }

    //        // POST: Requests/Delete/5
    //        [HttpPost, ActionName("Delete")]
    //        [ValidateAntiForgeryToken]
    //        public async Task<IActionResult> DeleteConfirmed(int id)
    //        {
    //            var requests = await _context.Requests.FindAsync(id);
    //            if (requests != null)
    //            {
    //                _context.Requests.Remove(requests);
    //            }

    //            await _context.SaveChangesAsync();
    //            return RedirectToAction(nameof(Index));
    //        }

    //        private bool RequestsExists(int id)
    //        {
    //            return _context.Requests.Any(e => e.Id == id);
    //        }
    //    }
    //}

    //[Authorize(Roles = "MedicalSpecialist,Admin")]
    public class RequestsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RequestsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var requests = await _context.Requests
                .Include(r => r.BloodGroup) // Включваме кръвната група
                .ToListAsync();
            return View(await _context.Requests.ToListAsync());
        }

        // Медицинско лице подава заявка
        //[Authorize(Roles = "MedicalSpecialist")]
        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.BloodGroups = new SelectList(_context.BloodGroups, "Id", "Name");

            var rhesusFactors = new List<SelectListItem>
            {
                new SelectListItem { Value = "+", Text = "Положителен (+)" },
                new SelectListItem { Value = "-", Text = "Отрицателен (-)" }
            };

            ViewBag.RhesusFactors = rhesusFactors;
            return View();
        }

        //[Authorize(Roles = "MedicalSpecialist")]
        [HttpPost]
        public async Task<IActionResult> Create(Requests request)
        {
            if (ModelState.IsValid)
            {
                request.RequestedById = User.FindFirstValue(ClaimTypes.NameIdentifier);
                request.Status = "В процес на изпълнение";

                _context.Requests.Add(request);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index");
            }

            ViewBag.BloodGroups = new SelectList(_context.BloodGroups, "Id", "Name");
            return View(request);
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
                var supply = await _context.Supplies.FirstOrDefaultAsync(s => s.BloodGroupId == request.BloodGroupId);
                if (supply != null && supply.Quantity >= request.Quantity)
                {
                    supply.Quantity -= request.Quantity;
                    request.Status = "Одобрена";
                    request.ExecutionDate = DateTime.Now;
                }
                else
                {
                    request.Status = "Няма достатъчно кръв";
                    request.ExecutionDate = DateTime.Now;
                }
            }
            else
            {
                request.Status = "Отхвърлена";
                request.ExecutionDate = DateTime.Now;
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        //[HttpGet]
        //public async Task<IActionResult> Edit(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var requests = await _context.Requests.FindAsync(id);
        //    if (requests == null)
        //    {
        //        return NotFound();
        //    }
        //    return View(requests);
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(int id, [Bind("Id,Hospital,BloodGroupsId,Quantity,Date,IsAvailable")] Requests requests)
        //{
        //    if (id != requests.Id)
        //    {
        //        return NotFound();
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            _context.Update(requests);
        //            await _context.SaveChangesAsync();
        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            if (!RequestsExists(requests.Id))
        //            {
        //                return NotFound();
        //            }
        //            else
        //            {
        //                throw;
        //            }
        //        }
        //        return RedirectToAction(nameof(Index));
        //    }
        //    return View(requests);
        //}

        public async Task<IActionResult> Delete(int? id)
        {
            var request = await _context.Requests
                .Include(r => r.BloodGroup) // Включи кръвната група, ако се използва във View-то
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
