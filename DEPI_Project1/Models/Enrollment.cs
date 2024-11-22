using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

public class Enrollment
{
    public int ID { get; set; }
    public int StudentID { get; set; }
    [ValidateNever]

    public Student Student { get; set; }

    public int CourseID { get; set; }
    [ValidateNever]

    public Course Course { get; set; }

    public DateTime EnrollmentDate { get; set; }
    [ValidateNever]


    public ICollection<UserProgress> UserProgresses { get; set; }
}
