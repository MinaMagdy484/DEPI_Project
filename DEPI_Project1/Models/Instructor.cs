using DEPI_Project1.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

public class Instructor
{
    public int ID { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public string Name { get; set; }


    [ValidateNever]
    public ICollection<Course> Courses { get; set; }
}
