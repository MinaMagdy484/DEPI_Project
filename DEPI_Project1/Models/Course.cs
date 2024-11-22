using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

public class Course
{
    public int ID { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTime Date { get; set; }

    public int InstructorID { get; set; }
    [ValidateNever]
    public Instructor Instructor { get; set; }
    [ValidateNever]

    public ICollection<Lesson> Lessons { get; set; }
    [ValidateNever]

    public ICollection<Enrollment> Enrollments { get; set; }
}
