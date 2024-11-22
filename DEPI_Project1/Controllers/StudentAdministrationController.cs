using DEPI_Project1.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using static DEPI_Project1.Controllers.AccountController;
namespace DEPI_Project1.Controllers
{
    //[Authorize(Roles = "Student")]
    public class StudentAdministrationController : Controller
    {
        private readonly ApplicationDbContext _context;

        public StudentAdministrationController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Profile()
        {
            var studentEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(studentEmail))
            {
                return RedirectToAction("Login", "Account");
            }

            var student = _context.Students.FirstOrDefault(s => s.Email == studentEmail);
            if (student == null)
            {
                // Optionally handle the case where the student is not found
                return NotFound();
            }

            return View("Profile", student);
        }


        public IActionResult EnrollCourse()
        {

            IDCourseListViewModel coursesVM = new IDCourseListViewModel();
            coursesVM.Courses = new SelectList(_context.Courses.ToList(), "ID", "Name");
            return View(coursesVM);
        }
        public IActionResult SaveEnroll(int CourseID, int id, IDCourseListViewModel courseFromRequest)
        {
            if (CourseID != 0)
            {
                var studentEmail = User.FindFirst(ClaimTypes.Email)?.Value;
                Student student = _context.Students.FirstOrDefault(s => s.Email == studentEmail);
                Enrollment enrollment = new Enrollment();
                enrollment.CourseID = CourseID;
                enrollment.StudentID = student.ID;
                enrollment.EnrollmentDate = DateTime.Now;
                _context.Enrollments.Add(enrollment);
                _context.SaveChanges();
                return RedirectToAction("Courses");
            }
            courseFromRequest.Courses = new SelectList(_context.Courses.ToList(), "ID", "Name");
            return View("EnrollCourse", courseFromRequest);
        }

        //=================================================================
        public IActionResult Courses()
        {
            var studentEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            var student = _context.Students
                .Include(s => s.Enrollments)
                .ThenInclude(e => e.Course)
                .ThenInclude(c => c.Instructor)
                .FirstOrDefault(s => s.Email == studentEmail);

            if (student == null)
            {
                return RedirectToAction("Login", "Account");
            }

            return View(student.Enrollments.Select(e => e.Course).ToList());
        }

        public IActionResult CourseDetails(int id)
        {
            var course = _context.Courses
                .Include(c => c.Instructor)
                .Include(c => c.Lessons)
                .FirstOrDefault(c => c.ID == id);

            if (course == null)
            {
                return NotFound();
            }

            return View(course);
        }

        //=================================================================



    }
}


//[Authorize(Roles = "Student")]
//public IActionResult StudentProfile()
//{
//    var student = _context.Students.FirstOrDefault(s => s.Email == User.Identity.Name);
//    return View(student);
//}