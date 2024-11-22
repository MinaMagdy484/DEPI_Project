using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace DEPI_Project1.Controllers
{
    public class CoursesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CoursesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Courses
        public async Task<IActionResult> Index()
        {
            var courses = await _context.Courses.ToListAsync();
            return View(courses);
        }

        // GET: Courses/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course = await _context.Courses
                .Include(c => c.Instructor)
                .Include(c => c.Lessons)
                .FirstOrDefaultAsync(m => m.ID == id);

            if (course == null)
            {
                return NotFound();
            }

            return View(course);
        }

        // GET: Courses/Create
        public IActionResult Create()
        {
            ViewData["InstructorID"] = new SelectList(_context.Instructors, "ID", "ID");
            return View();
        }

        // POST: Courses/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Name,Description,Date,InstructorID")] Course course)
        {
            if (ModelState.IsValid)
            {
                _context.Add(course);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["InstructorID"] = new SelectList(_context.Instructors, "ID", "ID", course.InstructorID);
            return View(course);
        }

        // GET: Courses/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course = await _context.Courses.FindAsync(id);
            if (course == null)
            {
                return NotFound();
            }
            ViewData["InstructorID"] = new SelectList(_context.Instructors, "ID", "ID", course.InstructorID);
            return View(course);
        }

        // POST: Courses/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Name,Description,Date,InstructorID")] Course course)
        {
            if (id != course.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(course);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CourseExists(course.ID))
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
            ViewData["InstructorID"] = new SelectList(_context.Instructors, "ID", "ID", course.InstructorID);
            return View(course);
        }

        // GET: Courses/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course = await _context.Courses
                .Include(c => c.Instructor)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (course == null)
            {
                return NotFound();
            }

            return View(course);
        }

        // POST: Courses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var course = await _context.Courses.FindAsync(id);
            if (course != null)
            {
                _context.Courses.Remove(course);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        //========================================================================
        [HttpPost]
        [Authorize(Roles = "Student")]
        [ValidateAntiForgeryToken]
        // POST: Courses/Enroll
        public async Task<IActionResult> Enroll(int courseId)
        {
            // Get the currently logged-in student's ID
            var studentEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            var student = await _context.Students.FirstOrDefaultAsync(s => s.Email == studentEmail);

            if (student == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Check if the enrollment already exists
            var enrollmentExists = await _context.Enrollments

                .AnyAsync(e => e.StudentID == student.ID && e.CourseID == courseId);

            if (enrollmentExists)
            {
                // Optionally, you can add a message indicating the student is already enrolled
                TempData["Message"] = "You are already enrolled in this course.";
                return RedirectToAction("Profile", "StudentAdministration");
            }

            // Proceed with creating a new enrollment
            var enrollment = new Enrollment
            {
                StudentID = student.ID,
                CourseID = courseId,
                EnrollmentDate = DateTime.UtcNow // Set the enrollment date
            };

            _context.Enrollments.Add(enrollment);
            await _context.SaveChangesAsync();

            return RedirectToAction("Profile", "StudentAdministration");
        }
        //========================================================================

        // GET: Courses/ViewVideo/5
        public IActionResult ViewVideo(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lesson = _context.Lessons

                .FirstOrDefault(m => m.ID == id);

            if (lesson == null || string.IsNullOrEmpty(lesson.VideoURL))
            {
                return NotFound();
            }

            return Redirect(lesson.VideoURL);
        }

        // GET: Courses/TakeQuiz/5
        public IActionResult TakeQuiz(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var quiz = _context.Lessons
                .FirstOrDefault(m => m.ID == id);

            if (quiz == null)
            {
                return NotFound();
            }

            // Assuming you have a view to display the quiz
            return View(quiz);
        }



        //========================================================================

        private bool CourseExists(int id)
        {
            return _context.Courses.Any(e => e.ID == id);
        }
    }
}