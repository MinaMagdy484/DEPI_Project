using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using DEPI_Project1.Models;
using Microsoft.AspNetCore.Identity;
public class ApplicationDbContext :IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // optionsBuilder.UseSqlServer("Server=localhost;Database=DEPI_Project2;Trusted_Connection = True;TrustServerCertificate=True;");
        optionsBuilder.UseSqlServer("Server=localhost;Database=DEPI_Project2;Trusted_Connection = True;TrustServerCertificate=True;");

        base.OnConfiguring(optionsBuilder);
    }
     //public DbSet<UserType> UserTypes { get; set; }
    public DbSet<Course> Courses { get; set; }
    public DbSet<Student> Students { get; set; }
    public DbSet<Instructor> Instructors { get; set; }
    public DbSet<Admin> Admins { get; set; }

    public DbSet<Lesson> Lessons { get; set; }
    public DbSet<Enrollment> Enrollments { get; set; }
    public DbSet<UserProgress> UserProgresses { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        #region Data Seeding
        //new DbInitializer(modelBuilder).Seed();
        modelBuilder.Entity<IdentityRole>().HasData(
                   new IdentityRole()
                   {
                       Id = Guid.NewGuid().ToString(),
                       Name = "Admin",
                       NormalizedName = "Admin".ToUpper(),
                       ConcurrencyStamp = "Admin".ToUpper(),
                   },
                   new IdentityRole()
                   {
                        Id = Guid.NewGuid().ToString(),
                        Name = "User",
                        NormalizedName = "User".ToUpper(),
                        ConcurrencyStamp = "User".ToUpper(),
                   },
                    new IdentityRole()
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = "Student",
                        NormalizedName = "Student".ToUpper(),
                        ConcurrencyStamp = "Student".ToUpper(),
                    },
                     new IdentityRole()
                     {
                         Id = Guid.NewGuid().ToString(),
                         Name = "Instructor",
                         NormalizedName = "Instructor".ToUpper(),
                         ConcurrencyStamp = "Instructor".ToUpper(),
                     }

        );
        #endregion


        



        modelBuilder.Entity<Course>()
            .HasOne(c => c.Instructor)
            .WithMany(i => i.Courses)
            .HasForeignKey(c => c.InstructorID)
            .OnDelete(DeleteBehavior.Cascade); // Cascade is okay here

        modelBuilder.Entity<Enrollment>()
            .HasOne(e => e.Student)
            .WithMany(s => s.Enrollments)
            .HasForeignKey(e => e.StudentID)
            .OnDelete(DeleteBehavior.Cascade); // Cascade is okay here

        modelBuilder.Entity<Enrollment>()
            .HasOne(e => e.Course)
            .WithMany(c => c.Enrollments)
            .HasForeignKey(e => e.CourseID)
            .OnDelete(DeleteBehavior.Cascade); // Cascade is okay here

        modelBuilder.Entity<UserProgress>()
            .HasOne(up => up.Enrollment)
            .WithMany(e => e.UserProgresses)
            .HasForeignKey(up => up.EnrollmentID)
            .OnDelete(DeleteBehavior.Cascade); // Cascade is okay here

        modelBuilder.Entity<UserProgress>()
            .HasOne(up => up.Lesson)
            .WithMany(l => l.UserProgresses)
            .HasForeignKey(up => up.LessonID)
            .OnDelete(DeleteBehavior.Restrict); // Avoid cascade here to prevent multiple paths

        base.OnModelCreating(modelBuilder);
    }

}
