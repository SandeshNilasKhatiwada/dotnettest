using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SydneyFitnessStudio.Models;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace SydneyFitnessStudio.Controllers
{
    public class FitnessClassController : Controller
    {
        private readonly FitnessStudioContext _context;

        public FitnessClassController(FitnessStudioContext context)
        {
            _context = context;
        }

        // Display all classes for clients
        public async Task<IActionResult> Index()
        {
            return View(await _context.FitnessClasses.ToListAsync());
        }

        // Add a new class (For Staff)
        [Authorize(Roles = "Staff")] // Restrict this action to users in the "Staff" role
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Staff")] // Restrict this action to users in the "Staff" role
        public async Task<IActionResult> Create(FitnessClass fitnessClass)
        {
            if (ModelState.IsValid)
            {
                _context.Add(fitnessClass);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(fitnessClass);
        }

        // Edit a class (For Staff) - GET
        [Authorize(Roles = "Staff")] // Restrict this action to users in the "Staff" role
        public async Task<IActionResult> Edit(int id)
        {
            var fitnessClass = await _context.FitnessClasses.FindAsync(id);
            if (fitnessClass == null)
            {
                return NotFound();
            }
            return View(fitnessClass);
        }

        // Edit a class (For Staff) - POST
        [HttpPost]
        [Authorize(Roles = "Staff")] // Restrict this action to users in the "Staff" role
        public async Task<IActionResult> Edit([Bind("FitnessClassId,ClassName,Instructor,Schedule")] FitnessClass fitnessClass)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(fitnessClass);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FitnessClassExists(fitnessClass.FitnessClassId))
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
            return View(fitnessClass);
        }

        // Delete a class (For Staff) - GET
        [Authorize(Roles = "Staff")] // Restrict this action to users in the "Staff" role
        public async Task<IActionResult> Delete(int id)
        {
            var fitnessClass = await _context.FitnessClasses.FindAsync(id);
            if (fitnessClass == null)
            {
                return NotFound();
            }
            return View(fitnessClass);
        }

        // Delete a class (For Staff) - POST
        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Staff")] // Restrict this action to users in the "Staff" role
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var fitnessClass = await _context.FitnessClasses.FindAsync(id);
            if (fitnessClass != null)
            {
                _context.FitnessClasses.Remove(fitnessClass);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool FitnessClassExists(int id)
        {
            return _context.FitnessClasses.Any(e => e.FitnessClassId == id);
        }
    }
}
