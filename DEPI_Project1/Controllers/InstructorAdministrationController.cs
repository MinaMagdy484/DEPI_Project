using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace DEPI_Project1.Controllers
{
    public class InstructorAdministrationController : Controller
    {


        private readonly ApplicationDbContext _context;
        public InstructorAdministrationController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }

        //[Authorize(Roles = "Instructor")]
        //public IActionResult InstructorProfile()
        //{
        //    var instructor = _context.Instructors.FirstOrDefault(i => i.Email == User.Identity.Name);
        //    return View(instructor);
        //}

        //[Authorize(Roles = "Instructor")]
        public async Task<IActionResult> Profile()
        {
            var instructorEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(instructorEmail))
            {
                return RedirectToAction("Login", "Account");
            }

            var instructor = await _context.Instructors
                .Include(i => i.Courses)
                .FirstOrDefaultAsync(i => i.Email == instructorEmail);

            if (instructor == null)
            {
                return NotFound();
            }

            return View(instructor);  // Use the default View name "Profile"
        }






        // GET: Courses/Details/5
        public async Task<IActionResult> CourseDetails(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course = await _context.Courses
                .Include(c => c.Instructor)
                .Include(c => c.Lessons)  // Including lessons within the course
                .FirstOrDefaultAsync(m => m.ID == id);

            if (course == null)
            {
                return NotFound();
            }

            return View(course);
        }

        // GET: InstructorAdministration/Courses
        public async Task<IActionResult> Courses()
        {
            var instructorEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            var instructor = await _context.Instructors.FirstOrDefaultAsync(i => i.Email == instructorEmail);

            if (instructor == null)
            {
                return NotFound();
            }

            var courses = await _context.Courses
                .Include(c => c.Instructor)
                .Where(c => c.InstructorID == instructor.ID)  // Filter courses by instructor
                .ToListAsync();

            return View(courses);
        }

        // GET: InstructorAdministration/Courses/Create
        public IActionResult CreateCourse()
        {
            return View();
        }

        // POST: InstructorAdministration/Courses/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCourse([Bind("ID,Name,Description,Date")] Course course)
        {
            if (ModelState.IsValid)
            {
                var instructorEmail = User.FindFirst(ClaimTypes.Email)?.Value;
                var instructor = await _context.Instructors.FirstOrDefaultAsync(i => i.Email == instructorEmail);

                if (instructor == null)
                {
                    return Unauthorized();  // If the instructor is not found
                }

                course.InstructorID = instructor.ID;  // Set the InstructorID

                _context.Add(course);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Courses));
            }
            return View(course);
        }

        // GET: InstructorAdministration/Courses/Edit/5
        public async Task<IActionResult> EditCourse(int? id)
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

            return View(course);
        }

        // POST: InstructorAdministration/Courses/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditCourse(int id, [Bind("ID,Name,Description,Date,InstructorID")] Course course)
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
                    throw;
                }
                return RedirectToAction(nameof(Courses));
            }

            return View(course);
        }

        // GET: InstructorAdministration/Courses/Delete/5
        public async Task<IActionResult> DeleteCourse(int? id)
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

        // POST: InstructorAdministration/Courses/Delete/5
        [HttpPost, ActionName("DeleteCourse")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCourseConfirmed(int id)
        {
            var course = await _context.Courses.FindAsync(id);
            if (course != null)
            {
                _context.Courses.Remove(course);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Courses));
        }

        private bool CourseExists(int id)
        {
            return _context.Courses.Any(e => e.ID == id);
        }

        // GET: InstructorAdministration/Lessons
        public async Task<IActionResult> Lessons(int courseId)
        {
            var lessons = await _context.Lessons
                .Include(l => l.Course)
                .Where(l => l.CourseID == courseId)  // Filter lessons by course
                .ToListAsync();

            ViewBag.CourseID = courseId;  // Use ViewBag for a simpler view model
            return View(lessons);
        }

        // GET: InstructorAdministration/Lessons/Create
        public IActionResult CreateLesson(int courseId)
        {
            ViewBag.CourseID = courseId;  // Pass CourseID using ViewBag
            return View();
        }

        // POST: InstructorAdministration/Lessons/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateLesson([Bind("ID,VideoURL,LessonContent,QuizID,CourseID")] Lesson lesson)
        {
            if (ModelState.IsValid)
            {
                if (lesson.QuizID > 0)
                {
                    _context.Add(lesson);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("CourseDetails", "InstructorAdministration", new { id = lesson.CourseID }); // Redirect to CourseDetails
                }
                else
                {
                    //error message
                    ModelState.AddModelError("QuizID", "Enter Number Bigger Than Zero");
                }
            }

            ViewData["CourseID"] = new SelectList(_context.Courses, "ID", "Name", lesson.CourseID);
            return View(lesson);
        }


        // GET: InstructorAdministration/Lessons/Edit/5
        public async Task<IActionResult> EditLesson(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lesson = await _context.Lessons.FindAsync(id);
            if (lesson == null)
            {
                return NotFound();
            }

            ViewBag.CourseID = lesson.CourseID;  // Set ViewBag for CourseID
            return View(lesson);
        }

        // POST: InstructorAdministration/Lessons/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditLesson(int id, [Bind("ID,VideoURL,LessonContent,QuizID,CourseID")] Lesson lesson)
        {
            if (id != lesson.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                if (lesson.QuizID > 0)
                {
                    try
                    {
                        _context.Update(lesson);
                        await _context.SaveChangesAsync();
                        return RedirectToAction("CourseDetails", "InstructorAdministration", new { id = lesson.CourseID }); // Redirect to CourseDetails
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!LessonExists(lesson.ID))
                        {
                            return NotFound();
                        }
                        throw;
                    }
                }
                else
                {
                    //error message
                    ModelState.AddModelError("QuizID", "Enter Number Bigger Than Zero");
                }
            }

            ViewData["CourseID"] = new SelectList(_context.Courses, "ID", "Name", lesson.CourseID);
            return View(lesson);
        }


        // GET: InstructorAdministration/Lessons/Delete/5
        public async Task<IActionResult> DeleteLesson(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lesson = await _context.Lessons
                .Include(l => l.Course)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (lesson == null)
            {
                return NotFound();
            }

            return View(lesson);
        }

        // POST: InstructorAdministration/Lessons/Delete/5
        [HttpPost, ActionName("DeleteLesson")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteLessonConfirmed(int id)
        {
            var lesson = await _context.Lessons.FindAsync(id);
            int courseId = lesson.CourseID; // Get the CourseID before deletion
            if (lesson != null)
            {
                _context.Lessons.Remove(lesson);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("CourseDetails", "InstructorAdministration", new { id = courseId }); // Redirect to CourseDetails
        }

        private bool LessonExists(int id)
        {
            return _context.Lessons.Any(e => e.ID == id);
        }



    }
}
