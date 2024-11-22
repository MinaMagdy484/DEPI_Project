using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using static System.Formats.Asn1.AsnWriter;

namespace DEPI_Project1.Models
{
    public class DbInitializer
    {
        //private readonly ModelBuilder modelBuilder;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public DbInitializer(ApplicationDbContext context, RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager)
        {
            // this.modelBuilder = modelBuilder;
            _context = context;
            _roleManager = roleManager;
            _userManager = userManager;
        }

        public async Task Seed()
        {

            try
            {
                // Ensure the database is created
                await _context.Database.EnsureCreatedAsync();

                // Check if the Admin role exists
                if (!await _roleManager.RoleExistsAsync("Admin"))
                {
                    await _roleManager.CreateAsync(new IdentityRole("Admin"));
                }
                // Check if no Admin users exist
                if (!_context.Admins.Any())
                {
                    // Create and add a new UserType
                   
                    var passwordHasher = new PasswordHasher<Admin>();
                    var hashedPassword = passwordHasher.HashPassword(null, "Admin123#");

                    
                   await _context.Admins.AddAsync(new Admin
                    {
                        Name = "Admin",
                        Password = hashedPassword,  // In real scenarios, hash the password
                        Email = "Admin@gmail.com",
                        
                   });
                    //add ApplicationUser in database 
                    string userPassword = "Admin123#";
                    var adminUser = new ApplicationUser()
                    {
                        UserName = "Admin",
                        Email = "Admin@gmail.com",
                        NormalizedEmail = "Admin@gmail.com".ToUpper(),
                       
                    };
                    var result = await _userManager.CreateAsync(adminUser, userPassword);
                    if (result.Succeeded)
                    {
                        await _userManager.AddToRoleAsync(adminUser, "Admin");
                    }
                    else
                    {
                        return;
                    }
                    // Save the Admin to the database
                   await _context.SaveChangesAsync();
                }

            }
            catch (Exception)
            {

                throw;
            }

        }
    }
}
