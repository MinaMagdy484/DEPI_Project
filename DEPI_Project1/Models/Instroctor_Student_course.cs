using Microsoft.EntityFrameworkCore;

namespace DEPI_Project1.Models
{
    public class Instroctor_Student_course : DbContext
    {
        public List<Student> students { get; set; }
        public List<Instructor> Instructors { get; set; }

        public List<Course> Courses { get; set; }
    }
}
