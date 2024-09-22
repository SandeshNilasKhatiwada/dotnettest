using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SydneyFitnessStudio.Models;
using System.Linq;
using System.Threading.Tasks;

namespace SydneyFitnessStudio.Controllers
{
    [Authorize(Roles = "Client,Staff")] // Allow both Clients and Staff to access this controller
    public class ReservationController : Controller
    {
        private readonly FitnessStudioContext _context;

        public ReservationController(FitnessStudioContext context)
        {
            _context = context;
        }

        // Staff-only action to see all reservations
        [Authorize(Roles = "Staff")] // Restrict this action to staff members
        public async Task<IActionResult> AllReservations()
        {
            var reservations = await _context.Reservations
                .Include(r => r.Person) // Include client information
                .Include(r => r.FitnessClass) // Include fitness class information
                .ToListAsync();

            return View(reservations); // Return the view with all reservations
        }

        // POST: Reservation/DeleteAsStaff/5 (Staff can delete any reservation)
        [HttpPost]
        [Authorize(Roles = "Staff")] // Restrict this action to staff members
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAsStaff(int id)
        {
            var reservation = await _context.Reservations.FindAsync(id);

            if (reservation != null)
            {
                _context.Reservations.Remove(reservation);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(AllReservations)); // Redirect staff to AllReservations after deletion
        }

        // Client-only action to view their own reservations
        [Authorize(Roles = "Client")] // Restrict this action to clients
        public async Task<IActionResult> MyReservations()
        {
            var currentUserEmail = User.Identity?.Name;
            var person = await _context.People.FirstOrDefaultAsync(p => p.Email == currentUserEmail);

            if (person == null) return NotFound();

            var reservations = await _context.Reservations
                .Where(r => r.PersonId == person.PersonId) // Filter reservations by the current client's ID
                .Include(r => r.FitnessClass) // Include fitness class information
                .ToListAsync();

            return View(reservations); // Return the view with the client's reservations
        }

        // GET: Reservation/Create (For clients)
        [Authorize(Roles = "Client")]
        public IActionResult Create()
        {
            ViewBag.FitnessClassList = new SelectList(
                _context.FitnessClasses,
                "FitnessClassId",
                "ClassName"
            );
            return View();
        }

        // POST: Reservation/Create (For clients)
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Client")]
        public async Task<IActionResult> Create([Bind("FitnessClassId,ReservationDate")] Reservation reservation)
        {
            if (ModelState.IsValid)
            {
                var currentUserEmail = User.Identity?.Name;
                var person = await _context.People.FirstOrDefaultAsync(p => p.Email == currentUserEmail);

                if (person == null) return NotFound();

                reservation.PersonId = person.PersonId; // Set the client's ID for the reservation

                _context.Add(reservation);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(MyReservations)); // Redirect to the client's reservations page after creation
            }

            ViewBag.FitnessClassList = new SelectList(
                _context.FitnessClasses,
                "FitnessClassId",
                "ClassName",
                reservation.FitnessClassId
            );

            return View(reservation);
        }

        // GET: Reservation/Edit/5 (For clients)
        [Authorize(Roles = "Client")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var reservation = await _context.Reservations
                .Include(r => r.FitnessClass)
                .FirstOrDefaultAsync(r => r.ReservationId == id);

            if (reservation == null) return NotFound();

            ViewBag.FitnessClassList = new SelectList(
                _context.FitnessClasses,
                "FitnessClassId",
                "ClassName",
                reservation.FitnessClassId
            );

            return View(reservation);
        }

        // POST: Reservation/Edit/5 (For clients)
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Client")]
        public async Task<IActionResult> Edit(int id, [Bind("ReservationId,FitnessClassId,ReservationDate")] Reservation reservation)
        {
            if (id != reservation.ReservationId) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(reservation);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReservationExists(reservation.ReservationId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(MyReservations)); // Redirect to the client's reservations page after editing
            }

            ViewBag.FitnessClassList = new SelectList(
                _context.FitnessClasses,
                "FitnessClassId",
                "ClassName",
                reservation.FitnessClassId
            );

            return View(reservation);
        }

        // POST: Reservation/Delete/5 (Clients can delete their own reservations)
        [HttpPost]
        [Authorize(Roles = "Client")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var reservation = await _context.Reservations.FindAsync(id);

            if (reservation != null)
            {
                _context.Reservations.Remove(reservation);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(MyReservations)); // Redirect to the client's reservations page after deletion
        }

        private bool ReservationExists(int id)
        {
            return _context.Reservations.Any(e => e.ReservationId == id);
        }
    }
}
