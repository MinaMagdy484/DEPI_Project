using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

public class UserProgress
{
    public int ID { get; set; }
    public int EnrollmentID { get; set; }
    [ValidateNever]

    public Enrollment Enrollment { get; set; }

    public int LessonID { get; set; }
    [ValidateNever]

    public Lesson Lesson { get; set; }

    public bool IsCompleted { get; set; }
    public DateTime? CompletionDate { get; set; }
}


