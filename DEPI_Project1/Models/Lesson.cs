using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

public class Lesson
{
    public int ID { get; set; }
    public string VideoURL { get; set; }
    public string LessonContent { get; set; }
    public int QuizID { get; set; }

    public int CourseID { get; set; }
    [ValidateNever]

    public Course Course { get; set; }
    [ValidateNever]

    public ICollection<UserProgress> UserProgresses { get; set; }
}
