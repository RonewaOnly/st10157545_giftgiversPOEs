using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using ST10157545_GIFTGIVERS.Controllers;
using ST10157545_GIFTGIVERS.Models;
using st10157545_giftgiversPOEs.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Configure DbContext with SQL Server
builder.Services.AddDbContext<DatabaseController>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("AzureConnection")));

// Add authentication
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Auth/Login";
        options.LogoutPath = "/Auth/Logout";
        options.AccessDeniedPath = "/Auth/AccessDenied";
        options.Cookie.Name = "ST10157545_GIFTGIVERS.Auth";
        options.Cookie.HttpOnly = true;
        options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
        options.SlidingExpiration = true;
    });

// Register the session service
builder.Services.AddTransient<SessionService>(); // Register SessionService

// Register the background service
builder.Services.AddHostedService<TokenRefreshService>(); // Register TokenRefreshService

//Service for Subscription:
builder.Services.AddScoped<SubscriptionService>();

//adding UserRoles policies
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminPolicy", policy => policy.RequireRole("Admin"));
    options.AddPolicy("UserPolicy", policy => policy.RequireRole("User"));
    options.AddPolicy("VolunteerPolicy", policy => policy.RequireRole("Volunteer"));

});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication(); // Make sure authentication is used
app.UseAuthorization();


app.UseMiddleware<TokenMiddleware>(); //for automatice token refreshment 

// Custom middleware to redirect based on user roles
app.Use(async (context, next) =>
{
    if (context.User.Identity != null && context.User.Identity.IsAuthenticated)
    {
        if (context.User.IsInRole(UserType.Admin.ToString()))
        {
            // Redirect Admin to the admin dashboard if they are in the admin role
            if (!context.Request.Path.StartsWithSegments("/Admin"))
            {
                context.Response.Redirect("/Admin/Index");
                return;
            }
        }
        else if (context.User.IsInRole(UserType.User.ToString()))
        {
            // Normal users are redirected to the default user home if not already there
            if (!context.Request.Path.StartsWithSegments("/Home"))
            {
                context.Response.Redirect("/Home/Index");
                return;
            }
        }
        else if (context.User.IsInRole(UserType.Volunteer.ToString()))
        {
            // Normal users are redirected to the default user home if not already there
            if (!context.Request.Path.StartsWithSegments("/Volunteer"))
            {
                context.Response.Redirect("/Volunteer/Index");
                return;
            }
        }
    }

    // Call the next middleware in the pipeline
    await next.Invoke();
});

// Route for normal users
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Route for admin users
app.MapControllerRoute(
    name: "Admin",
    pattern: "Admin/{controller=Admin}/{action=Index}/{id?}");

// Route for volunteer users
app.MapControllerRoute(
    name: "volunteer",
    pattern: "Volunteer/{controller=Volunteer}/{action=Index}/{id?}");

app.Run();
