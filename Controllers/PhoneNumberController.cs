
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MVC_4Point1.Models;

namespace MVC_4Point1.Controllers
{
    public class PhoneNumberController : Controller
    {
        private readonly PersonContext _context;

        public PhoneNumberController(PersonContext context)
        {
            _context = context;
        }

        // GET: PhoneNumber
        public async Task<IActionResult> Index()
        {
            var personContext = _context.PhoneNumbers.Include(p => p.Person);
            return View(await personContext.ToListAsync());
        }

        // GET: PhoneNumber/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var phoneNumber = await _context.PhoneNumbers
                .Include(p => p.Person)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (phoneNumber == null)
            {
                return NotFound();
            }

            return View(phoneNumber);
        }

        // GET: PhoneNumber/Create
        public IActionResult Create()
        {
            ViewData["PersonID"] = new SelectList(_context.People, "ID", "FirstName");
            return View();
        }

        // POST: PhoneNumber/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Number,PersonID")] PhoneNumber phoneNumber)
        {
            if (ModelState.IsValid)
            {
                _context.Add(phoneNumber);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["PersonID"] = new SelectList(_context.People, "ID", "FirstName", phoneNumber.PersonID);
            return View(phoneNumber);
        }

        // GET: PhoneNumber/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var phoneNumber = await _context.PhoneNumbers.FindAsync(id);
            if (phoneNumber == null)
            {
                return NotFound();
            }
            ViewData["PersonID"] = new SelectList(_context.People, "ID", "FirstName", phoneNumber.PersonID);
            return View(phoneNumber);
        }

        // POST: PhoneNumber/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Number,PersonID")] PhoneNumber phoneNumber)
        {
            if (id != phoneNumber.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(phoneNumber);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PhoneNumberExists(phoneNumber.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["PersonID"] = new SelectList(_context.People, "ID", "FirstName", phoneNumber.PersonID);
            return View(phoneNumber);
        }

        // GET: PhoneNumber/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var phoneNumber = await _context.PhoneNumbers
                .Include(p => p.Person)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (phoneNumber == null)
            {
                return NotFound();
            }

            return View(phoneNumber);
        }

        // POST: PhoneNumber/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var phoneNumber = await _context.PhoneNumbers.FindAsync(id);
            _context.PhoneNumbers.Remove(phoneNumber);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PhoneNumberExists(int id)
        {
            return _context.PhoneNumbers.Any(e => e.ID == id);
        }
    }
}
