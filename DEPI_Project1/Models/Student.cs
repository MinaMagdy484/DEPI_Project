using DEPI_Project1.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

public class Student
{
    public int ID { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public string Name { get; set; }

   
   
    [ValidateNever]
    public ICollection<Enrollment> Enrollments { get; set; }
}
