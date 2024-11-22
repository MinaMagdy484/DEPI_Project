using DEPI_Project1.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace DEPI_Project1.Controllers
{
    public class AdminAdministrationController : Controller
    {

        private readonly ApplicationDbContext _context;


        public AdminAdministrationController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }

        //[Authorize(Roles = "Admin")]
        //public IActionResult AdminProfile()
        //{
        //    var admin = _context.Admins.FirstOrDefault(a => a.Username == User.Identity.Name);
        //    return View(admin);
        //}


        //[Authorize(Roles = "Admin")]
        public IActionResult Profile()
        {
            var adminEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            var admin = _context.Admins.FirstOrDefault(a => a.Email == adminEmail);
            return View("Profile", admin);
        }

        public IActionResult Control()
        {
            Instroctor_Student_course result = new Instroctor_Student_course();
            result.Instructors = _context.Instructors.ToList();
            result.students = _context.Students.ToList();
            result.Courses = _context.Courses.ToList();
            return View("HomeControler", result);
        }

    }
}
