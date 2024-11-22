using DEPI_Project1.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Configure session
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(1);  // Session timeout duration
    options.Cookie.HttpOnly = true;               // Ensures the cookie is accessible only through HTTP requests
    options.Cookie.IsEssential = true;            // Marks the cookie as essential for GDPR compliance
});

// Configure authentication with cookie scheme
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
})
.AddCookie(options =>
{
    options.LoginPath = "/Account/Login";        // Redirect to Login when user is not authenticated
    options.LogoutPath = "/Account/Logout";      // Path for logging out
    options.ExpireTimeSpan = TimeSpan.FromHours(1);  // Cookie expiration time
    options.SlidingExpiration = true;            // Refreshes the cookie before expiration
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;  // Ensures the cookie is only sent over HTTPS
    options.Cookie.SameSite = SameSiteMode.Strict;  // Ensures strict SameSite enforcement
});

// Configure the DbContext to use SQL Server
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("ConStr1"));
});

// Add logging service
builder.Logging.ClearProviders();  // Clear default logging providers (optional)
builder.Logging.AddConsole();      // Add console logging
builder.Logging.AddDebug();        // Add debug logging

//builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
//    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.User.RequireUniqueEmail = true;
    options.Password.RequireDigit = false;
    options.Password.RequiredLength = 4;
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.User.AllowedUserNameCharacters = null; // Allow any characters in the username
})

.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();
var app = builder.Build();

#region  Perform database seeding

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var dbInitializer = new DbInitializer(dbContext,roleManager,userManager);
    dbInitializer.Seed().Wait();  // Call the seeding logic
}


#endregion

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error"); // Generic error handler in non-development environments
    app.UseHsts();                          // Use HSTS (Strict Transport Security)
}

app.UseHttpsRedirection();       // Redirects HTTP to HTTPS
app.UseStaticFiles();            // Serve static files
app.UseRouting();                // Routing middleware
app.UseSession();                // Enable session support
app.UseAuthentication();         // Enable authentication
app.UseAuthorization();          // Enable authorization

// Route configuration
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Exception handling for application startup
try
{
    app.Run();
}
catch (Exception ex)
{
    // Log any unhandled exception at the application level
    Console.WriteLine($"Application startup error: {ex.Message}");
}


//using Microsoft.EntityFrameworkCore;
//using Microsoft.AspNetCore.Session;
//using Microsoft.AspNetCore.Authentication.Cookies;

//var builder = WebApplication.CreateBuilder(args);

//// Add services to the container.
//builder.Services.AddControllersWithViews();
//builder.Services.AddSession(options =>
//{
//    options.IdleTimeout = TimeSpan.FromHours(1);
//    options.Cookie.HttpOnly = true;
//    options.Cookie.IsEssential = true;
//});

//builder.Services.AddAuthentication(options =>
//{
//    options.DefaultScheme = "Cookies";
//    options.DefaultChallengeScheme = "Cookies";
//})
//.AddCookie("Cookies", options =>
//{
//    options.LoginPath = "/Account/Login";
//    options.LogoutPath = "/Account/Logout";
//});

//builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("ConStr1")));

//var app = builder.Build();

//// Configure the HTTP request pipeline.
//if (!app.Environment.IsDevelopment())
//{
//    app.UseExceptionHandler("/Home/Error");
//    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
//    app.UseHsts();
//}

//app.UseHttpsRedirection();
//app.UseStaticFiles();

//app.UseRouting();
//app.UseSession();
//app.UseAuthentication(); // Add this line
//app.UseAuthorization();

//try
//{
//    app.MapControllerRoute(
//        name: "default", 
//        pattern: "{controller=Home}/{action=Index}/{id?}");
//}
//catch (Exception ex)
//{
//    // Log the exception
//    Console.WriteLine($"Error configuring routes: {ex.Message}");
//}

//app.Run();