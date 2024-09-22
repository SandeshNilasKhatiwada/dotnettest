using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using SydneyFitnessStudio;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Register the DbContext with the DI container
builder.Services.AddDbContext<FitnessStudioContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("FitnessStudioContext")));

// Add cookie-based authentication
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Person/Login";   // Redirect to Login page if not authenticated
        options.AccessDeniedPath = "/Person/AccessDenied";  // Redirect to Access Denied page if needed
        options.Events = new CookieAuthenticationEvents
        {
            OnValidatePrincipal = async context =>
            {
                // Custom logic for validating the principal
                // This is a place to handle claims-based logic if needed
                // For example, reloading user claims from the database if necessary
            }
        };
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage(); // Show detailed error pages during development
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();  // Enable HSTS for production
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Enable authentication and authorization middleware
app.UseAuthentication();  // Add this to enable authentication
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Person}/{action=Login}"); // Default route

app.Run();
