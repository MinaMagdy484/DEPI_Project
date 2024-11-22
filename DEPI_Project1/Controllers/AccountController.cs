using DEPI_Project1.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.AspNetCore.Session;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using DEPI_Project1.Models;
using Microsoft.AspNetCore.Identity;
using System.Security.Cryptography;
using System;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
// --------------------------- 6/10
namespace DEPI_Project1.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, ApplicationDbContext context, RoleManager<IdentityRole> roleManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            _context = context;
            _roleManager = roleManager;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task <IActionResult> Register(RegisterViewModel model)
        {          
            if (ModelState.IsValid)
            {
                ApplicationUser appUser = new ApplicationUser();

                appUser.Email = model.Email;
                appUser.UserName=model.Name;
                appUser.PasswordHash = model.Password;
                

                //await userManager.CreateAsync(appUser);
                var result = await userManager.CreateAsync(appUser,model.Password);
                if (result.Succeeded)
                {
                     await userManager.AddToRoleAsync(appUser, "User");
                    await signInManager.SignInAsync(appUser, false);
                  
                    if (model.UserType == "Student")
                    {
                        await userManager.AddToRoleAsync(appUser, "Student");
                       
                        var student = new Student()
                        {
                            Email = model.Email,
                            Name = model.Name,
                            Password = HashPassword(model.Password),
                           

                        };
                        await _context.Students.AddAsync(student);
                        //await _context.SaveChangesAsync();
                    }
                    else
                    {
                        await userManager.AddToRoleAsync(appUser, "Instructor");
                        
                        var instructor = new Instructor()
                        {
                            Email = model.Email,
                            Name = model.Name,
                            Password = HashPassword(model.Password),
                            

                        };
                        await _context.Instructors.AddAsync(instructor);
                    }
                     await _context.SaveChangesAsync();
                    return RedirectToAction("Login");
                }

                foreach (var item in result.Errors)
                {
                    ModelState.AddModelError("", item.Description);
                }
            }
             
            return View(model);
               
        }
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {  
                ApplicationUser appUser = await userManager.FindByEmailAsync(model.Email);
                if (appUser != null)
                {
                    bool isPasswordCorrect = await userManager.CheckPasswordAsync(appUser,model.Password);
                    if (isPasswordCorrect)
                    {
                        await signInManager.SignInAsync(appUser, model.RememberMe);
                        if (await userManager.IsInRoleAsync(appUser, "Admin"))
                        {
                            return RedirectToAction("Profile", "AdminAdministration");
                        }
                        else if (await userManager.IsInRoleAsync(appUser, "Student"))
                        {
                            return RedirectToAction("Profile", "StudentAdministration");
                        }
                        else if (await userManager.IsInRoleAsync(appUser, "Instructor"))
                        {
                            return RedirectToAction("Profile", "InstructorAdministration");
                        }
                    }
                }
                ModelState.AddModelError("", "البريد الإلكتروني أو كلمة المرور غير صحيحة");
            }
            return View("Login", model);
        }



        [HttpPost]
        public async Task<IActionResult> SignOut()
        {
            await signInManager.SignOutAsync();
             HttpContext.Session.Clear();
            return RedirectToAction("Login","Account");
            //return View("Logout");
        }

        private  string Hash(string password, int iterations)
        {
            // Create salt
            byte[] salt;
            new RNGCryptoServiceProvider().GetBytes(salt = new byte[16]);

            // Create hash
            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations);
            var hash = pbkdf2.GetBytes(20);

            // Combine salt and hash
            var hashBytes = new byte[16 + 16];
            //Array.Copy(salt, 0, hashBytes, 0, 20);
            //Array.Copy(hash, 0, hashBytes, 16, 20);

            // Convert to base64
            var base64Hash = Convert.ToBase64String(hashBytes);

            // Format hash with extra information
            return string.Format("$MYHASH$V1${0}${1}", iterations, base64Hash);
        }


        private string HashPassword(string password)
        {
            var salt = new byte[128 / 8];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }
            var hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 10000,
                numBytesRequested: 256 / 8));
            return hashed;
        }

        private bool VerifyPassword(string password, string hashedPassword)
        {
            var salt = Convert.FromBase64String(hashedPassword);
            var hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 10000,
                numBytesRequested: 256 / 8));
            return hashed == hashedPassword;
        }
    }
}