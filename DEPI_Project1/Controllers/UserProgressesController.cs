using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace DEPI_Project1.Controllers
{
    public class UserProgressesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UserProgressesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: UserProgresses
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.UserProgresses.Include(u => u.Enrollment).Include(u => u.Lesson);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: UserProgresses/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userProgress = await _context.UserProgresses
                .Include(u => u.Enrollment)
                .Include(u => u.Lesson)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (userProgress == null)
            {
                return NotFound();
            }

            return View(userProgress);
        }

        // GET: UserProgresses/Create
        public IActionResult Create()
        {
            ViewData["EnrollmentID"] = new SelectList(_context.Enrollments, "ID", "ID");
            ViewData["LessonID"] = new SelectList(_context.Lessons, "ID", "ID");
            return View();
        }

        // POST: UserProgresses/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,EnrollmentID,LessonID,IsCompleted,CompletionDate")] UserProgress userProgress)
        {
            if (ModelState.IsValid)
            {
                _context.Add(userProgress);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["EnrollmentID"] = new SelectList(_context.Enrollments, "ID", "ID", userProgress.EnrollmentID);
            ViewData["LessonID"] = new SelectList(_context.Lessons, "ID", "ID", userProgress.LessonID);
            return View(userProgress);
        }

        // GET: UserProgresses/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userProgress = await _context.UserProgresses.FindAsync(id);
            if (userProgress == null)
            {
                return NotFound();
            }
            ViewData["EnrollmentID"] = new SelectList(_context.Enrollments, "ID", "ID", userProgress.EnrollmentID);
            ViewData["LessonID"] = new SelectList(_context.Lessons, "ID", "ID", userProgress.LessonID);
            return View(userProgress);
        }

        // POST: UserProgresses/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,EnrollmentID,LessonID,IsCompleted,CompletionDate")] UserProgress userProgress)
        {
            if (id != userProgress.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(userProgress);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserProgressExists(userProgress.ID))
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
            ViewData["EnrollmentID"] = new SelectList(_context.Enrollments, "ID", "ID", userProgress.EnrollmentID);
            ViewData["LessonID"] = new SelectList(_context.Lessons, "ID", "ID", userProgress.LessonID);
            return View(userProgress);
        }

        // GET: UserProgresses/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userProgress = await _context.UserProgresses
                .Include(u => u.Enrollment)
                .Include(u => u.Lesson)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (userProgress == null)
            {
                return NotFound();
            }

            return View(userProgress);
        }

        // POST: UserProgresses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userProgress = await _context.UserProgresses.FindAsync(id);
            if (userProgress != null)
            {
                _context.UserProgresses.Remove(userProgress);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserProgressExists(int id)
        {
            return _context.UserProgresses.Any(e => e.ID == id);
        }
    }
}
